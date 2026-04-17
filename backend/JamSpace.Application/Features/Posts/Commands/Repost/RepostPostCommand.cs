using JamSpace.Application.Features.Posts.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.Repost;

public record RepostPostCommand(Guid UserId, Guid OriginalPostId) : IRequest<PostDto>;