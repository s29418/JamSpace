import {login, logout, logoutAll, register} from "../../../entities/user/api/auth.api";
import { getToken } from "../../../shared/lib/auth/token";

export function useAuth() {
    return {
        loginUser: (email: string, password: string) => login(email, password),
        logoutUser: () => logout(),
        logoutAllUser: () => logoutAll(),
        registerUser: (email: string, username: string, password: string) => register(email, username, password),
        isAuthenticated: !!getToken(),
    };
}