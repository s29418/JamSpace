using JamSpace.Application.Features.Posts.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.CommentPost;

public record CommentPostCommand(Guid UserId, Guid PostId, string Content) : IRequest<PostCommentDto>;