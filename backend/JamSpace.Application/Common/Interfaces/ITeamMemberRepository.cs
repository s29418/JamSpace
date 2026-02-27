using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Application.Common.Interfaces;

public interface ITeamMemberRepository
{
    Task<bool> HasRequiredRoleAsync(Guid teamId, Guid userId, FunctionalRole minimumRole, CancellationToken ct);

    Task<TeamMember?> GetByTeamAndUserAsync(Guid teamId, Guid userId, CancellationToken ct);
    Task<List<TeamMember>> GetLeadersAsync(Guid teamId, CancellationToken ct);

    Task AddAsync(TeamMember member, CancellationToken ct);
    void Remove(TeamMember member);
}