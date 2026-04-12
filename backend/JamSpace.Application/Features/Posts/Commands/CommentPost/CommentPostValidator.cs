using FluentValidation;

namespace JamSpace.Application.Features.Posts.Commands.CommentPost;

public class CommentPostValidator : AbstractValidator<CommentPostCommand>
{
    public CommentPostValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .Must(c => !string.IsNullOrWhiteSpace(c))
            .WithMessage("Comment must have content.");

        RuleFor(x => x.Content)
            .MaximumLength(500)
            .WithMessage("Comment must not exceed 500 characters.");

        RuleFor(x => x.Content)
            .MinimumLength(2)
            .WithMessage("Comment must have at least 2 characters.");
    }
}