using JamSpace.Application.Common.Enums;

namespace JamSpace.API.Requests;

public class UploadPictureRequest
{
    public IFormFile? File { get; set; } = null!;
}
