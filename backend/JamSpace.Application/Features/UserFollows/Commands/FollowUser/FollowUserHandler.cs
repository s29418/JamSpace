using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.UserFollows.DTOs;
using JamSpace.Domain.Entities;
using MediatR;

namespace JamSpace.Application.Features.UserFollows.Commands.FollowUser;

public class FollowUserHandler : IRequestHandler<FollowUserCommand, BasicUserFollowDto>
{
    private readonly IUserFollowRepository _userFollowRepository;
    private readonly IUnitOfWork _uow;

    public FollowUserHandler(IUserFollowRepository userFollowRepository, IUnitOfWork uow)
    {
        _userFollowRepository = userFollowRepository;
        _uow = uow;
    }

    public async Task<BasicUserFollowDto> Handle(FollowUserCommand request, CancellationToken ct)
    {
        var alreadyFollowing = await _userFollowRepository.UserFollowsAsync(request.FollowerId, request.FollowedId, ct);
        if (alreadyFollowing)
            throw new ConflictException("You are already following this user.");

        var userFollow = new UserFollow
        {
            FollowerId = request.FollowerId,
            FolloweeId = request.FollowedId,
            FollowedAt = DateTime.UtcNow
        };

        await _userFollowRepository.AddAsync(userFollow, ct);
        await _uow.SaveChangesAsync(ct);

        return new BasicUserFollowDto
        {
            FollowerId = userFollow.FollowerId,
            FolloweeId = userFollow.FolloweeId
        };
    }
}