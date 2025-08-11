using Microsoft.AspNetCore.Http;

namespace JamSpace.Application.Features.Uploads.UploadPicture;

public class UploadPictureRequest
{
    public IFormFile File { get; set; } = null!;
}
