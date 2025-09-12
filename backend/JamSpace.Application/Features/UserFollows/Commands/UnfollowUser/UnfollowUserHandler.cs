using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.UserFollows.Commands.UnfollowUser;

public class UnfollowUserHandler : IRequestHandler<UnfollowUserCommand, Unit>
{
    private readonly IUserFollowRepository _userFollowRepository;
    
    public UnfollowUserHandler(IUserFollowRepository userFollowRepository)
    {
        _userFollowRepository = userFollowRepository;
    }
    
    public async Task<Unit> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
    {
        bool follows = await _userFollowRepository.UserFollowsAsync(request.FollowerId, request.FollowedId, cancellationToken);
        if (!follows)
        {
            throw new ConflictException("You are not following this user.");
        }

        await _userFollowRepository.UnfollowUserAsync(request.FollowerId, request.FollowedId, cancellationToken);
        
        return Unit.Value;
    }
}