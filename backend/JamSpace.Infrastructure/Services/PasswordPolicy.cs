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

    public (bool IsValid, Dictionary<string, string> Errors) Validate(string passwordPlain)
    {
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrEmpty(passwordPlain))
            errors["Password"] = "Password cannot be empty.";

        if (_opts.DisallowWhitespace && HasWhitespace.IsMatch(passwordPlain))
            errors["Password"] = "Password must not contain whitespace.";

        if (passwordPlain.Length < _opts.MinimumLength)
            errors["Password"] = $"Password must be at least {_opts.MinimumLength} characters long.";

        var groups = 0;
        var upper = HasUpper.IsMatch(passwordPlain); if (upper) groups++;
        var lower = HasLower.IsMatch(passwordPlain); if (lower) groups++;
        var digit = HasDigit.IsMatch(passwordPlain); if (digit) groups++;
        var symbol = HasSymbol.IsMatch(passwordPlain); if (symbol) groups++;

        if (_opts.RequireUppercase && !upper)
            errors["Password"] = "Password must contain an uppercase letter.";
        if (_opts.RequireLowercase && !lower)
            errors["Password"] = "Password must contain a lowercase letter.";
        if (_opts.RequireDigit && !digit)
            errors["Password"] = "Password must contain a digit.";
        if (_opts.RequireNonAlphanumeric && !symbol)
            errors["Password"] = "Password must contain a non-alphanumeric character.";

        return (errors.Count == 0, errors);
    }
}
