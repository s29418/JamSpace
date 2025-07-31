using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.KickTeamMember;

public class KickTeamMemberHandler : IRequestHandler<KickTeamMemberCommand, Unit>
{
    private readonly ITeamRepository _repo;

    public KickTeamMemberHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(KickTeamMemberCommand request, CancellationToken ct)
    {
        if (!await _repo.IsUserALeaderAsync(request.TeamId, request.RequestingUserId)) 
            throw new ForbiddenAccessException("Only team leader can kick members.");

        if (!await _repo.IsUserALeaderAsync(request.TeamId, request.UserId))
            throw new ConflictException("Cannot kick a team leader.");

        await _repo.KickTeamMemberAsync(request.TeamId, request.UserId, ct);
        return Unit.Value;
    }
}