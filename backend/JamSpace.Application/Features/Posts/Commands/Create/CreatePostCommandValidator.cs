using FluentValidation;
using JamSpace.Application.Common.Validation;

namespace JamSpace.Application.Features.Posts.Commands.Create;

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Content)
                       || x.File is not null
                       || x.PortfolioTrackId.HasValue
                       || !string.IsNullOrWhiteSpace(x.SpotifyPlaylistExternalUrl))
            .WithMessage("Post must contain text, a media file, a portfolio track or a Spotify playlist.");

        RuleFor(x => x)
            .Must(x => CountAttachments(x) <= 1)
            .WithMessage("Post can contain only one attachment: a media file, a portfolio track or a Spotify playlist.");

        RuleFor(x => x.Content)
            .MaximumLength(1000)
            .When(x => x.Content is not null);

        RuleFor(x => x.SpotifyPlaylistTitle)
            .NotEmpty()
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.SpotifyPlaylistExternalUrl));

        RuleFor(x => x.SpotifyPlaylistExternalUrl)
            .MaximumLength(500)
            .Must(url => SpotifyPlaylistLink.TryBuildEmbedUrl(url, out _))
            .WithMessage("Paste a valid Spotify playlist link.")
            .When(x => !string.IsNullOrWhiteSpace(x.SpotifyPlaylistExternalUrl));

        RuleFor(x => x.File)
            .MustBeValidPostMedia();
    }

    private static int CountAttachments(CreatePostCommand command)
    {
        var count = 0;

        if (command.File is not null) count++;
        if (command.PortfolioTrackId.HasValue) count++;
        if (!string.IsNullOrWhiteSpace(command.SpotifyPlaylistExternalUrl)) count++;

        return count;
    }
}
