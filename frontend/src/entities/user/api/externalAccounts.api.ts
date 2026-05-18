import { api } from '../../../shared/api/base';

export type ExternalMusicProvider = 'Spotify' | 'SoundCloud';

export type UserExternalAccount = {
    id: string;
    provider: ExternalMusicProvider;
    externalUserId: string;
    displayName: string;
    profileUrl: string;
    avatarUrl?: string | null;
    connectedAt: string;
    updatedAt: string;
};

export type PublicUserExternalAccount = {
    provider: ExternalMusicProvider;
    displayName: string;
    profileUrl: string;
    avatarUrl?: string | null;
};

type ExternalAuthUrlResponse = {
    url: string;
};

export const getMyExternalAccounts = async () => {
    const res = await api.get<UserExternalAccount[]>('/me/external-accounts');
    return res.data;
};

export const getUserExternalAccounts = async (userId: string) => {
    const res = await api.get<PublicUserExternalAccount[]>(`/users/${userId}/external-accounts`);
    return res.data;
};

export const startExternalAccountConnection = async (
    provider: ExternalMusicProvider,
    returnUrl: string,
) => {
    const res = await api.post<ExternalAuthUrlResponse>(
        `/me/external-accounts/${provider}/connect`,
        null,
        { params: { returnUrl } },
    );

    return res.data;
};

export const disconnectExternalAccount = async (provider: ExternalMusicProvider) => {
    await api.delete(`/me/external-accounts/${provider}`);
};
