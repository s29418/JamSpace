using Microsoft.AspNetCore.Http;

namespace JamSpace.Application.Features.Uploads.UploadTeamPicture;

public class UploadPictureRequest
{
    public IFormFile File { get; set; } = null!;
}
