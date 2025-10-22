import { api } from '../../../shared/lib/api/base';
import type { UserProfile } from '../model/types';

const ROOT = '/users';

export const getUserById = async (userId: string) => {
    const res = await api.get<UserProfile>(`${ROOT}/${userId}`);
    return res.data;
};

export const followUser = async (userId: string) => {
    const res = await api.post<{ message?: string }>(`${ROOT}/${userId}/follow`, null);
    return res.data;
};

export const unfollowUser = async (userId: string) => {
    const res = await api.delete<{ message?: string }>(`${ROOT}/${userId}/follow`);
    return res.data;
};
