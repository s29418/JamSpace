import { useEffect, useState } from "react";
import { getDecodedToken, getCurrentUserId, getToken } from "../auth/token";

type AuthState = {
    isAuthenticated: boolean;
    currentUserId: string | null;
    hasToken: boolean;
};

function readAuthState(): AuthState {
    const token = getToken();
    const decoded = getDecodedToken();

    return {
        isAuthenticated: !!decoded,
        currentUserId: getCurrentUserId(),
        hasToken: !!token,
    };
}

export function useAuthState() {
    const [authState, setAuthState] = useState<AuthState>(() => readAuthState());

    useEffect(() => {
        const sync = () => {
            setAuthState(readAuthState());
        };

        window.addEventListener("auth:changed", sync);
        window.addEventListener("auth:ready", sync);
        window.addEventListener("auth:unauthorized", sync);

        return () => {
            window.removeEventListener("auth:changed", sync);
            window.removeEventListener("auth:ready", sync);
            window.removeEventListener("auth:unauthorized", sync);
        };
    }, []);

    return authState;
}