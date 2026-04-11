using FluentValidation;
using JamSpace.Application.Common.Models;

namespace JamSpace.Application.Common.Validation;

public static class MediaValidationRules
{
    public const long MaxImageSizeBytes = 2 * 1024 * 1024;   // 2 MB
    public const long MaxAudioSizeBytes = 20 * 1024 * 1024;  // 20 MB
    public const long MaxVideoSizeBytes = 100 * 1024 * 1024; // 100 MB

    private static readonly string[] AllowedImageContentTypes =
    [
        "image/jpeg",
        "image/png"
    ];

    private static readonly string[] AllowedAudioContentTypes =
    [
        "audio/mpeg",
        "audio/wav",
        "audio/ogg"
    ];

    private static readonly string[] AllowedVideoContentTypes =
    [
        "video/mp4",
        "video/webm"
    ];

    public static MediaCategory? ResolveCategory(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return null;

        var normalized = contentType.Trim().ToLowerInvariant();

        if (AllowedImageContentTypes.Contains(normalized))
            return MediaCategory.Image;

        if (AllowedAudioContentTypes.Contains(normalized))
            return MediaCategory.Audio;

        if (AllowedVideoContentTypes.Contains(normalized))
            return MediaCategory.Video;

        return null;
    }

    public static bool IsAllowedImage(FileUpload file)
    {
        return ResolveCategory(file.ContentType) == MediaCategory.Image;
    }

    public static bool HasValidImageSize(FileUpload file)
    {
        return file.Length <= MaxImageSizeBytes;
    }

    public static bool IsAllowedPostMedia(FileUpload file)
    {
        return ResolveCategory(file.ContentType) is not null;
    }

    public static bool HasValidSizeForResolvedCategory(FileUpload file)
    {
        var category = ResolveCategory(file.ContentType);

        return category switch
        {
            MediaCategory.Image => file.Length <= MaxImageSizeBytes,
            MediaCategory.Audio => file.Length <= MaxAudioSizeBytes,
            MediaCategory.Video => file.Length <= MaxVideoSizeBytes,
            _ => false
        };
    }

    public static IRuleBuilderOptions<T, FileUpload> MustBeValidImage<T>(
        this IRuleBuilder<T, FileUpload> ruleBuilder)
    {
        return ruleBuilder
            .NotNull().WithMessage("File is required.")
            .Must(file => file.Length > 0).WithMessage("No file provided.")
            .Must(file => !string.IsNullOrWhiteSpace(file.FileName)).WithMessage("File name is required.")
            .Must(IsAllowedImage).WithMessage("Only JPEG or PNG images are allowed.")
            .Must(HasValidImageSize).WithMessage("Image file size must not exceed 2 MB.");
    }

    public static IRuleBuilderOptions<T, FileUpload?> MustBeValidPostMedia<T>(
        this IRuleBuilder<T, FileUpload?> ruleBuilder)
    {
        return ruleBuilder
            .Must(file => file is null || file.Length > 0)
            .WithMessage("Uploaded file cannot be empty.")
            .Must(file => file is null || !string.IsNullOrWhiteSpace(file.FileName))
            .WithMessage("File name is required.")
            .Must(file => file is null || IsAllowedPostMedia(file))
            .WithMessage("Only JPEG, PNG, MP3, WAV, OGG, MP4 or WebM files are allowed.")
            .Must(file => file is null || HasValidSizeForResolvedCategory(file))
            .WithMessage((_, file) => file switch
            {
                null => "Invalid file.",
                _ when ResolveCategory(file.ContentType) == MediaCategory.Image => "Image file size must not exceed 2 MB.",
                _ when ResolveCategory(file.ContentType) == MediaCategory.Audio => "Audio file size must not exceed 20 MB.",
                _ when ResolveCategory(file.ContentType) == MediaCategory.Video => "Video file size must not exceed 100 MB.",
                _ => "Unsupported media type."
            });
    }
}