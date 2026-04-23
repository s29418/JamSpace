using FluentValidation;

namespace JamSpace.Application.Features.TeamEvents.Commands.Edit;

public class EditTeamEventCommandValidator : AbstractValidator<EditTeamEventCommand>
{
    public EditTeamEventCommandValidator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(2)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Title))
            .WithMessage("Title must be between 2 and 50 characters long.");

        RuleFor(x => x.Description)
            .MaximumLength(150)
            .WithMessage("Description cannot exceed 150 characters.");

        RuleFor(x => x.DurationMinutes)
            .GreaterThanOrEqualTo(5)
            .When(x => x.DurationMinutes is not null)
            .WithMessage("Duration of an event must be at least 5 minutes long.");

        RuleFor(x => x.DurationMinutes)
            .LessThanOrEqualTo(1440)
            .When(x => x.DurationMinutes is not null)
            .WithMessage("Duration of an event cannot exceed 24 hours.");

        RuleFor(x => x.StartDateTime)
            .GreaterThan(DateTimeOffset.UtcNow)
            .When(x => x.StartDateTime is not null)
            .WithMessage("Event must take place in the future.");
    }
}
