import React, { useState, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';
import LoginForm from '../components/LoginForm';
import RegisterForm from '../components/RegisterForm';

interface JwtPayload {
    username: string;
    sub: string;
    email: string;
}

const ProfilePage = () => {
    const [username, setUsername] = useState<string | null>(null);
    const [isLoginView, setIsLoginView] = useState(true);

    const checkLogin = () => {
        const token = localStorage.getItem('token');
        if (token) {
            try {
                const decoded = jwtDecode<JwtPayload>(token);
                setUsername(decoded.username);
            } catch {
                setUsername(null);
            }
        } else {
            setUsername(null);
        }
    };

    const handleLogout = () => {
        localStorage.removeItem('token');
        setUsername(null);
    };

    useEffect(() => {
        checkLogin();
    }, []);

    if (username) {
        return (
            <div style={{ color: 'white', padding: '2rem' }}>
                <h2>Welcome, {username}!</h2>
                <button onClick={handleLogout}>Log out</button>
            </div>
        );
    }

    return (
        <div style={{ color: 'white', padding: '2rem' }}>
            {isLoginView ? (
                <>
                    <LoginForm onLogin={checkLogin} />
                    <p>
                        Don’t have an account?{' '}
                        <span
                            onClick={() => setIsLoginView(false)}
                            style={{ color: '#26cdd4', cursor: 'pointer', textDecoration: 'underline' }}
                        >
              Sign up!
            </span>
                    </p>
                </>
            ) : (
                <>
                    <RegisterForm />
                    <p>
                        Already have an account?{' '}
                        <span
                            onClick={() => setIsLoginView(true)}
                            style={{ color: '#26cdd4', cursor: 'pointer', textDecoration: 'underline' }}
                        >
              Log in!
            </span>
                    </p>
                </>
            )}
        </div>
    );
};

export default ProfilePage;
