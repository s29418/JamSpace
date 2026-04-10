using FluentValidation;
using JamSpace.Application.Common.Models;

namespace JamSpace.Application.Common.Validation;

public static class ImageValidationRules
{
    public const long MaxImageSizeBytes = 2 * 1024 * 1024; //2MB

    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png"
    ];

    public static IRuleBuilderOptions<T, FileUpload> MustBeValidImage<T>(this IRuleBuilder<T, FileUpload> ruleBuilder)
    {
        return ruleBuilder
            .NotNull().WithMessage("File is required.")
            .Must(file => file.Length > 0).WithMessage("No file provided.")
            .Must(file => !string.IsNullOrWhiteSpace(file.FileName)).WithMessage("File name is required.")
            .Must(file => file.ContentType is not null &&
                          AllowedContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Only JPEG or PNG images are allowed.")
            .Must(file => file.Length <= MaxImageSizeBytes)
            .WithMessage("File size must not exceed 2 MB.");
    }
    
}