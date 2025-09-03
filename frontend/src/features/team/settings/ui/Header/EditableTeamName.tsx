import React, { useEffect, useRef, useState } from 'react';
import styles from '../TeamSettingsModal.module.css';
import { PencilSquareIcon } from '@heroicons/react/24/outline';

type Props = {
    name: string;
    canEdit?: boolean;
    onSave: (newName: string) => void | Promise<void>;
    className?: string;
    editButtonLabel?: string;
    closeButtonLabel?: string;
    rightActions?: React.ReactNode;
};

export const EditableTeamName: React.FC<Props> = ({
                                                      name,
                                                      canEdit = false,
                                                      onSave,
                                                      className,
                                                      rightActions,
                                                  }) => {
    const [editing, setEditing] = useState(false);
    const [value, setValue] = useState(name);
    const [submitting, setSubmitting] = useState(false);

    const inputRef = useRef<HTMLInputElement | null>(null);

    useEffect(() => setValue(name), [name]);

    useEffect(() => {
        if (editing) {
            setTimeout(() => {
                inputRef.current?.focus();
                inputRef.current?.select();
            }, 0);
        }
    }, [editing]);

    const onSubmit = async () => {
        const trimmed = value.trim();
        if (!trimmed || submitting) return;
        setSubmitting(true);
        try {
            await Promise.resolve(onSave(trimmed));
            setEditing(false);
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <div className={className}>
            <div className={styles.nameRow}>
                {editing ? (
                    <>
                        <input
                            ref={inputRef}
                            className={styles.teamNameInput}
                            type="text"
                            aria-label="New team name"
                            placeholder="Enter new team name"
                            value={value}
                            onChange={(e) => setValue(e.target.value)}
                            onKeyDown={(e) => {
                                if (e.key === 'Enter') onSubmit();
                                if (e.key === 'Escape') {
                                    setEditing(false);
                                    setValue(name);
                                }
                            }}
                            disabled={submitting}
                        />

                        <div className={styles.changeNameButtonsRow}>
                            <button
                                className={styles.nameActionButton}
                                type="button"
                                onClick={onSubmit}
                                disabled={submitting || value.trim().length === 0 || value.trim() === name.trim()}
                                aria-label="Save team name"
                            >
                                {submitting ? 'Saving…' : 'Save'}
                            </button>
                            <button
                                className={styles.nameActionButton}
                                type="button"
                                onClick={() => {
                                    setEditing(false);
                                    setValue(name);
                                }}
                                aria-label="Cancel team name edit"
                                disabled={submitting}
                            >
                                Cancel
                            </button>
                        </div>
                    </>
                ) : (
                    <>
                        <h2 className={`${styles.title} ${styles.centerText}`} title={name}>
                            {name}
                        </h2>

                        {canEdit && (
                            <button
                                type="button"
                                className={styles.editButton}
                                onClick={() => setEditing(true)}
                            >
                                <PencilSquareIcon className={styles.icon}/> Change Name
                            </button>
                        )}
                    </>
                )}

                {rightActions}
            </div>

        </div>
    );
};
