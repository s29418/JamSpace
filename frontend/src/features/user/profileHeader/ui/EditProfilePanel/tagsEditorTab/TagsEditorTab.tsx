import React from 'react';
import styles from '../EditProfilePanel.module.css';
import {UserTag} from "../../../../../../entities/user/model/types";

type Props = {
    title: 'Skills' | 'Genres';
    items: UserTag[];
    onAdd: (name: string) => Promise<void> | void;
    onRemove: (id: string) => Promise<void> | void;
    placeholder?: string;
};

export const TagsEditorTab: React.FC<Props> = ({
                                                   title, items, onAdd, onRemove, placeholder
                                               }) => {
    const [value, setValue] = React.useState('');
    const [submitting, setSubmitting] = React.useState(false);

    const add = async () => {
        const name = value.trim();
        if (!name) return;
        try {
            setSubmitting(true);
            await onAdd(name);
            setValue('');
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <div className={styles.tagsTab}>
            <div className={styles.addRow}>
                <input
                    className={styles.input}
                    value={value}
                    onChange={e => setValue(e.target.value)}
                    onKeyDown={e => { if (e.key === 'Enter') { e.preventDefault(); void add(); } }}
                    placeholder={placeholder ?? `Add ${title.toLowerCase().slice(0, -1)}`}
                />
                <button
                    className={`${styles.button} ${styles.buttonPrimary}`}
                    onClick={() => void add()}
                    disabled={submitting || !value.trim()}
                    type="button"
                >
                    Add
                </button>
            </div>

            <div className={styles.tagsList}>
                {items.map(t => (
                    <span key={t.id} className={`${styles.chip} ${styles.chipEditable}`}>
            {t.name}
                        <button
                            type="button"
                            className={styles.tagRemove}
                            aria-label={`Remove ${t.name}`}
                            onClick={() => void onRemove(t.id)}
                        >×</button>
          </span>
                ))}
                {!items.length && <p className={styles.help}>No {title.toLowerCase()} yet.</p>}
            </div>
        </div>
    );
};
