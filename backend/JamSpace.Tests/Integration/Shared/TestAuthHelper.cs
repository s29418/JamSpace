using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace JamSpace.Tests.Integration.Shared;

public static class TestAuthHelper
{
    public static async Task<string> CreateUserAndGenerateJwtAsync(
        HttpClient client,
        IServiceProvider services,
        string userName,
        string role = "User")
    {
        var db = services.GetRequiredService<JamSpaceDbContext>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Email = $"{userName}@test.com",
            PasswordHash = "fakefakefake"
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return GenerateJwt(user, services);
    }

    private static string GenerateJwt(User user, IServiceProvider services)
    {
        var config = services.GetRequiredService<IConfiguration>();
        var jwtKey = config["Jwt:Key"]!;
        var jwtIssuer = config["Jwt:Issuer"]!;
        var jwtAudience = config["Jwt:Audience"]!;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = jwtIssuer,
            Audience = jwtAudience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}