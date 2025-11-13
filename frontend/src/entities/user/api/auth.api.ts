import { api } from '../../../shared/lib/api/base';
import {clearToken, setToken} from "../../../shared/lib/utils/auth";

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

export async function login(email: string, password: string) {
    const res = await api.post(`${ROOT}/login`, { email, password });
    setToken(res.data.accessToken);
    return res.data;
}

export async function logout() {
    await api.post(`${ROOT}/logout`);
    clearToken();
}

export async function logoutAll() {
    await api.post(`${ROOT}/logout-all`);
    clearToken();
}

export const register = async (email: string, username: string, password: string) => {
    const res = await api.post<AuthResponse>(`${ROOT}/register`, { email, username, password });
    return res.data;
};
