namespace JamSpace.Application.Features.Teams.Create;

using FluentValidation;

public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name of the team cannot be empty.")
            .MinimumLength(4).WithMessage("Name of the team must be at least 4 characters long.");
    }
}
