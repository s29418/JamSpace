using Azure.Storage.Blobs;
using JamSpace.Application.Common.Enums;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;

namespace JamSpace.Infrastructure.Services;


public class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public AzureBlobStorageService(BlobServiceClient blobServiceClient, string containerName)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = containerName; 
    }

    public async Task<string> UploadAsync(FileUpload file, PictureType type, Guid? relatedEntityId, CancellationToken ct)
    {
        var container = _blobServiceClient.GetBlobContainerClient(_containerName);
        await container.CreateIfNotExistsAsync(cancellationToken: ct);

        var blobName = BuildBlobName(type, relatedEntityId, file.FileName);
        var blobClient = container.GetBlobClient(blobName);
        
        await blobClient.UploadAsync(file.Content, overwrite: true, cancellationToken: ct);

        return blobClient.Uri.ToString();
    }

    private static string BuildBlobName(PictureType type, Guid? relatedEntityId, string fileName)
    {
        var typeFolder = type.ToString().ToLowerInvariant();        
        var entityFolder = (relatedEntityId ?? Guid.NewGuid()).ToString("N");
        var uniqueName = $"{Guid.NewGuid():N}-{fileName}";
        return $"{typeFolder}/{entityFolder}/{uniqueName}";
    }
}
