using FluentValidation;
using JamSpace.Application.Common.Behaviors;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Services;
using JamSpace.Application.Features.Conversations.Strategies;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace JamSpace.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ValidatorOptions.Global.LanguageManager.Culture = CultureInfo.GetCultureInfo("en");

        services.AddScoped<IConversationCardStrategy, DirectConversationCardStrategy>();
        services.AddScoped<IConversationCardStrategy, TeamConversationCardStrategy>();
        services.AddScoped<IConversationCardStrategyResolver, ConversationCardStrategyResolver>();
        services.AddScoped<IMusicPlatformAuthClientResolver, MusicPlatformAuthClientResolver>();
        services.AddScoped<IExternalAccountTokenService, ExternalAccountTokenService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
