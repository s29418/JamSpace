using MediatR;
using Microsoft.AspNetCore.Http;

namespace JamSpace.Application.Features.Uploads.UploadTeamPicture;

public record UploadTeamPictureCommand(IFormFile File) : IRequest<string>;
