using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Users.Dtos;
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
        
        if (!string.IsNullOrWhiteSpace(usernameQuery))
        {
            var q = usernameQuery.Trim().ToLowerInvariant();
            query = query.Where(u => u.UserName.ToLower().Contains(q));
        }
        
        if (location is not null)
        {
            var city = location.City?.Trim();
            var countryCode = location.CountryCode?.Trim().ToUpperInvariant();
            
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
        
        if (requiredSkills.Count > 0)
        {
            var skillsNorm = requiredSkills
                .Select(s => s.Trim().ToLowerInvariant())
                .Distinct()
                .ToArray();

            query = query.Where(u =>
                u.UserSkills
                    .Select(us => us.Skill.Name.ToLower())
                    .Distinct()
                    .Count(n => skillsNorm.Contains(n)) == skillsNorm.Length
            );
        }
        
        if (requiredGenres.Count > 0)
        {
            var genresNorm = requiredGenres
                .Select(g => g.Trim().ToLowerInvariant())
                .Distinct()
                .ToArray();

            query = query.Where(u =>
                u.UserGenres
                    .Select(ug => ug.Genre.Name.ToLower())
                    .Distinct()
                    .Count(n => genresNorm.Contains(n)) == genresNorm.Length
            );
        }
        
        var totalItems = await query.CountAsync(ct);
        
        var skip = (page - 1) * pageSize;

        var items = await query
            .OrderBy(u => u.UserName) 
            .Skip(skip)
            .Take(pageSize)
            .Select(u => new UserCardDto
            {
                Id = u.Id,
                Username = u.UserName,
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
                
                FollowersCount = _db.UserFollows
                    .Count(f => f.FolloweeId == u.Id),

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
