using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using MediatR;

namespace JamSpace.Application.Features.UserFollows.Commands.UnfollowUser;

public class UnfollowUserHandler : IRequestHandler<UnfollowUserCommand, Unit>
{
    private readonly IUserFollowRepository _userFollowRepository;
    private readonly IUnitOfWork _uow;

    public UnfollowUserHandler(IUserFollowRepository userFollowRepository, IUnitOfWork uow)
    {
        _userFollowRepository = userFollowRepository;
        _uow = uow;
    }

    public async Task<Unit> Handle(UnfollowUserCommand request, CancellationToken ct)
    {
        var userFollow = await _userFollowRepository.GetAsync(request.FollowerId, request.FollowedId, ct);
        if (userFollow is null)
            throw new ConflictException("You are not following this user.");

        _userFollowRepository.Remove(userFollow);
        await _uow.SaveChangesAsync(ct);

        return Unit.Value;
    }
}