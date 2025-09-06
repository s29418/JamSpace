import React, { useEffect, useRef, useState } from 'react';
import styles from '../TeamSettingsModal.module.css';

type Props = {
    initialValue?: string;
    onSave: (value: string) => void | Promise<void>;
    onCancel: () => void;
};

export const MusicalRoleEditor: React.FC<Props> = ({
                                                       initialValue = '',
                                                       onSave,
                                                       onCancel,
                                                   }) => {
    const [value, setValue] = useState(initialValue);
    const containerRef = useRef<HTMLDivElement | null>(null);
    const contentRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        const el = containerRef.current;
        const inner = contentRef.current;
        if (!el || !inner) return;
    }, []);

    return (
        <div
            className={`${styles.expandable} ${styles.expanded ?? ''}`}
            ref={containerRef}
            aria-live="polite"
        >
            <div ref={contentRef}>
                <input
                    type="text"
                    value={value}
                    onChange={(e) => setValue(e.target.value)}
                    className={styles.musicalRoleInput}
                    placeholder="Enter new musical role"
                    aria-label="Musical role"
                />

                <div className={styles.editButtonsRow}>
                    <button
                        className={styles.userActionButton}
                        type="button"
                        onClick={() => onSave(value.trim())}
                        aria-label="Save musical role"
                        disabled={!value.trim()}
                    >
                        Save
                    </button>
                    <button
                        className={styles.userActionButton}
                        type="button"
                        onClick={onCancel}
                        aria-label="Cancel musical role edit"
                    >
                        Cancel
                    </button>
                </div>
            </div>
        </div>
    );
};
