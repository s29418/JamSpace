using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using JamSpace.Application.Common.Enums;
using JamSpace.Application.Common.Interfaces;

namespace JamSpace.Infrastructure.Services;

public class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobContainerClient _containerClient;

    public AzureBlobStorageService(BlobServiceClient blobServiceClient, string containerName)
    {
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        _containerClient.CreateIfNotExists(PublicAccessType.Blob);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, PictureType pictureType)
    {
        var blobName = $"{pictureType.ToString().ToLowerInvariant()}/{fileName}";
        var blobClient = _containerClient.GetBlobClient(blobName);

        var headers = new BlobHttpHeaders { ContentType = contentType };
        await blobClient.UploadAsync(fileStream, headers);

        return blobClient.Uri.ToString();
    }
}