using MediatR;
using Microsoft.AspNetCore.Http;

namespace JamSpace.Application.Features.Uploads.UpdateTeamPicture;

public class UpdateTeamPictureCommand : IRequest<string>
{
    public Guid TeamId { get; set; }
    public Guid RequestingUserId { get; set; }
    public IFormFile File { get; set; } = null!;
}