import React, { useEffect, useRef, useState } from 'react';
import {
    PhotoIcon,
    TrashIcon,
    XMarkIcon,
} from '@heroicons/react/24/outline';
import type { TeamProject } from 'entities/team/model/types';
import { ApiError, isApiError } from 'shared/api/base';
import ConfirmDialog from 'shared/ui/confirm-dialog/ConfirmDialog';
import styles from './ProjectEditModal.module.css';

type Props = {
    isOpen: boolean;
    project: TeamProject | null;
    mode?: 'create' | 'edit';
    onClose: () => void;
    onSave: (payload: { name: string; description?: string | null; picture?: File | null }) => Promise<void>;
    onDelete?: () => Promise<void>;
};

const getErrorMessage = (error: unknown, fallback: string) =>
    isApiError(error) ? (error as ApiError).message : fallback;

const getProjectFallback = (name?: string | null) => name?.trim().charAt(0).toUpperCase() || '?';

const ProjectEditModal: React.FC<Props> = ({
    isOpen,
    project,
    mode = 'edit',
    onClose,
    onSave,
    onDelete,
}) => {
    const fileInputRef = useRef<HTMLInputElement | null>(null);
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [picture, setPicture] = useState<File | null>(null);
    const [picturePreviewUrl, setPicturePreviewUrl] = useState<string | null>(null);
    const [formError, setFormError] = useState<string | null>(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [confirmDeleteOpen, setConfirmDeleteOpen] = useState(false);
    const isCreateMode = mode === 'create';

    useEffect(() => {
        if (!isOpen) return;

        setName(project?.name ?? '');
        setDescription(project?.description ?? '');
        setPicture(null);
        setPicturePreviewUrl(null);
        setFormError(null);
        setSaving(false);
        setDeleting(false);
        setConfirmDeleteOpen(false);
        if (fileInputRef.current) fileInputRef.current.value = '';
    }, [isOpen, project]);

    useEffect(() => {
        if (!picture) {
            setPicturePreviewUrl(null);
            return;
        }

        const nextUrl = URL.createObjectURL(picture);
        setPicturePreviewUrl(nextUrl);

        return () => URL.revokeObjectURL(nextUrl);
    }, [picture]);

    useEffect(() => {
        if (!isOpen) return;

        const handleKeyDown = (event: KeyboardEvent) => {
            if (event.key === 'Escape' && !saving && !deleting && !confirmDeleteOpen) {
                onClose();
            }
        };

        window.addEventListener('keydown', handleKeyDown);
        return () => window.removeEventListener('keydown', handleKeyDown);
    }, [confirmDeleteOpen, deleting, isOpen, onClose, saving]);

    if (!isOpen || (!project && !isCreateMode)) return null;

    const close = () => {
        if (saving || deleting) return;
        onClose();
    };

    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        setFormError(null);

        const trimmedName = name.trim();
        if (!trimmedName) {
            setFormError('Project name is required.');
            return;
        }

        try {
            setSaving(true);
            await onSave({
                name: trimmedName,
                description: description.trim() || null,
                picture,
            });
            onClose();
        } catch (e) {
            setFormError(getErrorMessage(e, 'Failed to update project.'));
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!onDelete) return;

        try {
            setDeleting(true);
            await onDelete();
            setConfirmDeleteOpen(false);
            onClose();
        } catch (e) {
            setFormError(getErrorMessage(e, 'Failed to delete project.'));
            setConfirmDeleteOpen(false);
        } finally {
            setDeleting(false);
        }
    };

    return (
        <>
            <div
                className={styles.backdrop}
                role="presentation"
                onClick={close}
            >
                <form
                    className={styles.modal}
                    role="dialog"
                    aria-modal="true"
                    aria-label="Edit project"
                    onSubmit={handleSubmit}
                    onClick={(event) => event.stopPropagation()}
                >
                    <div className={styles.header}>
                        <h2 className={styles.title}>{isCreateMode ? 'Create project' : 'Edit project'}</h2>
                        <button
                            type="button"
                            className={styles.iconButton}
                            onClick={close}
                            aria-label="Close project editor"
                            disabled={saving || deleting}
                        >
                            <XMarkIcon width={20} height={20} />
                        </button>
                    </div>

                    <div className={styles.pictureRow}>
                        <div className={styles.picturePreview}>
                            {picturePreviewUrl ? (
                                <img src={picturePreviewUrl} alt="Project preview" />
                            ) : project?.pictureUrl ? (
                                <img src={project.pictureUrl} alt={project.name} />
                            ) : (
                                <span>{getProjectFallback(name || project?.name)}</span>
                            )}
                        </div>

                        <div className={styles.pictureActions}>
                            <input
                                ref={fileInputRef}
                                type="file"
                                accept="image/*"
                                className={styles.hiddenInput}
                                onChange={(event) => setPicture(event.target.files?.[0] ?? null)}
                            />
                            <button
                                type="button"
                                className={styles.secondaryButton}
                                onClick={() => fileInputRef.current?.click()}
                                disabled={saving || deleting}
                            >
                                <PhotoIcon width={20} height={20} />
                                {picture ? 'Change picture' : 'Choose picture'}
                            </button>
                            {picture && <span className={styles.fileName}>{picture.name}</span>}
                        </div>
                    </div>

                    <label className={styles.field}>
                        <span>Name</span>
                        <input
                            className={styles.input}
                            value={name}
                            onChange={(event) => setName(event.target.value)}
                            maxLength={80}
                            disabled={saving || deleting}
                            required
                        />
                    </label>

                    <label className={styles.field}>
                        <span>Description (optional)</span>
                        <textarea
                            className={styles.textarea}
                            value={description}
                            onChange={(event) => setDescription(event.target.value)}
                            maxLength={500}
                            disabled={saving || deleting}
                            rows={4}
                        />
                    </label>

                    {formError && <p className={styles.error}>{formError}</p>}

                    <div className={styles.footer}>
                        {!isCreateMode && onDelete ? (
                            <button
                                type="button"
                                className={styles.deleteButton}
                                onClick={() => setConfirmDeleteOpen(true)}
                                disabled={saving || deleting}
                            >
                                <TrashIcon width={18} height={18} />
                                Delete project
                            </button>
                        ) : (
                            <span />
                        )}

                        <div className={styles.actions}>
                            <button
                                type="button"
                                className={styles.secondaryButton}
                                onClick={close}
                                disabled={saving || deleting}
                            >
                                Cancel
                            </button>
                            <button
                                type="submit"
                                className={styles.primaryButton}
                                disabled={saving || deleting}
                            >
                                {saving ? (isCreateMode ? 'Creating...' : 'Saving...') : (isCreateMode ? 'Create project' : 'Save changes')}
                            </button>
                        </div>
                    </div>
                </form>
            </div>

            <ConfirmDialog
                isOpen={confirmDeleteOpen}
                title="Delete project"
                message={`Are you sure you want to delete "${project?.name ?? 'this project'}"?`}
                confirmLabel="Delete"
                loading={deleting}
                onConfirm={handleDelete}
                onCancel={() => {
                    if (!deleting) setConfirmDeleteOpen(false);
                }}
            />
        </>
    );
};

export default ProjectEditModal;
