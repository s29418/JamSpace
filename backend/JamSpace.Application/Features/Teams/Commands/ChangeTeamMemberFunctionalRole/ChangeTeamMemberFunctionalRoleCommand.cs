using JamSpace.Application.Features.Teams.Dtos;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.ChangeTeamMemberFunctionalRole;

public record ChangeTeamMemberFunctionalRoleCommand(Guid TeamId, Guid RequestingUserId, Guid UserId, FunctionalRole NewRole) : IRequest<TeamMemberDto>;