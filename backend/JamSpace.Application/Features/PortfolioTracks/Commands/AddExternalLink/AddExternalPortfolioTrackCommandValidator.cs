using FluentValidation;
using JamSpace.Domain.Enums;

namespace JamSpace.Application.Features.PortfolioTracks.Commands.AddExternalLink;

public class AddExternalPortfolioTrackCommandValidator : AbstractValidator<AddExternalPortfolioTrackCommand>
{
    public AddExternalPortfolioTrackCommandValidator()
    {
        RuleFor(x => x.Source)
            .Must(x => x is PortfolioTrackSource.Spotify or PortfolioTrackSource.SoundCloud)
            .WithMessage("External portfolio track source must be Spotify or SoundCloud.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.ArtistName)
            .MaximumLength(255)
            .When(x => x.ArtistName is not null);

        RuleFor(x => x.AlbumTitle)
            .MaximumLength(255)
            .When(x => x.AlbumTitle is not null);

        RuleFor(x => x.ArtworkUrl)
            .MaximumLength(1000)
            .Must(BeAbsoluteUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.ArtworkUrl))
            .WithMessage("Artwork URL must be an absolute URL.");

        RuleFor(x => x.DurationMs)
            .GreaterThan(0)
            .When(x => x.DurationMs.HasValue);

        RuleFor(x => x.ExternalTrackId)
            .MaximumLength(255)
            .When(x => x.ExternalTrackId is not null);

        RuleFor(x => x.ExternalUrl)
            .NotEmpty()
            .MaximumLength(1000)
            .Must(BeAbsoluteUrl)
            .WithMessage("External URL must be an absolute URL.");

        RuleFor(x => x.EmbedUrl)
            .MaximumLength(1000)
            .Must(BeAbsoluteUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.EmbedUrl))
            .WithMessage("Embed URL must be an absolute URL.");
    }

    private static bool BeAbsoluteUrl(string? value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
               && uri.Scheme is "http" or "https";
    }
}
