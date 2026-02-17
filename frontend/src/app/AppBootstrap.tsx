import { useEffect } from 'react';
import { setToken, clearToken, getToken } from 'shared/lib/utils/auth';

export default function AppBootstrap() {
    useEffect(() => {
        (async () => {
            if (getToken()) return;
            const res = await fetch('http://localhost:5072/api/auth/refresh', { method: 'POST', credentials: 'include' });
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