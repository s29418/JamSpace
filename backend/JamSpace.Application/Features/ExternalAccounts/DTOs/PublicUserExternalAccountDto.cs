namespace JamSpace.Application.Features.ExternalAccounts.DTOs;

public sealed record PublicUserExternalAccountDto(
    string Provider,
    string DisplayName,
    string ProfileUrl,
    string? AvatarUrl
);
