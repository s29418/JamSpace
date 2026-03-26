import React, { useState } from 'react';
import { isApiError, ApiError } from '../../../shared/api/base';
import { useAuth } from "../model/useAuth";
import styles from './AuthForm.module.css';

type FieldErr = {
    username?: string;
    email?: string;
    password?: string;
    confirm?: string;
    _global?: string;
};

const RegisterForm: React.FC = () => {
    const { registerUser } = useAuth();

    const [email, setEmail] = useState('');
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [confirm, setConfirm] = useState('');

    const [err, setErr] = useState<FieldErr>({});
    const [submitting, setSubmitting] = useState(false);
    const [success, setSuccess] = useState<string | null>(null);

    function mapDetails(details?: Record<string, string[]>): FieldErr {
        if (!details) return {};
        const first = (k: string) => details[k]?.[0];

        const out: FieldErr = {};
        out.username = first('Username') ?? out.username;
        out.email = first('Email') ?? out.email;
        out.password = first('Password') ?? out.password;

        if (!out.username && !out.email && !out.password) {
            const anyFirst = Object.values(details).flat()[0];
            if (anyFirst) out._global = anyFirst;
        }

        return out;
    }

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();

        const ee: FieldErr = {};
        if (!email.trim()) ee.email = 'Email is required.';
        if (!username.trim()) ee.username = 'Username is required.';
        if (!password) ee.password = 'Password is required.';
        if (password !== confirm) ee.confirm = 'Passwords do not match.';
        setErr(ee);

        if (Object.keys(ee).length) return;

        try {
            setSubmitting(true);
            setErr({});
            setSuccess(null);

            await registerUser(email.trim(), username.trim(), password);
            setSuccess('Registration successful. You can now log in.');
        } catch (ex) {
            if (isApiError(ex)) {
                const api = ex as ApiError;
                const mapped = mapDetails(api.details);

                const hasFieldErrors = !!(mapped.username || mapped.email || mapped.password || mapped.confirm);

                setErr((prev) => ({
                    ...prev,
                    ...mapped,
                    _global: hasFieldErrors ? prev._global : (mapped._global || api.message || prev._global),
                }));
            } else {
                setErr((prev) => ({
                    ...prev,
                    _global: (ex as any)?.message || 'Registration failed.',
                }));
            }
        } finally {
            setSubmitting(false);
        }
    }

    return (
        <form onSubmit={handleSubmit} className={styles.form}>
            <h2 className={styles.title}>Register</h2>

            <div className={styles.row}>
                <input
                    type="email"
                    placeholder="Email"
                    value={email}
                    onChange={(e) => {
                        setEmail(e.target.value);
                        if (err.email) setErr((prev) => ({ ...prev, email: undefined }));
                    }}
                    aria-invalid={!!err.email}
                    className={err.email ? styles.inputError : ''}
                    required
                />
                {err.email && <p className={styles.error}>{err.email}</p>}
            </div>

            <div className={styles.row}>
                <input
                    type="text"
                    placeholder="Username"
                    value={username}
                    onChange={(e) => {
                        setUsername(e.target.value);
                        if (err.username) setErr((prev) => ({ ...prev, username: undefined }));
                    }}
                    aria-invalid={!!err.username}
                    className={err.username ? styles.inputError : ''}
                    required
                />
                {err.username && <p className={styles.error}>{err.username}</p>}
            </div>

            <div className={styles.row}>
                <input
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => {
                        setPassword(e.target.value);
                        if (err.password) setErr((prev) => ({ ...prev, password: undefined }));
                    }}
                    aria-invalid={!!err.password}
                    className={err.password ? styles.inputError : ''}
                    required
                />
                {err.password && <p className={styles.error}>{err.password}</p>}
            </div>

            <div className={styles.row}>
                <input
                    type="password"
                    placeholder="Confirm password"
                    value={confirm}
                    onChange={(e) => {
                        setConfirm(e.target.value);
                        if (err.confirm) setErr((prev) => ({ ...prev, confirm: undefined }));
                    }}
                    aria-invalid={!!err.confirm}
                    className={err.confirm ? styles.inputError : ''}
                    required
                />
                {err.confirm && <p className={styles.error}>{err.confirm}</p>}
            </div>

            {err._global && <p className={styles.error}>{err._global}</p>}
            {success && <p className={styles.success}>{success}</p>}

            <button type="submit" disabled={submitting}>
                {submitting ? 'Registering…' : 'Register'}
            </button>
        </form>
    );
};

export default RegisterForm;