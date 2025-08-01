using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Uploads.UpdateTeamPicture;

public class UpdateTeamPictureHandler : IRequestHandler<UpdateTeamPictureCommand, string>
{
    private readonly IBlobStorageService _blobService;
    private readonly ITeamRepository _repo;

    public UpdateTeamPictureHandler(IBlobStorageService blobService, ITeamRepository repo)
    {
        _blobService = blobService;
        _repo = repo;
    }

    public async Task<string> Handle(UpdateTeamPictureCommand request, CancellationToken ct)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
        using var stream = request.File.OpenReadStream();

        var url = await _blobService.UploadAsync(stream, fileName, request.File.ContentType);
        
        await _repo.UpdateTeamPictureAsync(request.TeamId, request.RequestingUserId, url, ct);
        return url;
    }
}
