using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserSkills.DTOs;
using MediatR;

namespace JamSpace.Application.Features.UserSkills.Commands.AddUserSkill;

public class AddUserSkillHandler : IRequestHandler<AddUserSkillCommand, UserSkillDto>
{
    private readonly IUserSkillRepository _repo;
    private readonly ISkillRepository _skillRepo;
    
    public AddUserSkillHandler(IUserSkillRepository repo, ISkillRepository skillRepo)
    {
        _repo = repo;
        _skillRepo = skillRepo;
    }
    
    public async Task<UserSkillDto> Handle(AddUserSkillCommand request, CancellationToken ct)
    {
        var skill = await _skillRepo.GetSkillByNameAsync(request.SkillName, ct) 
            ?? await _skillRepo.CreateSkillAsync(request.SkillName, ct);
        
        if(await _repo.UserHasSkillAsync(request.UserId, skill.Id, ct))
            throw new ConflictException("User already has this skill.");
        
        await _repo.AddUserSkillAsync(request.UserId, skill.Id, ct);
        return new UserSkillDto
        {
            SkillId = skill.Id,
            SkillName = skill.Name
        };
    }
}