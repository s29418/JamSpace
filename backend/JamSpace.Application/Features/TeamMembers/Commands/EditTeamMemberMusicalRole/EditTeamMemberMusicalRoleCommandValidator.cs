using FluentValidation;

namespace JamSpace.Application.Features.TeamMembers.Commands.EditTeamMemberMusicalRole;

public class EditTeamMemberMusicalRoleCommandValidator : AbstractValidator<EditTeamMemberMusicalRoleCommand>
{
    public EditTeamMemberMusicalRoleCommandValidator()
    {
        RuleFor(x => x.MusicalRole)
            .NotEmpty().WithMessage("Musical role cannot be empty.")
            .MinimumLength(3).WithMessage("Musical role must be at least 3 characters long.")
            .MaximumLength(25).WithMessage("Musical role cannot be longer than 25 characters.");
    }
    
}