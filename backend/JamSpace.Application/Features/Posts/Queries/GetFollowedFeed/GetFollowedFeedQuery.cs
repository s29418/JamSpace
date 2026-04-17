using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Posts.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Posts.Queries.GetFollowedFeed;

public record GetFollowedFeedQuery(Guid UserId, DateTimeOffset? Before, int Take) : IRequest<CursorResult<PostDto>>;