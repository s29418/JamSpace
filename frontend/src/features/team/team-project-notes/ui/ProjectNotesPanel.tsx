import React from 'react';
import { PlusIcon } from '@heroicons/react/24/outline';
import type { ProjectAudioVersion, ProjectNote } from 'entities/team/model/types';
import { formatTime } from 'entities/team/lib/teamProjectFormatters';
import ProjectNoteCard from 'entities/team/ui/project-note-card/ProjectNoteCard';
import styles from './ProjectNotesPanel.module.css';

type ProjectNotesPanelProps = {
    notes: ProjectNote[];
    versions: ProjectAudioVersion[];
    formOpen: boolean;
    editingNote: ProjectNote | null;
    content: string;
    audioVersionId: string;
    attachCurrentTime: boolean;
    startTime: string;
    endTime: string;
    error: string | null;
    saving: boolean;
    updatingNoteId: string | null;
    onOpenCreate: () => void;
    onSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
    onContentChange: (value: string) => void;
    onAudioVersionChange: (value: string) => void;
    onAttachCurrentTimeChange: (checked: boolean) => void;
    onStartTimeChange: (value: string) => void;
    onEndTimeChange: (value: string) => void;
    onUsePlayerTime: () => void;
    onCloseForm: () => void;
    onToggleStatus: (note: ProjectNote) => void | Promise<void>;
    onEdit: (note: ProjectNote) => void;
    onDelete: (note: ProjectNote) => void;
};

const ProjectNotesPanel: React.FC<ProjectNotesPanelProps> = ({
    notes,
    versions,
    formOpen,
    editingNote,
    content,
    audioVersionId,
    attachCurrentTime,
    startTime,
    endTime,
    error,
    saving,
    updatingNoteId,
    onOpenCreate,
    onSubmit,
    onContentChange,
    onAudioVersionChange,
    onAttachCurrentTimeChange,
    onStartTimeChange,
    onEndTimeChange,
    onUsePlayerTime,
    onCloseForm,
    onToggleStatus,
    onEdit,
    onDelete,
}) => (
    <section className={styles.notesPanel}>
        <div className={styles.sectionHeader}>
            <h2 className={styles.sectionTitle}>All notes</h2>
            <span className={styles.count}>{notes.length}</span>
        </div>

        <button
            type="button"
            className={styles.addButton}
            onClick={onOpenCreate}
        >
            <PlusIcon width={20} height={20} />
            Add note
        </button>

        {formOpen && (
            <form className={styles.noteForm} onSubmit={onSubmit}>
                <div className={styles.noteFormHeader}>
                    <div>
                        <h3 className={styles.noteFormTitle}>
                            {editingNote ? 'Edit note' : 'Add note'}
                        </h3>
                        <p className={styles.noteFormMeta}>
                            {attachCurrentTime ? 'Timestamp note' : 'Project note'}
                        </p>
                    </div>

                    {attachCurrentTime && (
                        <span className={styles.noteTimeBadge}>
                            {formatTime(Number(startTime) || 0)}
                            {' - '}
                            {formatTime(Number(endTime || startTime) || 0)}
                        </span>
                    )}
                </div>

                <label className={styles.field}>
                    <span>Content</span>
                    <textarea
                        className={styles.textarea}
                        value={content}
                        onChange={(event) => onContentChange(event.target.value)}
                        rows={4}
                        maxLength={2000}
                        disabled={saving}
                        required
                    />
                </label>

                <div className={styles.noteOptionsGrid}>
                    <label className={styles.field}>
                        <span>Source version</span>
                        <select
                            className={styles.select}
                            value={audioVersionId}
                            onChange={(event) => onAudioVersionChange(event.target.value)}
                            disabled={saving}
                        >
                            <option value="">General</option>
                            {versions.map(version => (
                                <option key={version.id} value={version.id}>
                                    {version.name}
                                </option>
                            ))}
                        </select>
                    </label>

                    <label className={`${styles.timestampToggle} ${attachCurrentTime ? styles.timestampToggleActive : ''}`}>
                        <input
                            type="checkbox"
                            checked={attachCurrentTime}
                            onChange={(event) => onAttachCurrentTimeChange(event.target.checked)}
                            disabled={saving}
                        />
                        <span>
                            <strong>Timestamp</strong>
                            <small>{attachCurrentTime ? 'Shown near player time' : 'No player time'}</small>
                        </span>
                    </label>
                </div>

                {attachCurrentTime && (
                    <div className={styles.timeRangePanel}>
                        <div className={styles.timeRangeGrid}>
                            <label className={styles.field}>
                                <span>Start</span>
                                <input
                                    type="number"
                                    min="0"
                                    step="1"
                                    className={styles.input}
                                    value={startTime}
                                    onChange={(event) => onStartTimeChange(event.target.value)}
                                    disabled={saving}
                                />
                            </label>

                            <label className={styles.field}>
                                <span>End</span>
                                <input
                                    type="number"
                                    min="0"
                                    step="1"
                                    className={styles.input}
                                    value={endTime}
                                    onChange={(event) => onEndTimeChange(event.target.value)}
                                    disabled={saving}
                                />
                            </label>

                            <button
                                type="button"
                                className={styles.secondaryButton}
                                onClick={onUsePlayerTime}
                                disabled={saving}
                            >
                                Use player time
                            </button>
                        </div>
                    </div>
                )}

                {error && <p className={styles.error}>{error}</p>}

                <div className={styles.formActions}>
                    <button
                        type="button"
                        className={styles.secondaryButton}
                        onClick={onCloseForm}
                        disabled={saving}
                    >
                        Cancel
                    </button>
                    <button
                        type="submit"
                        className={styles.primaryButton}
                        disabled={saving}
                    >
                        {saving ? 'Saving...' : editingNote ? 'Save note' : 'Add note'}
                    </button>
                </div>
            </form>
        )}

        {!formOpen && error && <p className={styles.error}>{error}</p>}

        <div className={styles.notesList}>
            {notes.length === 0 && <p className={styles.muted}>No notes yet.</p>}
            {notes.map(note => (
                <ProjectNoteCard
                    key={note.id}
                    note={note}
                    busy={updatingNoteId === note.id}
                    onToggleStatus={onToggleStatus}
                    onEdit={onEdit}
                    onDelete={onDelete}
                />
            ))}
        </div>
    </section>
);

export default ProjectNotesPanel;
