import React, { useState } from 'react';
import { register } from '../services/auth.service';
import {useNavigate} from "react-router-dom";

const RegisterForm = () => {
    const [email, setEmail] = useState('');
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [confirm, setConfirm] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (password !== confirm) {
            alert('Passwords do not match');
            return;
        }

        try {
            await register(email, username, password);
            alert('Registration successful. You can now log in.');
            window.location.reload();
        } catch (error) {
            alert('Registration failed.');
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <h2>Register</h2>
            <input type="email" placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} required />
            <input type="text" placeholder="Username" value={username} onChange={e => setUsername(e.target.value)} required />
            <input type="password" placeholder="Password" value={password} onChange={e => setPassword(e.target.value)} required />
            <input type="password" placeholder="Confirm password" value={confirm} onChange={e => setConfirm(e.target.value)} required />
            <button type="submit">Register</button>
        </form>
    );
};

export default RegisterForm;
