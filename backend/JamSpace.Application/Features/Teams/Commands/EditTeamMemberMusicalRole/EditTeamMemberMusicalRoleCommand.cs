using JamSpace.Application.Common.Features.Teams.Dtos;
using MediatR;

namespace JamSpace.Application.Common.Features.Teams.Commands.EditTeamMemberMusicalRole;

public record EditTeamMemberMusicalRoleCommand(Guid TeamId, Guid RequestingUserId, Guid UserId, string MusicalRole) : IRequest<TeamMemberDto>;