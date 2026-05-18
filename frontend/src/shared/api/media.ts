import { API_BASE_URL } from './base';

export function toMediaProxyUrl(url?: string | null) {
    if (!url) return url ?? null;

    return `${API_BASE_URL}/api/media?url=${encodeURIComponent(url)}`;
}
