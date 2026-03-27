using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.UserSkills.DTOs;
using JamSpace.Domain.Common;
using JamSpace.Domain.Entities;
using MediatR;

namespace JamSpace.Application.Features.UserSkills.Commands.AddUserSkill;

public class AddUserSkillHandler : IRequestHandler<AddUserSkillCommand, UserSkillDto>
{
    private readonly IUserSkillRepository _repo;
    private readonly ISkillRepository _skillRepo;
    private readonly IUnitOfWork _uow;

    public AddUserSkillHandler(IUserSkillRepository repo, ISkillRepository skillRepo, IUnitOfWork uow)
    {
        _repo = repo;
        _skillRepo = skillRepo;
        _uow = uow;
    }

    public async Task<UserSkillDto> Handle(AddUserSkillCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.SkillName))
            throw new ArgumentException("Skill name is required.", nameof(request.SkillName));

        var skill = await _skillRepo.GetSkillByNameAsync(request.SkillName, ct);

        if (skill is null)
        {
            skill = new Skill
            {
                Id = Guid.NewGuid(),
                Name = NameConventions.PrettifyForDisplay(request.SkillName)
            };

            await _skillRepo.AddAsync(skill, ct);
        }

        if (await _repo.UserHasSkillAsync(request.UserId, skill.Id, ct))
            throw new ConflictException("User already has this skill.");

        await _repo.AddAsync(new UserSkill
        {
            UserId = request.UserId,
            SkillId = skill.Id,
            AddeddAt = DateTime.UtcNow
        }, ct);

        await _uow.SaveChangesAsync(ct);

        return new UserSkillDto
        {
            SkillId = skill.Id,
            SkillName = skill.Name
        };
    }
}