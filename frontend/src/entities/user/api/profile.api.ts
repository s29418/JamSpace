import {api, ApiError} from '../../../shared/lib/api/base';
import type { UserProfile } from '../model/types';

const ROOT = '/users';

function normalizeUserProfile(dto: any) {
    return {
        ...dto,
        skills: (dto.skills ?? []).map((x: any) => ({
            id: x.id ?? x.skillId,
            name: x.name ?? x.skillName,
        })),
        genres: (dto.genres ?? []).map((x: any) => ({
            id: x.id ?? x.genreId,
            name: x.name ?? x.genreName,
        })),
    };
}

export type UpdateUserProfileRequest = {
    setDisplayName?: boolean; displayName?: string;
    setBio?: boolean; bio?: string | null;
    setProfilePicture?: boolean; profilePictureUrl?: string | null;
    setLocation?: boolean; location?: { city?: string | null; country?: string | null };
    setEmail?: boolean; email?: string | null;
};

export type ChangePasswordRequest = {
    currentPassword: string;
    newPassword: string;
};


export const getUserById = async (userId: string) => {
    const res = await api.get<UserProfile>(`${ROOT}/${userId}`);
    return normalizeUserProfile(res.data as any) as UserProfile;
};

export const updateUserProfile = async (
    userId: string,
    p: UpdateUserProfileRequest,
    file?: File
) => {
    const fd = new FormData();

    fd.append('SetDisplayName', String(!!p.setDisplayName));
    fd.append('DisplayName', p.displayName ?? '');

    fd.append('SetBio', String(!!p.setBio));
    fd.append('Bio', p.bio ?? '');

    fd.append('SetLocation', String(!!p.setLocation));
    fd.append('Location.City', p.location?.city ?? '');
    fd.append('Location.Country', p.location?.country ?? '');

    fd.append('SetEmail', String(!!p.setEmail));
    fd.append('Email', p.email ?? '');

    if (file) {
        fd.append('SetProfilePicture', 'true');
        fd.append('File', file, file.name);
    } else {
        fd.append('SetProfilePicture', String(!!p.setProfilePicture));
        fd.append('ProfilePictureUrl', p.profilePictureUrl ?? '');
    }

    const res = await api.patch<{ message?: string }>(`/users/${userId}`, fd, {
        headers: { 'Content-Type': 'multipart/form-data' },
    });
    return res.data;
};

export const changeUserPassword = async (userId: string, body: ChangePasswordRequest) => {
    const res = await api.patch<{ message?: string }>(`${ROOT}/${userId}/password`, JSON.stringify(body), {
        headers: { 'Content-Type': 'application/json' },
    });
    return res.data;
};

export const deleteUserAccount = async (userId: string) => {
    const res = await api.delete<{ message?: string }>(`${ROOT}/${userId}`);
    return res.data;
};