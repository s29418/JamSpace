import { api } from '../../../shared/lib/api/base';

const ROOT = '/users';

export const addUserGenre = async (userId: string, name: string) => {
    const res = await api.post(`${ROOT}/${userId}/genres`, JSON.stringify(name), {
        headers: { 'Content-Type': 'application/json' },
    });

    const data = res.data as any;
    return {
        id: data.id ?? data.genreId ?? crypto.randomUUID(),
        name: data.name ?? data.genreName ?? name,
    };
};

export const deleteUserGenre = async (userId: string, genreId: string) => {
    const res = await api.delete<{ message?: string }>(`${ROOT}/${userId}/genres/${genreId}`);
    return res.data;
};
