using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Application.Features.TeamInvites.Mappers;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Queries.GetTeamInviteById;

public class GetTeamInviteByIdHadnler : IRequestHandler<GetTeamInviteByIdQuery, TeamInviteDto>
{

    private readonly ITeamInviteRepository _repo;
    
    public GetTeamInviteByIdHadnler(ITeamInviteRepository repo)
    {
        this._repo = repo;
    }

    public async Task<TeamInviteDto> Handle(GetTeamInviteByIdQuery request, CancellationToken cancellationToken)
    {
        return TeamInviteMapper.ToDto(await _repo.GetByIdAsync(request.Id, cancellationToken) 
        ??  throw new NotFoundException("Invite not found"));
    }
}