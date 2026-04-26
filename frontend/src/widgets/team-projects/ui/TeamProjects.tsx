import React, { useEffect, useRef, useState } from 'react';
import { PlusIcon, XMarkIcon } from '@heroicons/react/24/outline';
import { useTeamProjects } from 'features/team/team-projects/model/useTeamProjects';
import { ApiError, isApiError } from 'shared/api/base';
import styles from './TeamProjects.module.css';

type Props = {
    teamId: string;
};

const getProjectFallback = (name: string) => name.trim().charAt(0).toUpperCase() || '?';
const getErrorMessage = (error: unknown, fallback: string) =>
    isApiError(error) ? (error as ApiError).message : fallback;

const TeamProjects = ({ teamId }: Props) => {
    const { projects, loading, error, createProject, updateProjectPicture } = useTeamProjects(teamId);
    const panelRef = useRef<HTMLElement | null>(null);
    const titleRef = useRef<HTMLHeadingElement | null>(null);
    const createButtonRef = useRef<HTMLButtonElement | null>(null);
    const fileInputRef = useRef<HTMLInputElement | null>(null);
    const [listHeight, setListHeight] = useState<number | null>(null);
    const [isCreateOpen, setIsCreateOpen] = useState(false);
    const [projectName, setProjectName] = useState('');
    const [projectImage, setProjectImage] = useState<File | null>(null);
    const [projectImagePreviewUrl, setProjectImagePreviewUrl] = useState<string | null>(null);
    const [formError, setFormError] = useState<string | null>(null);
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        const panelElement = panelRef.current;
        const titleElement = titleRef.current;
        const buttonElement = createButtonRef.current;

        if (!panelElement || !titleElement || !buttonElement) {
            setListHeight(null);
            return;
        }

        const updateListHeight = () => {
            if (window.innerWidth <= 900) {
                setListHeight(null);
                return;
            }

            const styles = window.getComputedStyle(panelElement);
            const paddingTop = Number.parseFloat(styles.paddingTop) || 0;
            const paddingBottom = Number.parseFloat(styles.paddingBottom) || 0;
            const titleBottomMargin = Number.parseFloat(window.getComputedStyle(titleElement).marginBottom) || 0;
            const buttonBottomMargin = Number.parseFloat(window.getComputedStyle(buttonElement).marginBottom) || 0;

            const reservedSpace =
                paddingTop +
                paddingBottom +
                titleElement.offsetHeight +
                titleBottomMargin +
                buttonElement.offsetHeight +
                buttonBottomMargin;

            setListHeight(Math.max(panelElement.clientHeight - reservedSpace, 0));
        };

        updateListHeight();

        const observer = new ResizeObserver(() => {
            updateListHeight();
        });

        observer.observe(panelElement);
        observer.observe(titleElement);
        observer.observe(buttonElement);
        window.addEventListener('resize', updateListHeight);

        return () => {
            observer.disconnect();
            window.removeEventListener('resize', updateListHeight);
        };
    }, []);

    useEffect(() => {
        if (!projectImage) {
            setProjectImagePreviewUrl(null);
            return;
        }

        const nextPreviewUrl = URL.createObjectURL(projectImage);
        setProjectImagePreviewUrl(nextPreviewUrl);

        return () => {
            URL.revokeObjectURL(nextPreviewUrl);
        };
    }, [projectImage]);

    const resetCreateForm = () => {
        setIsCreateOpen(false);
        setProjectName('');
        setProjectImage(null);
        setProjectImagePreviewUrl(null);
        if (fileInputRef.current) fileInputRef.current.value = '';
        setFormError(null);
    };

    const closeCreateForm = () => {
        if (saving) return;
        resetCreateForm();
    };

    const handleCreateProject = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        setFormError(null);

        const trimmedName = projectName.trim();
        if (!trimmedName) {
            setFormError('Project name is required.');
            return;
        }

        try {
            setSaving(true);
            const created = await createProject({ name: trimmedName });
            if (projectImage) {
                await updateProjectPicture(created.id, projectImage);
            }
            resetCreateForm();
        } catch (e) {
            setFormError(getErrorMessage(e, 'Failed to create project.'));
        } finally {
            setSaving(false);
        }
    };

    return (
        <aside ref={panelRef} className={styles.panel}>
            {isCreateOpen && (
                <div
                    className={styles.formOverlay}
                    role="presentation"
                    onClick={closeCreateForm}
                >
                    <form
                        className={styles.projectForm}
                        onSubmit={handleCreateProject}
                        onClick={(event) => event.stopPropagation()}
                    >
                        <div className={styles.formHeader}>
                            <h3 className={styles.formTitle}>Create project</h3>
                            <button
                                type="button"
                                className={styles.closeButton}
                                onClick={closeCreateForm}
                                aria-label="Close project form"
                            >
                                <XMarkIcon className={styles.closeIcon} />
                            </button>
                        </div>

                        <label className={styles.field}>
                            <span className={styles.fieldLabel}>Name</span>
                            <input
                                className={styles.input}
                                value={projectName}
                                onChange={(event) => setProjectName(event.target.value)}
                                maxLength={25}
                                required
                            />
                        </label>

                        <div className={styles.field}>
                            <span className={styles.fieldLabel}>Image (optional)</span>
                            <input
                                ref={fileInputRef}
                                type="file"
                                accept="image/*"
                                className={styles.hiddenFileInput}
                                onChange={(event) => {
                                    setProjectImage(event.target.files?.[0] ?? null);
                                }}
                            />

                            {projectImage && (
                                <>
                                    <div className={styles.previewBox}>
                                        {projectImagePreviewUrl ? (
                                            <img
                                                src={projectImagePreviewUrl}
                                                alt="Project preview"
                                                className={styles.previewImage}
                                            />
                                        ) : (
                                            <div className={styles.previewFallback}>
                                                {getProjectFallback(projectName || projectImage.name)}
                                            </div>
                                        )}
                                    </div>
                                    <span className={styles.fileName}>{projectImage.name}</span>
                                </>
                            )}

                            <button
                                type="button"
                                className={styles.secondaryButton}
                                onClick={() => fileInputRef.current?.click()}
                                disabled={saving}
                            >
                                {projectImage ? 'Change image' : 'Choose image'}
                            </button>

                        </div>

                        {formError && <p className={styles.formError}>{formError}</p>}

                        <div className={styles.formActions}>
                            <button
                                type="button"
                                className={styles.secondaryButton}
                                onClick={closeCreateForm}
                                disabled={saving}
                            >
                                Cancel
                            </button>
                            <button type="submit" className={styles.primaryButton} disabled={saving}>
                                {saving ? 'Creating...' : 'Create project'}
                            </button>
                        </div>
                    </form>
                </div>
            )}

            <h2 ref={titleRef} className={styles.title}>Team Projects</h2>

            <button
                ref={createButtonRef}
                type="button"
                className={styles.createButton}
                onClick={() => {
                    setFormError(null);
                    setIsCreateOpen(true);
                }}
            >
                <PlusIcon className={styles.buttonIcon} />
                Create new project
            </button>

            <div
                className={styles.projectsList}
                style={listHeight ? { height: `${listHeight}px`, maxHeight: `${listHeight}px` } : undefined}
            >
                {loading && <p className={styles.note}>Loading projects...</p>}
                {!loading && error && <p className={styles.error}>{error}</p>}
                {!loading && !error && projects.length === 0 && (
                    <p className={styles.note}>No projects yet.</p>
                )}

                {!loading && !error && projects.map((project) => (
                    <article key={project.id} className={styles.projectCard}>
                        <div className={styles.projectCardRow}>
                            {project.pictureUrl ? (
                                <img
                                    src={project.pictureUrl}
                                    alt={project.name}
                                    className={styles.projectImage}
                                    loading="lazy"
                                    decoding="async"
                                />
                            ) : (
                                <div className={styles.projectFallback}>
                                    {getProjectFallback(project.name)}
                                </div>
                            )}

                            <div className={styles.projectContent}>
                                <h3 className={styles.projectName}>{project.name}</h3>
                                <button
                                    type="button"
                                    className={styles.openButton}
                                    disabled
                                    aria-disabled="true"
                                >
                                    Open
                                </button>
                            </div>
                        </div>
                    </article>
                ))}
            </div>
        </aside>
    );
};

export default TeamProjects;
