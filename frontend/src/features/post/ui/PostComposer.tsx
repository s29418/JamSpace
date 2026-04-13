import React, { useId, useState } from 'react';
import { PhotoIcon } from '@heroicons/react/24/outline';
import styles from './PostComposer.module.css';

type Props = {
    onSubmit: (content: string, file?: File | null) => void | Promise<void>;
};

const ACCEPTED_TYPES = 'image/*,audio/*,video/*';

export const PostComposer: React.FC<Props> = ({ onSubmit }) => {
    const inputId = useId();
    const [content, setContent] = useState('');
    const [file, setFile] = useState<File | null>(null);
    const [busy, setBusy] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const canSubmit = Boolean(content.trim() || file);

    async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();

        if (!canSubmit || busy) {
            return;
        }

        setBusy(true);
        setError(null);

        try {
            await onSubmit(content, file);
            setContent('');
            setFile(null);
        } catch (e) {
            setError(e instanceof Error ? e.message : 'Failed to create post.');
        } finally {
            setBusy(false);
        }
    }

    return (
        <section className={styles.card}>
            <form className={styles.form} onSubmit={handleSubmit}>
                <div className={styles.title}>Create post</div>

                <textarea
                    className={styles.textarea}
                    placeholder="Share something with the community..."
                    value={content}
                    onChange={(event) => setContent(event.target.value)}
                    disabled={busy}
                />

                <div className={styles.toolbar}>
                    <div className={styles.left}>
                        <input
                            id={inputId}
                            className={styles.fileInput}
                            type="file"
                            accept={ACCEPTED_TYPES}
                            disabled={busy}
                            onChange={(event) => {
                                setFile(event.target.files?.[0] ?? null);
                                setError(null);
                            }}
                        />

                        <label htmlFor={inputId} className={styles.fileButton}>
                            <PhotoIcon width={18} height={18} />
                            Add media
                        </label>

                        {file && (
                            <div className={styles.fileMeta}>
                                <span className={styles.fileName}>{file.name}</span>
                                <button
                                    type="button"
                                    className={styles.removeFileButton}
                                    onClick={() => setFile(null)}
                                    disabled={busy}
                                >
                                    Remove
                                </button>
                            </div>
                        )}
                    </div>

                    <button
                        type="submit"
                        className={styles.submitButton}
                        disabled={!canSubmit || busy}
                    >
                        {busy ? 'Posting...' : 'Post'}
                    </button>
                </div>

                {error && <div className={styles.error}>{error}</div>}
            </form>
        </section>
    );
};
