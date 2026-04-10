using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Posts.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Posts.Queries.GetUserFeed;

public record GetUserPostsQuery(Guid AuthorId, DateTimeOffset? Before, int Take) : IRequest<CursorResult<PostDto>>;