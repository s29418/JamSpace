using JamSpace.Application.Features.Posts.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Posts.Queries.GetPostComment;

public record GetPostCommentQuery(Guid CommentId) : IRequest<PostCommentDto>;