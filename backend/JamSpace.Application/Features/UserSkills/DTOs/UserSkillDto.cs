namespace JamSpace.Application.Features.UserSkills.DTOs;

public class UserSkillDto
{
    public Guid SkillId { get; set; }
    public string SkillName { get; set; } = default!;

    public UserSkillDto(Guid id, string name)
    {
        SkillId = id;
        SkillName = name;
    }

    public UserSkillDto()
    {
    }
}