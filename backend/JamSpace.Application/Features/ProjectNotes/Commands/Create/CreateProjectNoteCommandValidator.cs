using FluentValidation;

namespace JamSpace.Application.Features.ProjectNotes.Commands.Create;

public class CreateProjectNoteCommandValidator : AbstractValidator<CreateProjectNoteCommand>
{
    public CreateProjectNoteCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(750)
            .WithMessage("Note content cannot be longer than 750 characters.");

        RuleFor(x => x.StartTimeSeconds)
            .GreaterThanOrEqualTo(0)
            .When(x => x.StartTimeSeconds.HasValue);

        RuleFor(x => x.EndTimeSeconds)
            .GreaterThanOrEqualTo(0)
            .When(x => x.EndTimeSeconds.HasValue);

        RuleFor(x => x)
            .Must(x => !x.EndTimeSeconds.HasValue || x.StartTimeSeconds.HasValue)
            .WithMessage("Start time is required when end time is provided.");

        RuleFor(x => x)
            .Must(x => !x.StartTimeSeconds.HasValue || !x.EndTimeSeconds.HasValue || x.EndTimeSeconds >= x.StartTimeSeconds)
            .WithMessage("End time must be greater than or equal to start time.");

        RuleFor(x => x)
            .Must(x => !x.StartTimeSeconds.HasValue && !x.EndTimeSeconds.HasValue || x.AudioVersionId.HasValue)
            .WithMessage("Audio version is required for timestamped notes.");
    }
}
