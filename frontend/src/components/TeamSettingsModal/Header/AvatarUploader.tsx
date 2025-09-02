import React, { useEffect, useRef, useState } from 'react';
import ReactDOM from 'react-dom';
import styles from '../TeamSettingsModal.module.css';
import defaultTeamIcon from '../../../assets/defaultTeamIcon.jpg';
import { CameraIcon } from '@heroicons/react/24/outline';

type Props = {
    imageUrl?: string | null;
    canEdit?: boolean;
    onUpload: (file: File) => void | Promise<void>;
    className?: string;
};

export const AvatarUploader: React.FC<Props> = ({
                                                    imageUrl,
                                                    canEdit = false,
                                                    onUpload,
                                                    className,
                                                }) => {
    const fileInput = useRef<HTMLInputElement | null>(null);
    const [preview, setPreview] = useState<string | null>(null);
    const [file, setFile] = useState<File | null>(null);
    const [submitting, setSubmitting] = useState(false);
    const lastUrlRef = useRef<string | null>(imageUrl ?? null);

    // sprzątanie URL podglądu
    useEffect(() => {
        if (preview && imageUrl && imageUrl !== lastUrlRef.current) {
            URL.revokeObjectURL(preview);
            setPreview(null);     // przełącz się na świeży URL z propsów
        }
        lastUrlRef.current = imageUrl ?? null;
    }, [imageUrl, preview]);

    const shownImage = preview || imageUrl || defaultTeamIcon;

    const openPicker = () => {
        if (canEdit && !file) fileInput.current?.click();
    };

    const onFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const f = e.target.files?.[0];
        if (!f) return;
        if (preview) URL.revokeObjectURL(preview);
        const url = URL.createObjectURL(f);
        setPreview(url);
        setFile(f);
    };

    const onSave = async () => {
        if (!file || submitting) return;
        setSubmitting(true);
        try {
            await onUpload(file);
            if (preview) URL.revokeObjectURL(preview);
            setPreview(null);
            setFile(null);
            if (fileInput.current) fileInput.current.value = '';
        } finally {
            setSubmitting(false);
        }
    };

    const onCancel = () => {
        if (preview) URL.revokeObjectURL(preview);
        setPreview(null);
        setFile(null);
        if (fileInput.current) fileInput.current.value = '';
    };

    // Controls (Save/Cancel)
    const controls = (canEdit && file) ? (
        <div className={styles.editButtonsRow}>
            <button
                type="button"
                className={styles.nameActionButton}
                onClick={onSave}
                disabled={submitting}
            >
                {submitting ? 'Saving…' : 'Save'}
            </button>
            <button
                type="button"
                className={styles.nameActionButton}
                onClick={onCancel}
                disabled={submitting}
            >
                Cancel
            </button>
        </div>
    ) : null;

    const slot =
        typeof document !== 'undefined'
            ? document.getElementById('avatar-controls-slot')
            : null;

    return (
        <div
            className={className}
        >
            {/* AVATAR */}
            <div
                className={styles.avatarWrapper}
                role={canEdit ? 'button' : undefined}
                tabIndex={canEdit ? 0 : -1}
                aria-label={canEdit ? 'Change team picture' : 'Team picture'}
                onClick={openPicker}
                onKeyDown={(e) => {
                    if ((e.key === 'Enter' || e.key === ' ') && canEdit && !file) {
                        e.preventDefault();
                        openPicker();
                    }
                }}
            >
                <img
                    src={shownImage}
                    alt={canEdit ? 'Team picture. Click to change.' : 'Team picture'}
                    className={styles.avatarModal}
                    loading="lazy"
                />
                {canEdit && !file && (
                    <CameraIcon className={styles.cameraIcon} aria-hidden="true" />
                )}
            </div>

            {controls && !slot ? controls : null}

            <input
                ref={fileInput}
                type="file"
                accept="image/*"
                onChange={onFileChange}
                className={styles.hiddenInput}
                aria-hidden="true"
            />

            {controls && slot ? ReactDOM.createPortal(controls, slot) : null}
        </div>
    );
};
