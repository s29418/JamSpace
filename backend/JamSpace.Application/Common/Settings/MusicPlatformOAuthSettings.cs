namespace JamSpace.Application.Common.Settings;

public class MusicPlatformOAuthSettings
{
    public OAuthProviderSettings Spotify { get; set; } = new();
    public OAuthProviderSettings SoundCloud { get; set; } = new();
}
