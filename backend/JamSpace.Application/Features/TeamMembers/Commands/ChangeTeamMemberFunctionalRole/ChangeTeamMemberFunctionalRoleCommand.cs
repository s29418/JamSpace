using JamSpace.Application.Features.TeamMembers.DTOs;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamMembers.Commands.ChangeTeamMemberFunctionalRole;

public record ChangeTeamMemberFunctionalRoleCommand(Guid TeamId, Guid RequestingUserId, Guid UserId, FunctionalRole NewRole) : IRequest<TeamMemberDto>;