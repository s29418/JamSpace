import type { PortfolioTrackSource } from '../model/types';

type ExternalSource = Exclude<PortfolioTrackSource, 'Upload'>;

const SPOTIFY_SUPPORTED_TYPES = new Set(['track', 'album', 'playlist', 'artist', 'episode', 'show']);
const SOUNDCLOUD_ACCENT_COLOR = '26cdd4';

export function applyExternalTrackEmbedTheme(source: ExternalSource, embedUrl: string) {
    if (source !== 'SoundCloud') return embedUrl;

    try {
        const url = new URL(embedUrl);
        if (url.hostname.replace(/^www\./, '') !== 'w.soundcloud.com') {
            return embedUrl;
        }

        url.searchParams.set('color', SOUNDCLOUD_ACCENT_COLOR);
        url.searchParams.set('auto_play', 'false');
        url.searchParams.set('hide_related', 'true');
        url.searchParams.set('show_comments', 'false');
        url.searchParams.set('show_reposts', 'false');
        url.searchParams.set('show_teaser', 'false');

        return url.toString();
    } catch {
        return embedUrl;
    }
}

export function resolveSpotifyPlaylistEmbedUrl(externalUrl: string) {
    const trimmedUrl = externalUrl.trim();
    if (!trimmedUrl) return null;

    let url: URL;

    try {
        url = new URL(trimmedUrl);
    } catch {
        return null;
    }

    const host = url.hostname.replace(/^www\./, '');
    if (host !== 'open.spotify.com') return null;

    const [type, id] = url.pathname.split('/').filter(Boolean);
    if (type !== 'playlist' || !id) return null;

    return `https://open.spotify.com/embed/playlist/${id}`;
}

export function resolveExternalTrackEmbedUrl(source: ExternalSource, externalUrl: string) {
    const trimmedUrl = externalUrl.trim();
    if (!trimmedUrl) return null;

    let url: URL;

    try {
        url = new URL(trimmedUrl);
    } catch {
        return null;
    }

    if (source === 'Spotify') {
        const host = url.hostname.replace(/^www\./, '');
        if (host !== 'open.spotify.com') return null;

        const [type, id] = url.pathname.split('/').filter(Boolean);
        if (!type || !id || !SPOTIFY_SUPPORTED_TYPES.has(type)) return null;

        return `https://open.spotify.com/embed/${type}/${id}`;
    }

    const host = url.hostname.replace(/^www\./, '');
    if (host !== 'soundcloud.com') return null;

    return applyExternalTrackEmbedTheme(
        source,
        `https://w.soundcloud.com/player/?url=${encodeURIComponent(trimmedUrl)}`,
    );
}

export function resolveExternalTrackTitle(source: ExternalSource, externalUrl: string) {
    try {
        const url = new URL(externalUrl.trim());
        const parts = url.pathname.split('/').filter(Boolean);
        const lastPart = parts[parts.length - 1];

        if (lastPart) {
            return lastPart
                .replace(/[-_]+/g, ' ')
                .replace(/\s+/g, ' ')
                .trim();
        }
    } catch {
        return `${source} track`;
    }

    return `${source} track`;
}
