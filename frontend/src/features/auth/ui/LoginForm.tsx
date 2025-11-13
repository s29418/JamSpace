import React, { useState } from 'react';
import { useAuth } from '../model/useAuth';
import styles from '../../../pages/Profile/ProfilePage.module.css';

const LoginForm = ({ onLogin }: { onLogin: () => void }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState<string | null>(null);

    const { loginUser } = useAuth();


    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        try {
            const res = await loginUser(email, password);
            onLogin();
        } catch (err) {
            setError(err instanceof Error ? err.message : 'An unknown error occurred.');
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <h2>Log in</h2>

            <input
                type="email"
                placeholder="Email"
                value={email}
                onChange={e => setEmail(e.target.value)}
                required
            />

            <input
                type="password"
                placeholder="Password"
                value={password}
                onChange={e => setPassword(e.target.value)}
                required
            />

            {error &&
                <p className={styles.error}>
                    {error}
                </p>
            }

            <button type="submit">
                Log in
            </button>

        </form>
    );
};

export default LoginForm;
