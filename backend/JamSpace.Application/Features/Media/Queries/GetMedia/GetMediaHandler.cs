using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using MediatR;

namespace JamSpace.Application.Features.Media.Queries.GetMedia;

public sealed class GetMediaHandler : IRequestHandler<GetMediaQuery, StoredFileDownload?>
{
    private readonly IFileStorageService _fileStorageService;

    public GetMediaHandler(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    public Task<StoredFileDownload?> Handle(GetMediaQuery request, CancellationToken cancellationToken)
    {
        return _fileStorageService.DownloadAsync(request.Url, cancellationToken);
    }
}
