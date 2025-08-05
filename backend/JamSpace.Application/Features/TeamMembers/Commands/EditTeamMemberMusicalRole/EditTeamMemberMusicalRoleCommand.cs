using JamSpace.Application.Features.TeamMembers.DTOs;
using MediatR;

namespace JamSpace.Application.Features.TeamMembers.Commands.EditTeamMemberMusicalRole;

public record EditTeamMemberMusicalRoleCommand(Guid TeamId, Guid RequestingUserId, Guid UserId, string MusicalRole) : IRequest<TeamMemberDto>;