using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserFollows.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserFollows.Queries.GetUserFollowing;

public class GetUserFollowingHandler : IRequestHandler<GetUserFollowingQuery, List<UserFollowDto>>
{
    private readonly IUserFollowRepository _repository;
    
    public GetUserFollowingHandler(IUserFollowRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<List<UserFollowDto>> Handle(GetUserFollowingQuery request, CancellationToken cancellationToken)
    {
        var following = await _repository.GetFollowingAsync(request.UserId, cancellationToken);
        
        return following.Select(uf => new UserFollowDto
        {
            FollowerId = uf.FollowerId,
            FolloweeId = uf.FolloweeId
        }).ToList();
    }
}