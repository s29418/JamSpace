namespace JamSpace.Application.Features.ExternalAccounts.DTOs;

public sealed record CompleteExternalAccountConnectionResult(
    UserExternalAccountDto Account,
    string? ReturnUrl
);
