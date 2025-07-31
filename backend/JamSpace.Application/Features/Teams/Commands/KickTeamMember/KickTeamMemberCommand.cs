using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.KickTeamMember;

public record KickTeamMemberCommand(Guid TeamId, Guid RequestingUserId,Guid UserId) : IRequest<Unit>;