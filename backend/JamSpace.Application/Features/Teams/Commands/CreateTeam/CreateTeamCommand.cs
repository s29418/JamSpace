using JamSpace.Application.Features.Teams.Dtos;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.CreateTeam;

public record CreateTeamCommand(string Name, string? TeamPictureUrl) : IRequest<TeamDto>;