using FluentValidation;

namespace JamSpace.Application.Features.UserSkills.Commands.AddUserSkill;

public class AddUserSkillCommandValidator : AbstractValidator<AddUserSkillCommand>
{
    public AddUserSkillCommandValidator()
    {
        RuleFor(x => x.SkillName)
            .NotEmpty().WithMessage("Skill name cannot be empty.")
            .MinimumLength(3).WithMessage("Skill name must be at least 3 characters long.")
            .MaximumLength(20).WithMessage("Skill name cannot be longer than 20 characters.");
    }
}