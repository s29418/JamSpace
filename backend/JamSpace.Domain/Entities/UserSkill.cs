namespace JamSpace.Domain.Entities;

public class UserSkill
{
    public Guid SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}