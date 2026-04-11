using JamSpace.Application.Common.Enums;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Users.DTOs;
using JamSpace.Application.Features.Users.Mappers;
using MediatR;

namespace JamSpace.Application.Features.Users.Commands.UpdateUserProfilePicture;

public sealed class UpdateUserProfilePictureHandler
    : IRequestHandler<UpdateUserProfilePictureCommand, UserDto>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IFileStorageService _fileStorageService;

    public UpdateUserProfilePictureHandler(
        IUserRepository users,
        IUnitOfWork uow,
        IFileStorageService fileStorageService)
    {
        _users = users;
        _uow = uow;
        _fileStorageService = fileStorageService;
    }

    public async Task<UserDto> Handle(UpdateUserProfilePictureCommand c, CancellationToken ct)
    {
        if (c.UserId != c.RequestingUserId)
            throw new NotFoundException("You can only modify your own profile.");

        var user = await _users.GetByIdAsync(c.UserId, ct)
                   ?? throw new NotFoundException("User not found.");

        var oldPictureUrl = user.ProfilePictureUrl;
        string? newPictureUrl = null;

        try
        {
            newPictureUrl = await _fileStorageService.UploadAsync(
                c.File,
                StorageObjectType.UserPicture,
                c.UserId,
                ct);

            user.ProfilePictureUrl = newPictureUrl;

            await _uow.SaveChangesAsync(ct);
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(newPictureUrl))
            {
                try
                {
                    await _fileStorageService.DeleteAsync(newPictureUrl, ct);
                }
                catch
                {
                    // ignored
                }
            }

            throw;
        }

        if (!string.IsNullOrWhiteSpace(oldPictureUrl) &&
            !string.Equals(oldPictureUrl, newPictureUrl, StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                await _fileStorageService.DeleteAsync(oldPictureUrl, ct);
            }
            catch
            {
                // ignored
            }
        }

        return user.ToDto(true, false);
    }
}