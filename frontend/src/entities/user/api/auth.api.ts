import { api } from '../../../shared/api/base';
import {clearToken, setToken} from "../../../shared/lib/auth/token";
import { markAuthReady } from "shared/lib/auth/waitForAuthReady";

const ROOT = `/auth`;

export type AuthResponse = {
    token: string;
    [k: string]: any
};

export async function login(email: string, password: string) {
    const res = await api.post(`${ROOT}/login`, { email, password });
    setToken(res.data.accessToken);
    markAuthReady();
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

export const verifyPassword = async (password: string) => {
    const res = await api.post(`${ROOT}/verify-password`, password);
    return res.data;
}