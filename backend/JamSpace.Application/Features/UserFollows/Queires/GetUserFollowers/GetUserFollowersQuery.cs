using JamSpace.Application.Features.UserFollows.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserFollows.Queires.GetUserFollowers;

public record GetUserFollowersQuery(Guid UserId) : IRequest<List<UserFollowDto>>;