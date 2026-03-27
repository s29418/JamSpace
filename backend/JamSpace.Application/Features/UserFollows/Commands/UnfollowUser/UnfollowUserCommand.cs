using MediatR;

namespace JamSpace.Application.Features.UserFollows.Commands.UnfollowUser;

public record UnfollowUserCommand(Guid FollowerId, Guid FollowedId) : IRequest<Unit>;