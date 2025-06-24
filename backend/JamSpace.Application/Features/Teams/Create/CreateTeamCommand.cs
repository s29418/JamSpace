using JamSpace.Application.Features.Teams.Dtos;
using MediatR;

namespace JamSpace.Application.Features.Teams.Create;

public class CreateTeamCommand : IRequest<TeamDto>
{
    public string Name { get; init; } = default!;
}