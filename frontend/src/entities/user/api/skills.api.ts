import { api } from '../../../shared/lib/api/base';

const ROOT = '/users';

export const addUserSkill = async (userId: string, name: string) => {
    const res = await api.post(`${ROOT}/${userId}/skills`, JSON.stringify(name), {
        headers: { 'Content-Type': 'application/json' },
    });

    const data = res.data as any;
    return {
        id: data.id ?? data.skillId ?? crypto.randomUUID(),
        name: data.name ?? data.skillName ?? name,
    };
};

export const deleteUserSkill = async (userId: string, skillId: string) => {
    const res = await api.delete<{ message?: string }>(`${ROOT}/${userId}/skills/${skillId}`);
    return res.data;
};
