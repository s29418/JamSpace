using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamMembers.Commands.KickTeamMember;

public sealed class KickTeamMemberHandler : IRequestHandler<KickTeamMemberCommand, Unit>
{
    private readonly ITeamMemberRepository _repo;
    private readonly IUnitOfWork _uow;

    public KickTeamMemberHandler(ITeamMemberRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Unit> Handle(KickTeamMemberCommand request, CancellationToken ct)
    {
        if (!await _repo.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Leader, ct))
            throw new ForbiddenAccessException("Only team leader can kick members.");

        if (await _repo.HasRequiredRoleAsync(request.TeamId, request.UserId, FunctionalRole.Leader, ct))
            throw new ConflictException("Cannot kick a team leader.");

        var member = await _repo.GetByTeamAndUserAsync(request.TeamId, request.UserId, ct)
                     ?? throw new NotFoundException("Team member not found.");

        _repo.Remove(member);
        await _uow.SaveChangesAsync(ct);

        return Unit.Value;
    }
}