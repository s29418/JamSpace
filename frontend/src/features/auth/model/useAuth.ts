import { useCallback } from 'react';
import { ApiError, isApiError } from '../../../shared/lib/api/base';
import {login, register} from "../../../entities/user/api/auth.api";

export function useAuth() {

    const registerUser = useCallback(async (email: string, username: string, password: string) => {
        try {
            await register(email, username, password);
        } catch (e) {
            throw isApiError(e) ? e : new ApiError(0, 'Registration failed');
        }
    }, []);

    const loginUser = useCallback(async (email: string, password: string) => {
        try {
            const response = await login(email, password);
            return response;
        } catch (e) {
            throw isApiError(e) ? e : new ApiError(0, 'Login failed');
        }
    }, []);

    return { registerUser, loginUser };
}