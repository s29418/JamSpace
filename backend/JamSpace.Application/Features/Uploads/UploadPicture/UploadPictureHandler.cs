using JamSpace.Application.Common.Enums;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Uploads.UploadPicture;

public class UploadPictureHandler : IRequestHandler<UploadPictureCommand, string>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ITeamRepository _teamRepository;

    public UploadPictureHandler(IFileStorageService fileStorageService, ITeamRepository teamRepository)
    {
        _fileStorageService = fileStorageService;
        _teamRepository = teamRepository;
    }

    public async Task<string> Handle(UploadPictureCommand request, CancellationToken ct)
    {
        var url = await _fileStorageService.UploadAsync(
            request.File, 
            request.Type, 
            request.RelatedEntityId, 
            ct
        );

        if (request.Type == PictureType.TeamPicture && request.RelatedEntityId.HasValue)
        {
            await _teamRepository.UpdateTeamPictureAsync(
                request.RelatedEntityId.Value,
                request.RequestingUserId ?? Guid.Empty,
                url,
                ct
            );
        }

        return url;
    }
}
