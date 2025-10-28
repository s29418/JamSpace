using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserFollows.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserFollows.Queries.GetUserFollowing;

public class GetUserFollowingHandler : IRequestHandler<GetUserFollowingQuery, List<DetailedUserFollowDto>>
{
    private readonly IUserFollowRepository _repository;
    
    public GetUserFollowingHandler(IUserFollowRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<List<DetailedUserFollowDto>> Handle(GetUserFollowingQuery request, CancellationToken cancellationToken)
    {
        var requestingUserFollowingSet = request.RequestingUserId.HasValue 
            ? await _repository.GetFollowingAsync(request.RequestingUserId.Value, cancellationToken) 
            : null;
        
        var following = await _repository.GetFollowingAsync(request.UserId, cancellationToken);
        
        return following.Select(uf => new DetailedUserFollowDto
        {
            FollowerId = uf.FollowerId,
            FollowerUsername = uf.Followee.UserName,
            FollowerDisplayName = uf.Followee.DisplayName,
            FollowerPictureUrl = uf.Followee.ProfilePictureUrl,
            FolloweeId = uf.FolloweeId,
            IsFollowing = requestingUserFollowingSet != null &&
                          requestingUserFollowingSet.Any(f => f.FolloweeId == uf.FolloweeId)      
        }).ToList();
    }
}