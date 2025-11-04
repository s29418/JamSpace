using FluentValidation;

namespace JamSpace.Application.Features.Users.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.Bio)
            .MaximumLength(170)
            .WithMessage("Bio cannot be longer than 170 characters.")
            .When(x => x.SetBio);

        RuleFor(x => x.DisplayName)
            .MaximumLength(50).WithMessage("Display name cannot be longer than 50 characters.")
            .NotEmpty().WithMessage("Display name cannot be empty.")
            .MinimumLength(3).WithMessage("Display name must be at least 3 characters long.")
            .When(x => x.SetDisplayName); 

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email is not valid.")
            .When(x => x.SetEmail);

        // When(x => x.SetLocation && x.Location != null, () =>
        // {
        //     RuleFor(x => x.Location!.City)
        //         .MaximumLength(50)
        //         .WithMessage("City cannot be longer than 50 characters.")
        //         .MinimumLength(3)
        //         .WithMessage("City must be at least 3 characters long.");
        //     
        //     // RuleFor(x => x.Location.Country)
        //     //     .MaximumLength(50).WithMessage("Country cannot be longer than 50 characters.")
        //     //     .NotEmpty().WithMessage("Country cannot be empty.")
        //     //     .MinimumLength(3).WithMessage("Country must be at least 3 characters long.");
        // });
    }
}