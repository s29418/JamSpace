using JamSpace.Application.Common.Enums;

namespace JamSpace.API.Requests;

public class UploadPictureRequest
{
    public IFormFile File { get; set; } = null!;
    public PictureType Type { get; set; }
    public Guid? RelatedEntityId { get; set; }
}
