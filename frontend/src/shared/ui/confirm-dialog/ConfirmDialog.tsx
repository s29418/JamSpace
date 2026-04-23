import React, { useEffect } from 'react';
import styles from './ConfirmDialog.module.css';

type Props = {
    isOpen: boolean;
    title?: string;
    message: string;
    confirmLabel?: string;
    cancelLabel?: string;
    loading?: boolean;
    onConfirm: () => void | Promise<void>;
    onCancel: () => void;
};

const ConfirmDialog = ({
    isOpen,
    title,
    message,
    confirmLabel = 'Confirm',
    cancelLabel = 'Cancel',
    loading = false,
    onConfirm,
    onCancel,
}: Props) => {
    useEffect(() => {
        if (!isOpen) return;
        const onKey = (event: KeyboardEvent) => {
            if (event.key === 'Escape' && !loading) onCancel();
        };
        window.addEventListener('keydown', onKey);
        return () => window.removeEventListener('keydown', onKey);
    }, [isOpen, loading, onCancel]);

    if (!isOpen) return null;

    return (
        <div
            role="dialog"
            aria-modal="true"
            aria-label={title ?? message}
            className={styles.backdrop}
            onClick={() => { if (!loading) onCancel(); }}
        >
            <div className={styles.body} onClick={(event) => event.stopPropagation()}>
                {title && <h3 className={styles.title}>{title}</h3>}
                <p className={styles.content}>{message}</p>

                <div className={styles.actions}>
                    <button
                        type="button"
                        className={styles.actionButton}
                        onClick={onConfirm}
                        disabled={loading}
                    >
                        {loading ? 'Working...' : confirmLabel}
                    </button>
                    <button
                        type="button"
                        className={styles.actionButton}
                        onClick={onCancel}
                        disabled={loading}
                    >
                        {cancelLabel}
                    </button>
                </div>
            </div>
        </div>
    );
};

export default ConfirmDialog;
