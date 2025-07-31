using JamSpace.Application.Common.Features.Teams.Dtos;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Common.Features.Teams.Commands.ChangeTeamMemberFunctionalRole;

public record ChangeTeamMemberFunctionalRoleCommand(Guid TeamId, Guid RequestingUserId, Guid UserId, FunctionalRole NewRole) : IRequest<TeamMemberDto>;