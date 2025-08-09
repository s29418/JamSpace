import axios, { AxiosError } from 'axios';

export const API_URL = 'http://localhost:5072/api';

export class ApiError extends Error {
    status: number;
    constructor(status: number, message: string) {
        super(message);
        this.name = 'ApiError';
        this.status = status;
    }
}

export const api = axios.create({
    baseURL: API_URL,
    headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});

api.interceptors.response.use(
    (res) => res,
    (error: AxiosError<any>) => {
        if (error.response) {
            const status = error.response.status;
            const data = error.response.data;

            let message = 'Request failed';
            if (data && typeof data === 'object' && 'message' in data && data.message) {
                message = String(data.message);
            } else if (typeof data === 'string' && data.trim()) {
                message = data;
            } else if (error.response.statusText) {
                message = error.response.statusText;
            }

            return Promise.reject(new ApiError(status, message));
        }

        if (error.request) {
            return Promise.reject(new ApiError(0, 'Network error. Check your connection.'));
        }

        return Promise.reject(new ApiError(0, error.message || 'Unknown error'));
    }
);
