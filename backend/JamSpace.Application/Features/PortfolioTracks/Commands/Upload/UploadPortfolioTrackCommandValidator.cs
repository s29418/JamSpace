using FluentValidation;
using JamSpace.Application.Common.Validation;

namespace JamSpace.Application.Features.PortfolioTracks.Commands.Upload;

public class UploadPortfolioTrackCommandValidator : AbstractValidator<UploadPortfolioTrackCommand>
{
    public UploadPortfolioTrackCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.ArtistName)
            .MaximumLength(255)
            .When(x => x.ArtistName is not null);

        RuleFor(x => x.AlbumTitle)
            .MaximumLength(255)
            .When(x => x.AlbumTitle is not null);

        RuleFor(x => x.DurationMs)
            .GreaterThan(0)
            .When(x => x.DurationMs.HasValue);

        RuleFor(x => x.File)
            .MustBeValidAudio();
    }
}
