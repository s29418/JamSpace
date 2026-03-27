import {api} from "../../../shared/api/base";
import {UserLite} from "../model/types";

const ROOT = '/users';

export const getFollowers = async (userId: string): Promise<UserLite[]> => {
    const res = await api.get<any[]>(`${ROOT}/${userId}/followers`);

    return (res.data ?? []).map((x) => ({
        id: x.followerId ?? x.id,
        username: x.followerUsername ?? x.username ?? '',
        displayName: x.followerDisplayName ?? x.displayName ?? '',
        profilePictureUrl: x.followerPictureUrl ?? x.profilePictureUrl ?? null,
        isFollowing: x.isFollowing ?? false,
    }));
};

export const getFollowing = async (userId: string): Promise<UserLite[]> => {
    const res = await api.get<any[]>(`${ROOT}/${userId}/following`);

    return (res.data ?? []).map((x) => ({
        id: x.followeeId ?? x.id,
        username: x.followerUsername ?? x.username ?? '',
        displayName: x.followerDisplayName ?? x.displayName ?? '',
        profilePictureUrl: x.followerPictureUrl ?? x.profilePictureUrl ?? null,
        isFollowing: x.isFollowing ?? false,
    }));
};

export const followUser = async (userId: string) => {
    const res = await api.post<{ message?: string }>(`${ROOT}/${userId}/follow`, null);
    return res.data;
};

export const unfollowUser = async (userId: string) => {
    const res = await api.delete<{ message?: string }>(`${ROOT}/${userId}/follow`);
    return res.data;
};