using System.Text.RegularExpressions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Security;
using Microsoft.Extensions.Options;

namespace JamSpace.Infrastructure.Services;

public sealed class PasswordPolicy : IPasswordPolicy
{
    private readonly PasswordPolicyOptions _opts;
    
    private static readonly Regex HasUpper = new("[A-Z]", RegexOptions.Compiled);
    private static readonly Regex HasLower = new("[a-z]", RegexOptions.Compiled);
    private static readonly Regex HasDigit = new("\\d", RegexOptions.Compiled);
    private static readonly Regex HasSymbol = new("[^0-9A-Za-z]", RegexOptions.Compiled);
    private static readonly Regex HasWhitespace = new("\\s", RegexOptions.Compiled);

    public PasswordPolicy(IOptions<PasswordPolicyOptions> options)
    {
        _opts = options.Value;
    }

    public (bool IsValid, string? Error) Validate(string passwordPlain)
    {
        if (string.IsNullOrEmpty(passwordPlain))
            return (false, "Password cannot be empty.");

        if (_opts.DisallowWhitespace && HasWhitespace.IsMatch(passwordPlain))
            return (false, "Password must not contain whitespace.");

        if (passwordPlain.Length < _opts.MinimumLength)
            return (false, $"Password must be at least {_opts.MinimumLength} characters long.");

        var p = passwordPlain;
        var groups = 0;
        var upper = HasUpper.IsMatch(p); if (upper) groups++;
        var lower = HasLower.IsMatch(p); if (lower) groups++;
        var digit = HasDigit.IsMatch(p); if (digit) groups++;
        var symbol = HasSymbol.IsMatch(p); if (symbol) groups++;

        if (_opts.RequireUppercase && !upper)
            return (false, "Password must contain an uppercase letter.");
        if (_opts.RequireLowercase && !lower)
            return (false, "Password must contain a lowercase letter.");
        if (_opts.RequireDigit && !digit)
            return (false, "Password must contain a digit.");
        if (_opts.RequireNonAlphanumeric && !symbol)
            return (false, "Password must contain a non-alphanumeric character.");


        return (true, null);
    }
}
