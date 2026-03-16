import { useEffect } from 'react';
import { setToken, clearToken, getDecodedToken } from 'shared/lib/utils/auth';

export default function AppBootstrap() {
    useEffect(() => {
        (async () => {
            const decoded = getDecodedToken();

            if (decoded) return;

            const res = await fetch('http://localhost:5072/api/auth/refresh', {
                method: 'POST',
                credentials: 'include',
            });

            if (!res.ok) {
                clearToken();
                return;
            }

            const { accessToken } = await res.json();
            setToken(accessToken);
        })();
    }, []);

    return null;
}