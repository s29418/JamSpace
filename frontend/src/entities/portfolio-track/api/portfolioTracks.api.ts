import { api } from '../../../shared/api/base';
import type { PortfolioTrack } from '../model/types';

export const getUserPortfolioTracks = async (userId: string) => {
    const res = await api.get<PortfolioTrack[]>(`/users/${userId}/portfolio/tracks`);
    return res.data;
};
