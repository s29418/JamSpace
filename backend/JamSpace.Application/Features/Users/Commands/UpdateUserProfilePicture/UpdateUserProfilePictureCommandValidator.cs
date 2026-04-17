using FluentValidation;
using JamSpace.Application.Common.Validation;

namespace JamSpace.Application.Features.Users.Commands.UpdateUserProfilePicture;

public class UpdateUserProfilePictureCommandValidator : AbstractValidator<UpdateUserProfilePictureCommand>
{
    public UpdateUserProfilePictureCommandValidator()
    {
        RuleFor(x => x.File).MustBeValidImage();
    }
}