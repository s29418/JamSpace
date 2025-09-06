import { useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { clearToken } from 'shared/lib/utils/auth';

const LOGIN_ROUTE = '/profile';

export default function AuthEventsBridge() {
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        const on401 = () => {
            clearToken();
            if (location.pathname !== LOGIN_ROUTE) {
                navigate(LOGIN_ROUTE, { replace: true, state: { from: location } });
            }
        };
        window.addEventListener('auth:unauthorized', on401);
        return () => window.removeEventListener('auth:unauthorized', on401);
    }, [navigate, location]);

    return null;
}
