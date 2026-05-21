import React from 'react';
import { PlusIcon, TrashIcon } from '@heroicons/react/24/outline';
import type { ProjectAudioVersion } from 'entities/team/model/types';
import { formatDate } from 'entities/team/lib/teamProjectFormatters';
import styles from './ProjectVersionsPanel.module.css';

type ProjectVersionsPanelProps = {
    versions: ProjectAudioVersion[];
    selectedVersionId?: string | null;
    isFormOpen: boolean;
    name: string;
    file: File | null;
    error: string | null;
    saving: boolean;
    fileInputRef: React.RefObject<HTMLInputElement | null>;
    onToggleForm: () => void;
    onNameChange: (value: string) => void;
    onFileChange: (file: File | null) => void;
    onSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
    onCancel: () => void;
    onSelectVersion: (versionId: string) => void;
    onDeleteVersion: (version: ProjectAudioVersion) => void;
};

const ProjectVersionsPanel: React.FC<ProjectVersionsPanelProps> = ({
    versions,
    selectedVersionId,
    isFormOpen,
    name,
    file,
    error,
    saving,
    fileInputRef,
    onToggleForm,
    onNameChange,
    onFileChange,
    onSubmit,
    onCancel,
    onSelectVersion,
    onDeleteVersion,
}) => (
    <aside className={styles.versionsPanel}>
        <div className={styles.sectionHeader}>
            <h2 className={styles.sectionTitle}>Versions</h2>
            <span className={styles.count}>{versions.length}</span>
        </div>

        <button
            type="button"
            className={styles.addButton}
            onClick={onToggleForm}
        >
            <PlusIcon width={20} height={20} />
            Add new version
        </button>

        {isFormOpen && (
            <form className={styles.versionForm} onSubmit={onSubmit}>
                <label className={styles.field}>
                    <span>Name</span>
                    <input
                        className={styles.input}
                        value={name}
                        onChange={(event) => onNameChange(event.target.value)}
                        maxLength={25}
                        disabled={saving}
                        required
                    />
                </label>

                <div className={styles.field}>
                    <span>Audio file</span>
                    <input
                        ref={fileInputRef}
                        type="file"
                        accept="audio/*"
                        className={styles.hiddenInput}
                        onChange={(event) => onFileChange(event.target.files?.[0] ?? null)}
                    />
                    <button
                        type="button"
                        className={styles.fileButton}
                        onClick={() => fileInputRef.current?.click()}
                        disabled={saving}
                    >
                        {file ? 'Change file' : 'Choose file'}
                    </button>
                    {file && <span className={styles.fileName}>{file.name}</span>}
                </div>

                {error && <p className={styles.error}>{error}</p>}

                <div className={styles.formActions}>
                    <button
                        type="button"
                        className={styles.secondaryButton}
                        onClick={onCancel}
                        disabled={saving}
                    >
                        Cancel
                    </button>
                    <button
                        type="submit"
                        className={styles.primaryButton}
                        disabled={saving}
                    >
                        {saving ? 'Uploading...' : 'Upload'}
                    </button>
                </div>
            </form>
        )}

        {!isFormOpen && error && <p className={styles.error}>{error}</p>}

        <div className={styles.versionsList}>
            {versions.length === 0 && <p className={styles.muted}>No versions yet.</p>}
            {versions.map(version => {
                const isActive = version.id === selectedVersionId;

                return (
                    <article
                        key={version.id}
                        role="button"
                        tabIndex={0}
                        className={`${styles.versionItem} ${isActive ? styles.versionItemActive : ''}`}
                        onClick={() => onSelectVersion(version.id)}
                        onKeyDown={(event) => {
                            if (event.key === 'Enter' || event.key === ' ') {
                                event.preventDefault();
                                onSelectVersion(version.id);
                            }
                        }}
                    >
                        <span className={styles.versionName}>{version.name}</span>
                        <span className={styles.versionDate}>{formatDate(version.createdAt)}</span>
                        <button
                            type="button"
                            className={styles.deleteVersionButton}
                            aria-label={`Delete ${version.name}`}
                            onClick={(event) => {
                                event.stopPropagation();
                                onDeleteVersion(version);
                            }}
                        >
                            <TrashIcon width={18} height={18} />
                        </button>
                    </article>
                );
            })}
        </div>
    </aside>
);

export default ProjectVersionsPanel;
