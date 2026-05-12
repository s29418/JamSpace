namespace JamSpace.Application.Features.ExternalAccounts.DTOs;

public sealed record UserExternalAccountDto(
    Guid Id,
    string Provider,
    string ExternalUserId,
    string DisplayName,
    string ProfileUrl,
    string? AvatarUrl,
    DateTimeOffset ConnectedAt,
    DateTimeOffset UpdatedAt
);
