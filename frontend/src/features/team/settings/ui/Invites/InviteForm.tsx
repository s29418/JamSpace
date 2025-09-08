import React, { useCallback, useState } from 'react';
import styles from '../TeamSettingsModal.module.css';
import { UserPlusIcon } from '@heroicons/react/24/outline';

type Props = {
    onInvite: (username: string) => void | Promise<void>;
    className?: string;
    placeholder?: string;
    autoFocus?: boolean;
    disabled?: boolean;
};

export const InviteForm: React.FC<Props> = ({
                                                onInvite,
                                                placeholder = 'Enter username',
                                                autoFocus = false,
                                                disabled = false,
                                            }) => {
    const [username, setUsername] = useState('');
    const [submitting, setSubmitting] = useState(false);

    const handleSubmit = useCallback(
        async (e: React.FormEvent) => {
            e.preventDefault();
            const value = username.trim();
            if (!value || submitting || disabled) return;

            try {
                setSubmitting(true);
                console.log('[InviteForm] submit', value);
                await onInvite(value);
                setUsername('');
            } finally {
                setSubmitting(false);
            }
        },
        [username, onInvite, submitting, disabled]
    );

    return (
        <form className={styles.inviteForm} onSubmit={handleSubmit}>
            <input
                className={styles.inviteInput}
                type="text"
                placeholder={placeholder}
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                required
                autoFocus={autoFocus}
                aria-label="Invite user by username"
                disabled={disabled || submitting}
            />
            <button
                className={styles.inviteButton}
                type="submit"
                disabled={disabled || submitting}
            >
                <UserPlusIcon className={styles.icon} /> {submitting ? 'Inviting…' : 'Invite'}
            </button>
        </form>
    );
};
