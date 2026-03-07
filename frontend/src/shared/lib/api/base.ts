import axios, { AxiosError } from 'axios';
import { getToken, setToken, clearToken } from  '../utils/auth'

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


export const api = axios.create({
    baseURL: '/api',
    timeout: 15000,
    withCredentials: true,
    headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use((config) => {
    const token = getToken?.();
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});

let isRefreshing = false;
let queue: ((t: string) => void)[] = [];
const enqueue = (cb: (t: string) => void) => queue.push(cb);
const flush = (t: string) => { queue.forEach(cb => cb(t)); queue = []; };

api.interceptors.response.use(
    (res) => res,
    async (error: AxiosError) => {
        if (error.request && !error.response) {
            return Promise.reject(new ApiError(0, 'Network error. Check your connection.'));
        }

        const status = error.response?.status ?? 0;

        if (status === 401) {
            const original = error.config!;
            if ((original as any)._retry) {
                clearToken();
                window.dispatchEvent(new CustomEvent('auth:unauthorized'));
                return Promise.reject(error);
            }
            (original as any)._retry = true;

            if (isRefreshing) {
                return new Promise(resolve => {
                    enqueue((newAccess) => {
                        (original.headers as any).Authorization = `Bearer ${newAccess}`;
                        resolve(api(original));
                    });
                });
            }

            isRefreshing = true;
            try {
                const refreshRes = await api.post('/auth/refresh', null, { withCredentials: true });
                const newAccess = (refreshRes.data as any).accessToken;
                setToken(newAccess);
                isRefreshing = false;
                flush(newAccess);

                (original.headers as any).Authorization = `Bearer ${newAccess}`;
                return api(original);
            } catch (e) {
                isRefreshing = false;
                clearToken();
                window.dispatchEvent(new CustomEvent('auth:unauthorized'));
                return Promise.reject(e);
            }
        }

        if (error.response) {
            const data = error.response.data as any;
            let message = 'Request failed';
            let details: Record<string, string[]> | undefined;

            if (data && typeof data === 'object') {
                const pd = data as ProblemDetails;
                details = pd.errors;
                const first = details ? Object.values(details).flat()[0] : undefined;
                message = first || pd.detail || pd.title || data?.message || error.response.statusText || message;
            } else if (typeof data === 'string' && data.trim()) {
                message = data;
            }

            return Promise.reject(new ApiError(status, message, details));
        }
        return Promise.reject(new ApiError(0, error.message || 'Unknown error'));
    }
);