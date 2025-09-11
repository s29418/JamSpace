using FluentValidation;

namespace JamSpace.Application.Features.UserGenres.Commands.AddUserGenre;

public class AddUserGenreCommandValidator : AbstractValidator<AddUserGenreCommand>
{
    public AddUserGenreCommandValidator()
    {
        RuleFor(x => x.GenreName)
            .NotEmpty().WithMessage("Genre name cannot be empty.")
            .MinimumLength(3).WithMessage("Genre name must be at least 3 characters long.")
            .MaximumLength(20).WithMessage("Genre name cannot be longer than 20 characters.");
    }
}