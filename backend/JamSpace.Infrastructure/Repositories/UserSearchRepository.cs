using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Users.DTOs;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class UserSearchRepository : IUserSearchRepository
{
    private readonly JamSpaceDbContext _db;

    public UserSearchRepository(JamSpaceDbContext db)
    {
        _db = db;
    }
    
    public async Task<PagedResult<UserCardDto>> SearchAsync(
    string? usernameQuery,
    LocationFilter? location,
    IReadOnlyList<string> requiredSkills,
    IReadOnlyList<string> requiredGenres,
    Guid? currentUserId,
    int page,
    int pageSize,
    CancellationToken ct)
    {
        var query = _db.Users
            .AsNoTracking()
            .AsQueryable();

        var normalizedQuery = usernameQuery?.Trim().ToLowerInvariant();

        // username & displayName
        if (!string.IsNullOrWhiteSpace(normalizedQuery))
        {
            query = query.Where(u =>
                u.UserName.ToLower().Contains(normalizedQuery) ||
                (u.DisplayName.ToLower().Contains(normalizedQuery)));
        }

        // location
        if (location is not null)
        {
            var city = location.City;
            var countryCode = location.CountryCode;

            if (!string.IsNullOrWhiteSpace(city) && !string.IsNullOrWhiteSpace(countryCode))
            {
                var cityNorm = city.ToLowerInvariant();
                query = query.Where(u =>
                    u.Location != null &&
                    u.Location.City != null &&
                    u.Location.City.ToLower().Contains(cityNorm) &&
                    u.Location.CountryCode == countryCode);
            }
            else if (string.IsNullOrWhiteSpace(city) && !string.IsNullOrWhiteSpace(countryCode))
            {
                query = query.Where(u =>
                    u.Location != null &&
                    u.Location.CountryCode == countryCode);
            }
            else if (!string.IsNullOrWhiteSpace(city) && string.IsNullOrWhiteSpace(countryCode))
            {
                var cityNorm = city.ToLowerInvariant();
                query = query.Where(u =>
                    u.Location != null &&
                    u.Location.City != null &&
                    u.Location.City.ToLower().Contains(cityNorm));
            }
        }

        // skills (ALL required)
        if (requiredSkills.Count > 0)
        {
            var skillsNorm = requiredSkills
                .Select(s => s.ToLowerInvariant())
                .ToArray();

            query = query.Where(u =>
                skillsNorm.All(sn =>
                    u.UserSkills.Any(us => us.Skill.Name.ToLower() == sn)));
        }

        // genres (ALL required)
        if (requiredGenres.Count > 0)
        {
            var genresNorm = requiredGenres
                .Select(g => g.ToLowerInvariant())
                .ToArray();

            query = query.Where(u =>
                genresNorm.All(gn =>
                    u.UserGenres.Any(ug => ug.Genre.Name.ToLower() == gn)));
        }

        var totalItems = await query.CountAsync(ct);

        var skip = (page - 1) * pageSize;

        if (!string.IsNullOrWhiteSpace(normalizedQuery))
        {
            query = query
                .OrderBy(u =>
                    u.UserName.ToLower() == normalizedQuery ? 0 :
                    u.DisplayName.ToLower() == normalizedQuery ? 1 :
                    u.UserName.ToLower().StartsWith(normalizedQuery) ? 2 :
                    u.DisplayName.ToLower().StartsWith(normalizedQuery) ? 3 :
                    4)
                .ThenByDescending(u => u.Followers.Count)
                .ThenBy(u => u.DisplayName)
                .ThenBy(u => u.UserName);
        }
        else
        {
            query = query
                .OrderByDescending(u => u.Followers.Count)
                .ThenBy(u => u.DisplayName)
                .ThenBy(u => u.UserName);
        }

        var items = await query
            .Skip(skip)
            .Take(pageSize)
            .Select(u => new UserCardDto
            {
                Id = u.Id,
                Username = u.UserName,
                DisplayName = u.DisplayName,
                ProfilePictureUrl = u.ProfilePictureUrl,

                City = u.Location != null ? u.Location.City : null,
                CountryCode = u.Location != null ? u.Location.CountryCode : null,

                Skills = u.UserSkills
                    .Select(us => us.Skill.Name)
                    .OrderBy(n => n)
                    .ToArray(),

                Genres = u.UserGenres
                    .Select(ug => ug.Genre.Name)
                    .OrderBy(n => n)
                    .ToArray(),

                FollowersCount = _db.UserFollows.Count(f => f.FolloweeId == u.Id),

                IsFollowedByMe =
                    currentUserId != null &&
                    u.Id != currentUserId &&
                    _db.UserFollows.Any(f =>
                        f.FollowerId == currentUserId &&
                        f.FolloweeId == u.Id),

                IsMe = currentUserId != null && u.Id == currentUserId
            })
            .ToListAsync(ct);

        return PagedResult<UserCardDto>.Create(items, page, pageSize, totalItems);
    }
}