import { getDecodedToken } from "./auth";

let authReadyResolved = false;
let authReadyPromise: Promise<void> | null = null;

export function markAuthReady() {
    authReadyResolved = true;
    window.dispatchEvent(new Event("auth:ready"));
}

export function waitForAuthReady(): Promise<void> {
    if (authReadyResolved || !!getDecodedToken()) {
        authReadyResolved = true;
        return Promise.resolve();
    }

    if (!authReadyPromise) {
        authReadyPromise = new Promise((resolve) => {
            const handler = () => {
                authReadyResolved = true;
                window.removeEventListener("auth:ready", handler);
                resolve();
            };

            window.addEventListener("auth:ready", handler);
        });
    }

    return authReadyPromise;
}