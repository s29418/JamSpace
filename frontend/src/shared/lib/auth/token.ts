import { jwtDecode } from "jwt-decode";

export type AppJwtPayload = {
    id?: string;
    userId?: string;
    sub?: string;
    username?: string;
    exp?: number;
};

let accessTokenInMemory: string | null = localStorage.getItem("accessToken");

export function setToken(token: string) {
    accessTokenInMemory = token;
    localStorage.setItem("accessToken", token);
    window.dispatchEvent(new Event("auth:changed"));
}

export function getToken() {
    return accessTokenInMemory ?? localStorage.getItem("accessToken");
}

export function clearToken() {
    accessTokenInMemory = null;
    localStorage.removeItem("accessToken");
    window.dispatchEvent(new Event("auth:changed"));
}

export function getDecodedToken(): AppJwtPayload | null {
    const token = getToken();
    if (!token) return null;
    try {
        const payload = jwtDecode<AppJwtPayload>(token);
        const now = Math.floor(Date.now() / 1000);
        if (payload.exp && payload.exp < now) return null; 
        return payload;
    } catch {
        return null;
    }
}

export function getCurrentUserId(): string | null {
    const p = getDecodedToken();
    if (!p) return null;
    return p.userId || p.id || (typeof p.sub === "string" ? p.sub : null) || null;
}

export function getCurrentUsername(): string | null {
    const p = getDecodedToken();
    return p?.username ?? null;
}
