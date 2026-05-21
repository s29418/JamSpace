using FluentValidation;

namespace JamSpace.Application.Features.Projects.Commands.Create;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(25)
            .WithMessage("Project name cannot be longer than 25 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(200)
            .WithMessage("Project description cannot be longer than 200 characters.")
            .When(x => x.Description is not null);
    }
}
