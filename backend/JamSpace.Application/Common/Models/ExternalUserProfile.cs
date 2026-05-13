namespace JamSpace.Application.Common.Models;

public sealed record ExternalUserProfile(
    string ExternalUserId,
    string DisplayName,
    string ProfileUrl,
    string? AvatarUrl
);
