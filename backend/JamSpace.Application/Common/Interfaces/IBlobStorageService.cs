namespace JamSpace.Application.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
}