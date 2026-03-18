import { useEffect } from "react";
import {
    setToken,
    clearToken,
    getDecodedToken,
    getToken,
} from "shared/lib/utils/auth";
import { markAuthReady } from "shared/lib/utils/waitForAuthReady";

export default function AppBootstrap() {
    useEffect(() => {
        let cancelled = false;

        const bootstrap = async () => {
            try {
                if (getDecodedToken()) {
                    markAuthReady();
                    return;
                }

                const res = await fetch("http://localhost:5072/api/auth/refresh", {
                    method: "POST",
                    credentials: "include",
                });

                if (cancelled) return;

                if (!res.ok) {
                    if (!getDecodedToken() && !getToken()) {
                        clearToken();
                    }

                    markAuthReady();
                    return;
                }

                const { accessToken } = await res.json();

                if (cancelled) return;

                if (!getDecodedToken()) {
                    setToken(accessToken);
                }

                markAuthReady();
            } catch {
                if (!cancelled && !getDecodedToken() && !getToken()) {
                    clearToken();
                }

                if (!cancelled) {
                    markAuthReady();
                }
            }
        };

        void bootstrap();

        return () => {
            cancelled = true;
        };
    }, []);

    return null;
}