using JamSpace.Application.Common.Enums;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Teams.Commands.UpdateTeamPicture;
using JamSpace.Application.Features.Users.Commands.UpdateUserProfile;
using JamSpace.Application.Features.Users.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Uploads.UploadPicture;

public sealed class UploadPictureHandler : IRequestHandler<UploadPictureCommand, string>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IMediator _mediator;

    public UploadPictureHandler(IFileStorageService fileStorageService, IMediator mediator)
    {
        _fileStorageService = fileStorageService;
        _mediator = mediator;
    }

    public async Task<string> Handle(UploadPictureCommand request, CancellationToken ct)
    {
        var url = await _fileStorageService.UploadAsync(
            request.File,
            request.Type,
            request.RelatedEntityId,
            ct
        );

        try
        {
            switch (request.Type)
            {
                case PictureType.TeamPicture:
                {
                    if (!request.RelatedEntityId.HasValue)
                        throw new InvalidOperationException("TeamPicture requires RelatedEntityId (teamId).");

                    if (!request.RequestingUserId.HasValue)
                        throw new InvalidOperationException("TeamPicture requires RequestingUserId.");

                    await _mediator.Send(new UpdateTeamPictureCommand(
                        TeamId: request.RelatedEntityId.Value,
                        RequestingUserId: request.RequestingUserId.Value,
                        PictureUrl: url
                    ), ct);

                    break;
                }

                case PictureType.UserPicture:
                {
                    if (!request.RelatedEntityId.HasValue)
                        throw new InvalidOperationException("UserPicture requires RelatedEntityId (userId).");

                    // U Ciebie UpdateUserProfileCommand zwraca UserDto, ale my
                    // potrzebujemy tylko aby to się wykonało – wynik ignorujemy.
                    _ = await _mediator.Send(new UpdateUserProfileCommand(
                        UserId: request.RelatedEntityId.Value,

                        SetDisplayName: false,
                        DisplayName: null,

                        SetBio: false,
                        Bio: null,

                        SetProfilePicture: true,
                        ProfilePictureUrl: url,

                        SetLocation: false,
                        Location: (LocationDto?)null,

                        SetEmail: false,
                        Email: null
                    ), ct);

                    break;
                }

                default:
                    throw new NotSupportedException($"Unsupported picture type: {request.Type}");
            }

            return url;
        }
        catch
        {
            // kompensacja: DB update padł -> kasujemy blob
            try { await _fileStorageService.DeleteAsync(url, ct); }
            catch { /* nie przykrywamy oryginalnego wyjątku */ }

            throw;
        }
    }
}