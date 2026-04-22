using FluentValidation;

namespace JamSpace.Application.Features.TeamEvents.Commands.Create;

public class CreateTeamEventCommandValidator : AbstractValidator<CreateTeamEventCommand>
{
    public CreateTeamEventCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50)
            .WithMessage("Title must be between 2 and 50 characters long.");

        RuleFor(x => x.Description)
            .MaximumLength(150)
            .WithMessage("Description cannot exceed 150 characters.");

        RuleFor(x => x.DurationMinutes)
            .GreaterThanOrEqualTo(5)
            .WithMessage("Duration of an event must be at least 5 minutes long.");

        RuleFor(x => x.StartDateTime)
            .GreaterThan(DateTimeOffset.UtcNow)
            .WithMessage("Event must take place in the future.");
    }
}