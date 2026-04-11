using FluentValidation;
using JamSpace.Application.Common.Validation;

namespace JamSpace.Application.Features.Posts.Commands.CreatePost;

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Content) || x.File is not null)
            .WithMessage("Post must contain text or a media file.");

        RuleFor(x => x.Content)
            .MaximumLength(1000)
            .When(x => x.Content is not null);

        RuleFor(x => x.File)
            .MustBeValidPostMedia();
    }
    
}