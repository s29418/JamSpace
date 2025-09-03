// src/shared/lib/api/base.ts
import axios, { AxiosError, AxiosInstance } from 'axios';
import { getToken } from '../utils/auth'

type ProblemDetails = {
    type?: string;
    title?: string;
    status?: number;
    detail?: string;
    errors?: Record<string, string[]>;
};

export class ApiError extends Error {
    status: number;
    details?: Record<string, string[]>;
    constructor(status: number, message: string, details?: Record<string, string[]>) {
        super(message);
        this.name = 'ApiError';
        this.status = status;
        this.details = details;
    }
}

export const isApiError = (e: unknown): e is ApiError =>
    e instanceof Error && (e as any).name === 'ApiError';

const baseURL =
    (process.env.REACT_APP_API_URL as string | undefined) ?? 'http://localhost:5072/api';

export const api: AxiosInstance = axios.create({
    baseURL,
    timeout: 15000,
    withCredentials: false,
    headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use((config) => {
    const token = getToken?.();
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});

function pickMessage(pd?: ProblemDetails, fallback?: string) {
    if (!pd) return fallback ?? 'Unknown error';
    if (pd.detail) return pd.detail;
    if (pd.title) return pd.title;
    return fallback ?? 'Request failed';
}

api.interceptors.response.use(
    (res) => res,
    (error: AxiosError) => {
        if (error.request && !error.response) {
            return Promise.reject(new ApiError(0, 'Network error. Check your connection.'));
        }
        if (error.response) {
            const status = error.response.status ?? 0;
            let message = 'Request failed';
            let details: Record<string, string[]> | undefined;

            const data = error.response.data as any;
            if (data && typeof data === 'object') {
                const pd = data as ProblemDetails;
                details = pd.errors;
                message = pickMessage(pd, data?.message ?? error.response.statusText);
            } else if (typeof data === 'string' && data.trim()) {
                message = data;
            } else if (error.response.statusText) {
                message = error.response.statusText;
            }

            if (status === 401) {
                window.dispatchEvent(new CustomEvent('auth:unauthorized'));
            }
            return Promise.reject(new ApiError(status, message, details));
        }
        return Promise.reject(new ApiError(0, error.message || 'Unknown error'));
    }
);
