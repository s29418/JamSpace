using FluentValidation;
using JamSpace.Application.Common.Validation;

namespace JamSpace.Application.Features.Posts.Commands.Create;

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Content) || x.File is not null || x.PortfolioTrackId.HasValue)
            .WithMessage("Post must contain text, a media file or a portfolio track.");

        RuleFor(x => x)
            .Must(x => x.File is null || !x.PortfolioTrackId.HasValue)
            .WithMessage("Post cannot contain both a media file and a portfolio track.");

        RuleFor(x => x.Content)
            .MaximumLength(1000)
            .When(x => x.Content is not null);

        RuleFor(x => x.File)
            .MustBeValidPostMedia();
    }
    
}
