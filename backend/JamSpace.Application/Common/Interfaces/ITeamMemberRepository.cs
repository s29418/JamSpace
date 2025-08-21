using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Application.Common.Interfaces;

public interface ITeamMemberRepository
{
    Task<bool> IsUserInTeamAsync(Guid teamId, Guid userId, CancellationToken ct);
    Task<bool> IsUserALeaderAsync(Guid teamId, Guid userId, CancellationToken ct);
    Task<bool> IsUserAnAdminAsync(Guid teamId, Guid userId, CancellationToken ct);
    Task<TeamMember> GetTeamMemberAsync(Guid teamId, Guid userId, CancellationToken ct);
    Task<List<TeamMember>> GetLeadersAsync(Guid teamId, CancellationToken ct);
    Task<TeamMember> ChangeTeamMemberFunctionalRoleAsync(Guid teamId, Guid userId, FunctionalRole newRole, CancellationToken ct);
    Task<TeamMember> EditTeamMemberMusicalRole(Guid teamId, Guid userId, string musicalRole, CancellationToken ct);
    Task DeleteTeamMemberAsync(Guid teamId, Guid userId, CancellationToken ct);
}