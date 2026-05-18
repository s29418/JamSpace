import { api } from '../../../shared/api/base';
import type { PortfolioTrack, PortfolioTrackSource } from '../model/types';

export type AddExternalPortfolioTrackRequest = {
    source: Exclude<PortfolioTrackSource, 'Upload'>;
    title: string;
    artistName?: string | null;
    albumTitle?: string | null;
    artworkUrl?: string | null;
    durationMs?: number | null;
    externalTrackId?: string | null;
    externalUrl: string;
    embedUrl?: string | null;
};

export const getUserPortfolioTracks = async (userId: string) => {
    const res = await api.get<PortfolioTrack[]>(`/users/${userId}/portfolio/tracks`);
    return res.data;
};

export const addExternalPortfolioTrack = async (request: AddExternalPortfolioTrackRequest) => {
    const res = await api.post<PortfolioTrack>('/me/portfolio/tracks/external-link', request);
    return res.data;
};
