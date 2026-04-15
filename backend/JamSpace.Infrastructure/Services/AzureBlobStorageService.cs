using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using JamSpace.Application.Common.Enums;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using Azure;

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

    public async Task<string> UploadAsync(FileUpload file, StorageObjectType type, Guid? relatedEntityId, CancellationToken ct)
    {
        var container = _blobServiceClient.GetBlobContainerClient(_containerName);
        await container.CreateIfNotExistsAsync(cancellationToken: ct);

        var blobName = BuildBlobName(type, relatedEntityId, file.FileName);
        var blobClient = container.GetBlobClient(blobName);

        await blobClient.UploadAsync(
            file.Content,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                        ? "application/octet-stream"
                        : file.ContentType
                }
            },
            cancellationToken: ct);

        return blobClient.Uri.ToString();
    }

    public async Task DeleteAsync(string fileUrl, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            return;

        var uri = new Uri(fileUrl);
        var absolutePath = uri.AbsolutePath.TrimStart('/');

        var prefix = _containerName.Trim('/') + "/";
        if (!absolutePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return;

        var blobPath = absolutePath.Substring(prefix.Length);

        var container = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = container.GetBlobClient(blobPath);

        await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
    }

    public async Task<StoredFileDownload?> DownloadAsync(string fileUrl, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            return null;

        if (!TryGetBlobPath(fileUrl, out var blobPath))
            return null;

        var container = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = container.GetBlobClient(blobPath);

        try
        {
            var response = await blobClient.DownloadStreamingAsync(cancellationToken: ct);
            var contentType = response.Value.Details.ContentType;

            return new StoredFileDownload
            {
                Content = response.Value.Content,
                ContentType = string.IsNullOrWhiteSpace(contentType)
                    ? "application/octet-stream"
                    : contentType,
                FileName = Path.GetFileName(blobPath)
            };
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    private bool TryGetBlobPath(string fileUrl, out string blobPath)
    {
        blobPath = string.Empty;

        if (!Uri.TryCreate(fileUrl, UriKind.Absolute, out var uri))
            return false;

        var storageUri = _blobServiceClient.Uri;
        if (!string.Equals(uri.Host, storageUri.Host, StringComparison.OrdinalIgnoreCase))
            return false;

        var absolutePath = uri.AbsolutePath.TrimStart('/');
        var prefix = _containerName.Trim('/') + "/";

        if (!absolutePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return false;

        blobPath = absolutePath.Substring(prefix.Length);
        return !string.IsNullOrWhiteSpace(blobPath);
    }

    private static string BuildBlobName(StorageObjectType type, Guid? relatedEntityId, string fileName)
    {
        var typeFolder = type.ToString().ToLowerInvariant();
        var entityFolder = (relatedEntityId ?? Guid.NewGuid()).ToString("N");
        var uniqueName = $"{Guid.NewGuid():N}-{fileName}";
        return $"{typeFolder}/{entityFolder}/{uniqueName}";
    }
}