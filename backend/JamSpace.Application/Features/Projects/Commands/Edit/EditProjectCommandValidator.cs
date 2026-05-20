using FluentValidation;

namespace JamSpace.Application.Features.Projects.Commands.Edit;

public class EditProjectCommandValidator : AbstractValidator<EditProjectCommand>
{
    public EditProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(25);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);
    }
}
