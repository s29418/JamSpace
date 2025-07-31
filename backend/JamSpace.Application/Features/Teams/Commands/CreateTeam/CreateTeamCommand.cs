using JamSpace.Application.Common.Features.Teams.Dtos;
using MediatR;

namespace JamSpace.Application.Common.Features.Teams.Commands.Create;

public record CreateTeamCommand(string Name, string? TeamPictureUrl) : IRequest<TeamDto>;