using MediatR;
using JamSpace.Application.Features.Teams.Dtos;

namespace JamSpace.Application.Features.Teams.Create;

public record CreateTeamWithUserCommand(CreateTeamCommand Command, Guid CreatorUserId) : IRequest<TeamDto>;
