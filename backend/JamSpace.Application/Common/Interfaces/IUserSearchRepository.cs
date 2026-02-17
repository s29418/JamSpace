using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Users.Dtos;

namespace JamSpace.Application.Common.Interfaces;

public interface IUserSearchRepository
{
    Task<PagedResult<UserCardDto>> SearchAsync(
        string? usernameQuery,
        LocationFilter? location,
        IReadOnlyList<string> requiredSkills,
        IReadOnlyList<string> requiredGenres,
        Guid? currentUserId,
        int page,
        int pageSize,
        CancellationToken ct);
}

public sealed record LocationFilter(string? City, string? CountryCode);