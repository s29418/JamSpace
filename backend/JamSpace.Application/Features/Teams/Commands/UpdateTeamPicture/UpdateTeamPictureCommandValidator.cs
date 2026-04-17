using FluentValidation;
using JamSpace.Application.Common.Validation;

namespace JamSpace.Application.Features.Teams.Commands.UpdateTeamPicture;

public class UpdateTeamPictureCommandValidator : AbstractValidator<UpdateTeamPictureCommand>
{
    public UpdateTeamPictureCommandValidator()
    {
        RuleFor(x => x.File).MustBeValidImage();
    }
}