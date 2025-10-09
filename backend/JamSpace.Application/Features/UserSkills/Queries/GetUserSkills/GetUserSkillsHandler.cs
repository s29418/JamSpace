using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserSkills.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserSkills.Queries.GetUserSkills;

public class GetUserSkillsHandler : IRequestHandler<GetUserSkillsQuery, List<UserSkillDto>>
{
    private readonly IUserSkillRepository _repo;
    
    public GetUserSkillsHandler(IUserSkillRepository repo)
    {
        _repo = repo;
    }
    
    public async Task<List<UserSkillDto>> Handle(GetUserSkillsQuery request, CancellationToken cancellationToken)
    {
        var userSkills = await _repo.GetAllUserSkillsAsync(request.UserId, cancellationToken);

        return userSkills
            .Select(us => new UserSkillDto
            {
                SkillId = us.SkillId,
                SkillName = us.Skill.Name
            })
            .ToList();
    }
}