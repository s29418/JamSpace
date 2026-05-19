using FluentValidation;
using JamSpace.Application.Common.Validation;

namespace JamSpace.Application.Features.ProjectAudioVersions.Commands.Upload;

public class UploadProjectAudioVersionCommandValidator : AbstractValidator<UploadProjectAudioVersionCommand>
{
    public UploadProjectAudioVersionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.DurationSeconds)
            .GreaterThan(0)
            .When(x => x.DurationSeconds.HasValue);

        RuleFor(x => x.File)
            .MustBeValidAudio();
    }
}
