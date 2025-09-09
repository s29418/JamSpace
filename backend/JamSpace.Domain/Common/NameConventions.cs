namespace JamSpace.Domain.Common;

public static class NameConventions
{
    public static string NormalizeForQuery(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var trimmed = input.Trim();
        var compact = trimmed
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty)
            .Replace("_", string.Empty);

        return compact.ToLowerInvariant();
    }

    public static string PrettifyForDisplay(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var compact = string.Join(" ", input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        var words = compact.Split(' ');

        for (int i = 0; i < words.Length; i++)
        {
            var w = words[i];
            if (w.Length == 0) continue;
            var first = char.ToUpper(w[0]);
            var rest = w.Length > 1 ? w[1..].ToLower() : string.Empty;
            words[i] = first + rest;
        }

        return string.Join(' ', words);
    }
}