import { jwtDecode } from "jwt-decode";

export type AppJwtPayload = {
    id?: string;
    userId?: string;
    sub?: string;
    username?: string;
    exp?: number;
};

export function getToken(): string | null {
    return localStorage.getItem("token");
}

export function getDecodedToken(): AppJwtPayload | null {
    const token = getToken();
    if (!token) return null;
    try {
        const payload = jwtDecode<AppJwtPayload>(token);
        const now = Math.floor(Date.now() / 1000);
        if (payload.exp && payload.exp < now) return null; // wygasł
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
