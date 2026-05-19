using Azure.Storage.Blobs;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Common.Settings;
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
        services.Configure<MusicPlatformOAuthSettings>(config.GetSection("MusicPlatformOAuth"));

        services.AddDbContext<JamSpaceDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));
        
        services.AddMemoryCache();
        services.AddDataProtection();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserSearchRepository, UserSearchRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IPostLikeRepository, PostLikeRepository>();
        services.AddScoped<IPostCommentRepository, PostCommentRepository>();
        
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
        services.AddScoped<ITeamInviteRepository, TeamInviteRepository>();
        services.AddScoped<ITeamEventRepository, TeamEventRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectAudioVersionRepository, ProjectAudioVersionRepository>();
        services.AddScoped<IPortfolioTrackRepository, PortfolioTrackRepository>();
        
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IUserGenreRepository, UserGenreRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<IUserSkillRepository, UserSkillRepository>();
        services.AddScoped<IUserExternalAccountRepository, UserExternalAccountRepository>();
        services.AddScoped<IExternalOAuthStateRepository, ExternalOAuthStateRepository>();
        services.AddScoped<IUserFollowRepository, UserFollowRepository>();

        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IConversationParticipantRepository, ConversationParticipantRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();

        services.AddSingleton(_ =>
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
        services.AddScoped<ITokenProtector, DataProtectionTokenProtector>();
        services.AddSingleton<IOAuthPkceGenerator, OAuthPkceGenerator>();
        services.AddHttpClient<IMusicPlatformAuthClient, SpotifyAuthClient>();
        services.AddHttpClient<IMusicPlatformAuthClient, SoundCloudAuthClient>();
        services.AddHttpClient<ISpotifyPlaylistClient, SpotifyPlaylistClient>();
        services.AddSingleton<ICountryService, CountryService>();
        services.AddScoped<ICountryCodeResolver, CountryCodeResolver>();
        services.AddScoped<IAuthSessionService, AuthSessionService>();
        return services;
    }
}
