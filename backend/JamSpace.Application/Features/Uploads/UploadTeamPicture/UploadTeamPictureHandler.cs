using JamSpace.Application.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Uploads.UploadTeamPicture;

public class UploadTeamPictureHandler : IRequestHandler<UploadTeamPictureCommand, string>
{
    private readonly IBlobStorageService _blobService;

    public UploadTeamPictureHandler(IBlobStorageService blobService)
    {
        _blobService = blobService;
    }

    public async Task<string> Handle(UploadTeamPictureCommand request, CancellationToken cancellationToken)
    {
        var file = request.File;
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        using var stream = file.OpenReadStream();
        var url = await _blobService.UploadAsync(stream, fileName, file.ContentType);

        return url;
    }
}
