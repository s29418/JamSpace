using JamSpace.Application.Features.Teams.Dtos;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.CreateTeam;

public record CreateTeamWithUserCommand(CreateTeamCommand Command, Guid CreatorUserId) : IRequest<TeamDto>;
