using MediatR;

namespace JamSpace.Application.Features.TeamMembers.Commands.KickTeamMember;

public record KickTeamMemberCommand(Guid TeamId, Guid RequestingUserId,Guid UserId) : IRequest<Unit>;