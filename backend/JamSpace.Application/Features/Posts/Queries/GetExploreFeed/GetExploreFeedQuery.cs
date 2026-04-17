using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Posts.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Posts.Queries.GetExploreFeed;

public record GetExploreFeedQuery(Guid? UserId, DateTimeOffset? Before, int Take) : IRequest<CursorResult<PostDto>>;