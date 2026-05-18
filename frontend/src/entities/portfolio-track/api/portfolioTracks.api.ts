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

export type UploadPortfolioTrackRequest = {
    title: string;
    file: File;
    artworkFile?: File | null;
};

export const getUserPortfolioTracks = async (userId: string) => {
    const res = await api.get<PortfolioTrack[]>(`/users/${userId}/portfolio/tracks`);
    return res.data;
};

export const addExternalPortfolioTrack = async (request: AddExternalPortfolioTrackRequest) => {
    const res = await api.post<PortfolioTrack>('/me/portfolio/tracks/external-link', request);
    return res.data;
};

export const deletePortfolioTrack = async (trackId: string) => {
    await api.delete(`/me/portfolio/tracks/${trackId}`);
};

export const uploadPortfolioTrack = async (request: UploadPortfolioTrackRequest) => {
    const formData = new FormData();
    formData.append('Title', request.title);
    formData.append('File', request.file, request.file.name);

    if (request.artworkFile) {
        formData.append('ArtworkFile', request.artworkFile, request.artworkFile.name);
        formData.append('artworkFile', request.artworkFile, request.artworkFile.name);
    }

    const res = await api.post<PortfolioTrack>('/me/portfolio/tracks/upload', formData);

    return res.data;
};
