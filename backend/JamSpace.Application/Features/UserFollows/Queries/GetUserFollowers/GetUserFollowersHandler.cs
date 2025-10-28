using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserFollows.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserFollows.Queries.GetUserFollowers;

public class GetUserFollowersHandler : IRequestHandler<GetUserFollowersQuery, HashSet<DetailedUserFollowDto>>
{
    private readonly IUserFollowRepository _userFollowRepository;
    
    public GetUserFollowersHandler(IUserFollowRepository userFollowRepository)
    {
        _userFollowRepository = userFollowRepository;
    }
    
    public async Task<HashSet<DetailedUserFollowDto>> Handle(GetUserFollowersQuery request, CancellationToken cancellationToken)
    {
        var requestingUserFollowingSet = request.RequestingUserId.HasValue
            ? await _userFollowRepository.GetFollowingAsync(request.RequestingUserId.Value, cancellationToken)
            : null;
        
        var followers = await _userFollowRepository.GetFollowersAsync(request.UserId, cancellationToken);

        return followers.Select(userFollow => new DetailedUserFollowDto
        {
            FollowerId = userFollow.FollowerId,
            FolloweeId = userFollow.FolloweeId,
            FollowerUsername = userFollow.Follower.UserName,
            FollowerDisplayName = userFollow.Follower.DisplayName,
            FollowerPictureUrl = userFollow.Follower.ProfilePictureUrl,
            IsFollowing = requestingUserFollowingSet != null &&
                          requestingUserFollowingSet.Any(f => f.FolloweeId == userFollow.FollowerId)
        }).ToHashSet();
    }

}