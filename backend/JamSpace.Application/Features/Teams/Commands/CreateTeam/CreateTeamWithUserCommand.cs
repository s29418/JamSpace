using JamSpace.Application.Common.Features.Teams.Dtos;
using MediatR;

namespace JamSpace.Application.Common.Features.Teams.Commands.Create;

public record CreateTeamWithUserCommand(CreateTeamCommand Command, Guid CreatorUserId) : IRequest<TeamDto>;
