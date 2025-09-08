using Azure.Storage.Blobs;
using JamSpace.Application.Common.Interfaces;
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

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
        services.AddScoped<ITeamInviteRepository, TeamInviteRepository>();

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
        return services;
    }
}