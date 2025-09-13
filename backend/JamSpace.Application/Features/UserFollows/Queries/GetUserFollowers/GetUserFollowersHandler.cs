using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserFollows.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserFollows.Queries.GetUserFollowers;

public class GetUserFollowersHandler : IRequestHandler<GetUserFollowersQuery, List<UserFollowDto>>
{
    private readonly IUserFollowRepository _userFollowRepository;
    
    public GetUserFollowersHandler(IUserFollowRepository userFollowRepository)
    {
        _userFollowRepository = userFollowRepository;
    }
    
    public async Task<List<UserFollowDto>> Handle(GetUserFollowersQuery request, CancellationToken cancellationToken)
    {
        var followers = await _userFollowRepository.GetFollowersAsync(request.UserId, cancellationToken);
        
        return followers.Select(f => new UserFollowDto
        {
            FollowerId = f.FollowerId,
            FolloweeId = f.FolloweeId
        }).ToList();
    }
}