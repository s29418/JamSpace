using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Enums;

namespace JamSpace.Application.Common.Services;

public class MusicPlatformAuthClientResolver : IMusicPlatformAuthClientResolver
{
    private readonly Dictionary<ExternalMusicProvider, IMusicPlatformAuthClient> _clientsByProvider;

    public MusicPlatformAuthClientResolver(IEnumerable<IMusicPlatformAuthClient> clients)
    {
        _clientsByProvider = clients.ToDictionary(x => x.Provider);
    }

    public IMusicPlatformAuthClient Resolve(ExternalMusicProvider provider)
    {
        if (_clientsByProvider.TryGetValue(provider, out var client))
            return client;

        throw new NotSupportedException($"No music platform auth client registered for provider: {provider}");
    }
}
