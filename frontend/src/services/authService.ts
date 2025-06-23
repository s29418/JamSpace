import axios from 'axios';

const API_URL = 'http://localhost:5072/api/Auth';

export const login = async (email: string, password: string) => {
    const response = await axios.post(`${API_URL}/login`, { email, password });
    return response.data;
};

export const register = async (email: string, username: string, password: string) => {
    const response = await axios.post(`${API_URL}/register`, { email, username, password });
    return response.data;
};
