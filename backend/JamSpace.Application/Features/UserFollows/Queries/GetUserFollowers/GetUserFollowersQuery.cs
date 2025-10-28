using JamSpace.Application.Features.UserFollows.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserFollows.Queries.GetUserFollowers;

public record GetUserFollowersQuery(Guid UserId, Guid? RequestingUserId) : IRequest<HashSet<DetailedUserFollowDto>>;