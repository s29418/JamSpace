using JamSpace.Application.Features.Teams.Dtos;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.EditTeamMemberMusicalRole;

public record EditTeamMemberMusicalRoleCommand(Guid TeamId, Guid RequestingUserId, Guid UserId, string MusicalRole) : IRequest<TeamMemberDto>;