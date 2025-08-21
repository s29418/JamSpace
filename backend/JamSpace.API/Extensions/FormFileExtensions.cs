using JamSpace.Application.Common.Models;

namespace JamSpace.API.Extensions;

public static class FormFileExtensions
{
    public static FileUpload ToFileUpload(this IFormFile file)
        => new()
        {
            Content = file.OpenReadStream(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            Length = file.Length
        };
}