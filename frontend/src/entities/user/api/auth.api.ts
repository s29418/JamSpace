import { api } from '../../../shared/lib/api/base';

const ROOT = `/auth`;

export type LoginRequest = {
    email: string;
    password: string
};

export type RegisterRequest = {
    email: string;
    username: string;
    password: string
};

export type AuthResponse = {
    token: string;
    [k: string]: any
};

export const login = async (email: string, password: string) => {
    const res = await api.post<AuthResponse>(`${ROOT}/login`, { email, password });
    return res.data;
};

export const register = async (email: string, username: string, password: string) => {
    const res = await api.post<AuthResponse>(`${ROOT}/register`, { email, username, password });
    return res.data;
};
