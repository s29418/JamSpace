using JamSpace.Application.Features.UserFollows.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserFollows.Queires.GetUserFollowing;

public record GetUserFollowingQuery(Guid UserId) : IRequest<List<UserFollowDto>>;