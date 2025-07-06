using Microsoft.AspNetCore.Http;

namespace JamSpace.Application.Features.Uploads.UploadTeamPicture;

public class UploadTeamPictureRequest
{
    public IFormFile File { get; set; } = null!;
}
