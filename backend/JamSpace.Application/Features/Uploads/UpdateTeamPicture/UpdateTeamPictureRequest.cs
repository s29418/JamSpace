using Microsoft.AspNetCore.Http;

namespace JamSpace.Application.Features.Uploads.UpdateTeamPicture;

public class UpdateTeamPictureRequest
{
    public IFormFile File { get; set; } = null!;
}
