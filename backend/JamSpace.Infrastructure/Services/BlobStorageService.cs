using JamSpace.Application.Interfaces;

namespace JamSpace.Infrastructure.Services;

using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
        var containerName = configuration["AzureBlobStorage:ContainerName"];

        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        await _containerClient.CreateIfNotExistsAsync();

        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, overwrite: true);

        return blobClient.Uri.ToString(); // <-- publiczny URL
    }
}
