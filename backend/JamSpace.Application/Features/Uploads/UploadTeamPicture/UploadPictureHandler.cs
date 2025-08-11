using JamSpace.Application.Common.Enums;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Uploads.UploadTeamPicture;

public class UploadPictureHandler : IRequestHandler<UploadPictureCommand, string>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ITeamRepository _teamRepository;

    public UploadPictureHandler(IFileStorageService fileStorageService, ITeamRepository teamRepository)
    {
        _fileStorageService = fileStorageService;
        _teamRepository = teamRepository;
    }

    public async Task<string> Handle(UploadPictureCommand request, CancellationToken cancellationToken)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";

        using var stream = request.File.OpenReadStream();
        var url = await _fileStorageService.UploadAsync(
            stream,
            fileName,
            request.File.ContentType,
            request.Type 
        );

        if (request.Type == PictureType.Team && request.RelatedEntityId.HasValue)
        {
            await _teamRepository.UpdateTeamPictureAsync(
                request.RelatedEntityId.Value,
                request.RequestingUserId ?? Guid.Empty,
                url,
                cancellationToken
            );
        }

        return url;
    }
}
