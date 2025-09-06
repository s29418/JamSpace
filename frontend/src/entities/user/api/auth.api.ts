import axios from 'axios';

const baseURL =
    (process.env.REACT_APP_API_URL as string | undefined) ?? 'http://localhost:5072/api';
const AUTH = `${baseURL}/auth`;

export type LoginRequest = { email: string; password: string };
export type RegisterRequest = { email: string; username: string; password: string };
export type AuthResponse = { token: string; [k: string]: any };

export const login = async (email: string, password: string) => {
    const res = await axios.post<AuthResponse>(`${AUTH}/login`, { email, password });
    return res.data;
};

export const register = async (email: string, username: string, password: string) => {
    const res = await axios.post<AuthResponse>(`${AUTH}/register`, { email, username, password });
    return res.data;
};
