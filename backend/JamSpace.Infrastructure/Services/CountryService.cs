using System.Text.Json;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace JamSpace.Infrastructure.Services;

public class CountryService : ICountryService
{
    private readonly IMemoryCache _cache;
    private readonly string _relativePath;
    private const string CacheKey = "countries_en";

    public CountryService(IMemoryCache cache, IConfiguration cfg)
    {
        _cache = cache;
        _relativePath = cfg["CountryData:Path"] ?? "Resources/countries.en.json";
    }

    public IReadOnlyList<CountryDto> GetCountriesEn(bool refresh = false)
    {
        if (refresh) 
            _cache.Remove(CacheKey);

        if (_cache.TryGetValue(CacheKey, out IReadOnlyList<CountryDto> cached))
            return cached;

        var path = Path.Combine(AppContext.BaseDirectory, _relativePath);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Countries file not found at: {path}");

        var json = File.ReadAllText(path);
        var opts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        var raw = JsonSerializer.Deserialize<List<CountryDto?>>(json, opts) ?? new();

        var filtered = raw
            .Where(i => i is not null
                        && !string.IsNullOrWhiteSpace(i!.Code)
                        && !string.IsNullOrWhiteSpace(i!.Name))
            .Select(i => i! with
            {
                Code = i.Code!.Trim().ToUpperInvariant(),
                Name = i.Name!.Trim()
            })
            .Where(i => i.Code is not ("AN" or "CS" or "AP")) 
            .GroupBy(i => i.Code)
            .Select(g => g.First())
            .OrderBy(i => i.Name, StringComparer.Ordinal)
            .ToArray();

        _cache.Set(CacheKey, filtered, TimeSpan.FromHours(24));
        return filtered;
    }

    public CountryDto? GetCountryEn(string code)
        => GetCountriesEn().FirstOrDefault(c => c.Code == code.ToUpperInvariant());
}
