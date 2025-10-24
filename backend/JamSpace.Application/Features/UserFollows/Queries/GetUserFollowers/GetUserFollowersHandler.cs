using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserFollows.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserFollows.Queries.GetUserFollowers;

public class GetUserFollowersHandler : IRequestHandler<GetUserFollowersQuery, List<DetailedUserFollowDto>>
{
    private readonly IUserFollowRepository _userFollowRepository;
    
    public GetUserFollowersHandler(IUserFollowRepository userFollowRepository)
    {
        _userFollowRepository = userFollowRepository;
    }
    
    public async Task<List<DetailedUserFollowDto>> Handle(GetUserFollowersQuery request, CancellationToken cancellationToken)
    {
        var followers = await _userFollowRepository.GetFollowersAsync(request.UserId, cancellationToken);
        
        return followers.Select(userFollow => new DetailedUserFollowDto
        {
            FollowerId = userFollow.FollowerId,
            FolloweeId = userFollow.FolloweeId,
            FollowerUsername = userFollow.Follower.UserName,
            FollowerDisplayName = userFollow.Follower.DisplayName,
            FollowerPictureUrl = userFollow.Follower.ProfilePictureUrl
        }).ToList();

    }
}