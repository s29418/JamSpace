using JamSpace.Domain.Enums;

namespace JamSpace.Application.Common.Interfaces;

public interface IMusicPlatformAuthClientResolver
{
    IMusicPlatformAuthClient Resolve(ExternalMusicProvider provider);
}
