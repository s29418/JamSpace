using JamSpace.Application.Common.Interfaces;

namespace JamSpace.Infrastructure.Services;

public sealed class CountryCodeResolver : ICountryCodeResolver
{
    private readonly ICountryService _countryService;

    public CountryCodeResolver(ICountryService countryService)
    {
        _countryService = countryService;
    }

    public string? ResolveCountryCode(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var trimmed = input.Trim();
        
        if (trimmed.Length == 2)
        {
            var byCode = _countryService.GetCountryEn(trimmed);
            return byCode?.Code;
        }
        
        var byName = _countryService.GetCountryByNameEn(trimmed);
        return byName?.Code;
    }
}