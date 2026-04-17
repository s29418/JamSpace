using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.DeleteRepost;

public record DeleteRepostCommand(Guid UserId, Guid OriginalPostId) : IRequest<Unit>;