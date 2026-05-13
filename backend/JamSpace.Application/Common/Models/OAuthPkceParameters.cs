namespace JamSpace.Application.Common.Models;

public sealed record OAuthPkceParameters(
    string State,
    string CodeVerifier,
    string CodeChallenge
);
