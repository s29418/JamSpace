using JamSpace.Application.Features.UserFollows.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserFollows.Queries.GetUserFollowing;

public record GetUserFollowingQuery(Guid UserId, Guid? RequestingUserId) : IRequest<List<DetailedUserFollowDto>>;