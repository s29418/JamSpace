using FluentValidation;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Users.DTOs;
using JamSpace.Application.Features.Users.Mappers;
using JamSpace.Domain.ValueObjects;
using MediatR;

namespace JamSpace.Application.Features.Users.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileHandler
    : IRequestHandler<UpdateUserProfileCommand, UserDto>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public UpdateUserProfileHandler(IUserRepository users, IUnitOfWork uow)
    {
        _users = users;
        _uow = uow;
    }

    public async Task<UserDto> Handle(UpdateUserProfileCommand c, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(c.UserId, ct)
                   ?? throw new NotFoundException("User not found.");

        if (c.SetDisplayName)
            user.DisplayName = c.DisplayName.Trim();
        
        if (c.SetBio)
            user.Bio = c.Bio?.Trim();
        
        
        if (c.SetProfilePicture)
        {
            if (c.ProfilePictureUrl is not null &&
                !Uri.IsWellFormedUriString(c.ProfilePictureUrl, UriKind.Absolute))
                throw new ValidationException("Invalid profile picture URL.");
            user.ProfilePictureUrl = c.ProfilePictureUrl;
        }

        if (c.SetLocation)
        {
            user.Location = c.Location is null ? null
                : new Location
                {
                    City = c.Location.City.Trim(), 
                    CountryCode = c.Location.Country.Trim()
                };
        }
        
        if (c.SetEmail)
        {
            var existing = await _users.GetByEmailAsync(c.Email.Trim(), ct);
            if (existing is not null && existing.Id != user.Id)
                throw new ConflictException("Email already in use.");
            user.Email = c.Email.Trim();
        }

        await _uow.SaveChangesAsync(ct);
        return user.ToDto(true, false);
    }
}
