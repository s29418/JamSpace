import React, { useState } from 'react';
import { PaperAirplaneIcon } from '@heroicons/react/24/outline';
import styles from './PostCard.module.css';

type Props = {
    onSubmit: (content: string) => void | Promise<void>;
};

export const PostCommentComposer: React.FC<Props> = ({ onSubmit }) => {
    const [value, setValue] = useState('');
    const [busy, setBusy] = useState(false);
    const [error, setError] = useState<string | null>(null);

    async function submitComment() {
        const trimmed = value.trim();

        if (!trimmed) {
            setError('Comment cannot be empty.');
            return;
        }

        setBusy(true);
        setError(null);

        try {
            await onSubmit(trimmed);
            setValue('');
        } catch (e) {
            setError(e instanceof Error ? e.message : 'Failed to add comment.');
        } finally {
            setBusy(false);
        }
    }

    async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();
        await submitComment();
    }

    function handleKeyDown(event: React.KeyboardEvent<HTMLTextAreaElement>) {
        if (event.key !== 'Enter' || event.shiftKey) {
            return;
        }

        event.preventDefault();

        if (!busy && value.trim()) {
            void submitComment();
        }
    }

    return (
        <form className={styles.commentComposer} onSubmit={handleSubmit}>
            <div className={styles.commentComposerField}>
                <textarea
                    className={styles.commentComposerInput}
                    rows={2}
                    placeholder="Write a comment..."
                    value={value}
                    onChange={(event) => setValue(event.target.value)}
                    onKeyDown={handleKeyDown}
                    disabled={busy}
                />

                <button
                    type="submit"
                    className={styles.commentComposerButton}
                    disabled={busy || !value.trim()}
                    aria-label="Send comment"
                >
                    <PaperAirplaneIcon />
                </button>
            </div>

            <div className={styles.commentComposerMessage}>
                {error && <span>{error}</span>}
            </div>
        </form>
    );
};
