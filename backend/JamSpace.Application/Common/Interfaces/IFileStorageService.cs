using JamSpace.Application.Common.Enums;
using JamSpace.Application.Common.Models;

namespace JamSpace.Application.Common.Interfaces;


public interface IFileStorageService
{
    Task<string> UploadAsync(FileUpload file, StorageObjectType type, Guid? relatedEntityId, CancellationToken ct);
    Task DeleteAsync(string fileUrl, CancellationToken ct);
}