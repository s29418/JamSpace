using Azure.Storage.Blobs;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Infrastructure.Data;
using JamSpace.Infrastructure.Repositories;
using JamSpace.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JamSpace.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<JamSpaceDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));
        
        services.AddMemoryCache();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserSearchRepository, UserSearchRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
        services.AddScoped<ITeamInviteRepository, TeamInviteRepository>();
        
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IUserGenreRepository, UserGenreRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<IUserSkillRepository, UserSkillRepository>();
        services.AddScoped<IUserFollowRepository, UserFollowRepository>();

        services.AddSingleton(sp =>
        {
            var conn = config["AzureBlobStorage:ConnectionString"];
            return new BlobServiceClient(conn);
        });

        services.AddScoped<IFileStorageService>(sp =>
        {
            var blob = sp.GetRequiredService<BlobServiceClient>();
            var container = config["AzureBlobStorage:ContainerName"] ?? "media";
            return new AzureBlobStorageService(blob, container);
        });

        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordPolicy, PasswordPolicy>();
        services.AddTransient<IUserModificationService, UserModificationService>();
        services.AddSingleton<ICountryService, CountryService>();
        services.AddScoped<ICountryCodeResolver, CountryCodeResolver>();
        services.AddScoped<IAuthSessionService, AuthSessionService>();
        return services;
    }
}