using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Users.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Users.Queries.SearchUsers;

public sealed class SearchUsersHandler : IRequestHandler<SearchUsersQuery, PagedResult<UserCardDto>>
{
    private readonly IUserSearchRepository _repo;
    private readonly ICountryCodeResolver _countryResolver;

    public SearchUsersHandler(IUserSearchRepository repo, ICountryCodeResolver countryResolver)
    {
        _repo = repo;
        _countryResolver = countryResolver;
    }

    public async Task<PagedResult<UserCardDto>> Handle(SearchUsersQuery request, CancellationToken ct)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        
        var pageSize = request.PageSize switch
        {
            < 1 => 10,
            > 50 => 50,
            _ => request.PageSize
        };

        var q = Normalize(request.Q);

        var requiredSkills = NormalizeList(request.Skills);
        var requiredGenres = NormalizeList(request.Genres);

        var locationFilter = ParseLocation(request.Location);

        return await _repo.SearchAsync(
            usernameQuery: q,
            location: locationFilter,
            requiredSkills: requiredSkills,
            requiredGenres: requiredGenres,
            page: page,
            pageSize: pageSize,
            currentUserId: request.CurrentUserId,
            ct: ct);
    }

    private static string? Normalize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        return input.Trim();
    }

    private static IReadOnlyList<string> NormalizeList(IReadOnlyList<string>? list)
    {
        if (list is null || list.Count == 0) return Array.Empty<string>();

        return list
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private LocationFilter? ParseLocation(string? locationRaw)
    {
        if (string.IsNullOrWhiteSpace(locationRaw)) return null;

        var raw = locationRaw.Trim();
        
        var parts = raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length >= 2)
        {
            var city = parts[0];
            var countryInput = parts[1];

            var countryCode = _countryResolver.ResolveCountryCode(countryInput) ?? countryInput.ToUpperInvariant();

            return new LocationFilter(city, countryCode);
        }
        
        var maybeCountryCode = _countryResolver.ResolveCountryCode(raw);
        if (maybeCountryCode is not null)
        {
            return new LocationFilter(null, maybeCountryCode);
        }

        return new LocationFilter(raw, null);
    }
}
