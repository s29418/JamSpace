using JamSpace.Application.Common.Enums;

namespace JamSpace.Application.Common.Interfaces;


public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, PictureType pictureType);
}