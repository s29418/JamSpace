using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.TeamMembers.Commands.KickTeamMember;

public class KickTeamMemberHandler : IRequestHandler<KickTeamMemberCommand, Unit>
{
    private readonly ITeamMemberRepository _repo;

    public KickTeamMemberHandler(ITeamMemberRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(KickTeamMemberCommand request, CancellationToken ct)
    {
        if (!await _repo.IsUserALeaderAsync(request.TeamId, request.RequestingUserId, ct)) 
            throw new ForbiddenAccessException("Only team leader can kick members.");

        if (await _repo.IsUserALeaderAsync(request.TeamId, request.UserId, ct))
            throw new ConflictException("Cannot kick a team leader.");

        await _repo.DeleteTeamMemberAsync(request.TeamId, request.UserId, ct);
        return Unit.Value;
    }
}