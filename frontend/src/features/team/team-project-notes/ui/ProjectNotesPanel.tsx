import React, { useEffect, useMemo, useState } from 'react';
import {
    ChevronLeftIcon,
    ChevronRightIcon,
    PlusIcon,
    XMarkIcon,
    EyeIcon,
} from '@heroicons/react/24/outline';
import type { ProjectAudioVersion, ProjectNote } from 'entities/team/model/types';
import { formatTime } from 'entities/team/lib/teamProjectFormatters';
import ProjectNoteCard from 'entities/team/ui/project-note-card/ProjectNoteCard';
import styles from './ProjectNotesPanel.module.css';

const INLINE_NOTES_LIMIT = 5;
const MODAL_NOTES_PAGE_SIZE = 9;
const ALL_FILTER_VALUE = 'all';
const GENERAL_FILTER_VALUE = 'general';
const DELETED_VERSION_PREFIX = 'deleted:';

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

type NoteAuthorFilter = {
    id: string;
    label: string;
};

const getDeletedVersionFilterValue = (name: string) => `${DELETED_VERSION_PREFIX}${name}`;

const noteOverlapsTimeRange = (note: ProjectNote, filterStart: string, filterEnd: string) => {
    if (!filterStart && !filterEnd) return true;
    if (note.startTimeSeconds === null || note.startTimeSeconds === undefined) return false;

    const noteStart = note.startTimeSeconds;
    const noteEnd = note.endTimeSeconds ?? note.startTimeSeconds;
    const noteMin = Math.min(noteStart, noteEnd);
    const noteMax = Math.max(noteStart, noteEnd);
    const start = filterStart ? Number(filterStart) : 0;
    const end = filterEnd ? Number(filterEnd) : Number.MAX_SAFE_INTEGER;
    const filterMin = Math.min(start, end);
    const filterMax = Math.max(start, end);

    return noteMin <= filterMax && noteMax >= filterMin;
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
}) => {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [versionFilter, setVersionFilter] = useState(ALL_FILTER_VALUE);
    const [authorFilter, setAuthorFilter] = useState(ALL_FILTER_VALUE);
    const [timeStartFilter, setTimeStartFilter] = useState('');
    const [timeEndFilter, setTimeEndFilter] = useState('');
    const [modalPage, setModalPage] = useState(0);

    const inlineNotes = notes.slice(0, INLINE_NOTES_LIMIT);

    const authors = useMemo<NoteAuthorFilter[]>(() => {
        const map = new Map<string, string>();
        notes.forEach(note => {
            if (!map.has(note.createdById)) {
                map.set(note.createdById, note.createdByDisplayName);
            }
        });

        return Array.from(map.entries())
            .map(([id, label]) => ({ id, label }))
            .sort((a, b) => a.label.localeCompare(b.label));
    }, [notes]);

    const deletedVersionNames = useMemo(
        () => Array.from(new Set(
            notes
                .filter(note => note.isAudioVersionDeleted && note.audioVersionName)
                .map(note => note.audioVersionName as string)
        )).sort((a, b) => a.localeCompare(b)),
        [notes]
    );

    const filteredNotes = useMemo(
        () => notes.filter(note => {
            if (versionFilter === GENERAL_FILTER_VALUE && (note.audioVersionId || note.isAudioVersionDeleted)) {
                return false;
            }

            if (versionFilter.startsWith(DELETED_VERSION_PREFIX)) {
                const deletedVersionName = versionFilter.slice(DELETED_VERSION_PREFIX.length);
                if (!note.isAudioVersionDeleted || note.audioVersionName !== deletedVersionName) return false;
            } else if (versionFilter !== ALL_FILTER_VALUE && versionFilter !== GENERAL_FILTER_VALUE && note.audioVersionId !== versionFilter) {
                return false;
            }

            if (authorFilter !== ALL_FILTER_VALUE && note.createdById !== authorFilter) {
                return false;
            }

            return noteOverlapsTimeRange(note, timeStartFilter, timeEndFilter);
        }),
        [authorFilter, notes, timeEndFilter, timeStartFilter, versionFilter]
    );

    const modalPageCount = Math.max(Math.ceil(filteredNotes.length / MODAL_NOTES_PAGE_SIZE), 1);
    const visibleModalNotes = filteredNotes.slice(
        modalPage * MODAL_NOTES_PAGE_SIZE,
        modalPage * MODAL_NOTES_PAGE_SIZE + MODAL_NOTES_PAGE_SIZE
    );

    useEffect(() => {
        setModalPage(current => Math.min(current, modalPageCount - 1));
    }, [modalPageCount]);

    const updateFilter = (callback: () => void) => {
        callback();
        setModalPage(0);
    };

    const openModal = () => {
        setModalPage(0);
        setIsModalOpen(true);
    };

    return (
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
                <PlusIcon className={styles.icon} />
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
                            maxLength={750}
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
                {inlineNotes.map(note => (
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

            {notes.length > 0 && (
                <button
                    type="button"
                    className={styles.viewAllButton}
                    onClick={openModal}
                >
                    <EyeIcon className={styles.icon} />
                    View all notes
                </button>
            )}

            {isModalOpen && (
                <div className={styles.modalOverlay} role="presentation">
                    <div className={styles.notesModal} role="dialog" aria-modal="true" aria-labelledby="project-notes-modal-title">
                        <div className={styles.modalHeader}>
                            <div>
                                <h3 id="project-notes-modal-title" className={styles.modalTitle}>All notes</h3>
                                <p className={styles.modalMeta}>{filteredNotes.length} of {notes.length} notes</p>
                            </div>

                            <button
                                type="button"
                                className={styles.modalCloseButton}
                                aria-label="Close notes"
                                onClick={() => setIsModalOpen(false)}
                            >
                                <XMarkIcon width={22} height={22} />
                            </button>
                        </div>

                        <div className={styles.filtersGrid}>
                            <label className={styles.field}>
                                <span>Version</span>
                                <select
                                    className={styles.select}
                                    value={versionFilter}
                                    onChange={(event) => updateFilter(() => setVersionFilter(event.target.value))}
                                >
                                    <option value={ALL_FILTER_VALUE}>All versions</option>
                                    <option value={GENERAL_FILTER_VALUE}>General</option>
                                    {versions.map(version => (
                                        <option key={version.id} value={version.id}>
                                            {version.name}
                                        </option>
                                    ))}
                                    {deletedVersionNames.map(versionName => (
                                        <option key={versionName} value={getDeletedVersionFilterValue(versionName)}>
                                            Deleted version: {versionName}
                                        </option>
                                    ))}
                                </select>
                            </label>

                            <label className={styles.field}>
                                <span>Author</span>
                                <select
                                    className={styles.select}
                                    value={authorFilter}
                                    onChange={(event) => updateFilter(() => setAuthorFilter(event.target.value))}
                                >
                                    <option value={ALL_FILTER_VALUE}>All authors</option>
                                    {authors.map(author => (
                                        <option key={author.id} value={author.id}>
                                            {author.label}
                                        </option>
                                    ))}
                                </select>
                            </label>

                            <label className={styles.field}>
                                <span>Time from</span>
                                <input
                                    type="number"
                                    min="0"
                                    step="1"
                                    className={styles.input}
                                    value={timeStartFilter}
                                    onChange={(event) => updateFilter(() => setTimeStartFilter(event.target.value))}
                                />
                            </label>

                            <label className={styles.field}>
                                <span>Time to</span>
                                <input
                                    type="number"
                                    min="0"
                                    step="1"
                                    className={styles.input}
                                    value={timeEndFilter}
                                    onChange={(event) => updateFilter(() => setTimeEndFilter(event.target.value))}
                                />
                            </label>
                        </div>

                        <div className={styles.modalNotesGrid}>
                            {visibleModalNotes.length === 0 && <p className={styles.muted}>No notes match these filters.</p>}
                            {visibleModalNotes.map(note => (
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

                        <div className={styles.modalPagination}>
                            <button
                                type="button"
                                className={styles.pageButton}
                                onClick={() => setModalPage(current => Math.max(current - 1, 0))}
                                disabled={modalPage === 0}
                            >
                                <ChevronLeftIcon width={18} height={18} />
                                Previous
                            </button>
                            <span className={styles.pageIndicator}>
                                Page {modalPage + 1} / {modalPageCount}
                            </span>
                            <button
                                type="button"
                                className={styles.pageButton}
                                onClick={() => setModalPage(current => Math.min(current + 1, modalPageCount - 1))}
                                disabled={modalPage >= modalPageCount - 1}
                            >
                                Next
                                <ChevronRightIcon width={18} height={18} />
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </section>
    );
};

export default ProjectNotesPanel;
