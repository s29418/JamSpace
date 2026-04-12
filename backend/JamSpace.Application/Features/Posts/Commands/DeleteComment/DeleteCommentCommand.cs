using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.DeleteComment;

public record DeleteCommentCommand(Guid CommentId, Guid RequestingUserId) : IRequest<Unit>;