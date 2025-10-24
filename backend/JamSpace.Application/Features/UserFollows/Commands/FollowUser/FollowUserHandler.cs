using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserFollows.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserFollows.Commands.FollowUser;

public class FollowUserHandler : IRequestHandler<FollowUserCommand, BasicUserFollowDto>
{
    private readonly IUserFollowRepository _userFollowRepository;

    public FollowUserHandler(IUserFollowRepository userFollowRepository)
    {
        _userFollowRepository = userFollowRepository;
    }

    public async Task<BasicUserFollowDto> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        bool alreadyFollowing = await _userFollowRepository.UserFollowsAsync(request.FollowerId, request.FollowedId, cancellationToken);
        if (alreadyFollowing)
        {
            throw new ConflictException("You are already following this user.");
        }
        
        var userFollow = await _userFollowRepository.FollowUserAsync(request.FollowerId, request.FollowedId, cancellationToken);
        return new BasicUserFollowDto
        {
            FollowerId = userFollow.FollowerId,
            FolloweeId = userFollow.FolloweeId
        };
    }
}