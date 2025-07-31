using MediatR;

namespace JamSpace.Application.Common.Features.Teams.Commands.KickTeamMember;

public record KickTeamMemberCommand(Guid TeamId, Guid RequestingUserId,Guid UserId) : IRequest<Unit>;