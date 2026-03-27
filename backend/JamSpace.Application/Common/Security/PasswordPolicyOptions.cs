namespace JamSpace.Application.Common.Security;

public sealed class PasswordPolicyOptions
{
    public int MinimumLength { get; init; } = 8;
    public bool RequireUppercase { get; init; } = true;
    public bool RequireLowercase { get; init; } = true;
    public bool RequireDigit { get; init; } = true;
    public bool RequireNonAlphanumeric { get; init; } = false;
    public bool DisallowWhitespace { get; init; } = true;
}