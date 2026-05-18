export type PortfolioTrackSource = 'Upload' | 'Spotify' | 'SoundCloud';

export type PortfolioTrack = {
    id: string;
    userId: string;
    source: PortfolioTrackSource;
    title: string;
    artistName?: string | null;
    albumTitle?: string | null;
    artworkUrl?: string | null;
    durationMs?: number | null;
    externalTrackId?: string | null;
    externalUrl?: string | null;
    embedUrl?: string | null;
    fileUrl?: string | null;
    contentType?: string | null;
    length?: number | null;
    displayOrder: number;
    createdAt: string;
    updatedAt: string;
};
