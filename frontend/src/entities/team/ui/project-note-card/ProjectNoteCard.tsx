import React from 'react';
import {
    ArrowUturnLeftIcon,
    CheckCircleIcon,
    PencilSquareIcon,
    TrashIcon,
} from '@heroicons/react/24/outline';
import type { ProjectNote } from 'entities/team/model/types';
import { formatDate, formatRange, getProjectFallback } from 'entities/team/lib/teamProjectFormatters';
import styles from './ProjectNoteCard.module.css';

type ProjectNoteCardProps = {
    note: ProjectNote;
    compact?: boolean;
    busy?: boolean;
    onToggleStatus?: (note: ProjectNote) => void | Promise<void>;
    onEdit?: (note: ProjectNote) => void;
    onDelete?: (note: ProjectNote) => void;
};

const ProjectNoteCard: React.FC<ProjectNoteCardProps> = ({
    note,
    compact = false,
    busy = false,
    onToggleStatus,
    onEdit,
    onDelete,
}) => {
    const range = formatRange(note);
    const isCompleted = note.status === 'Completed';
    const sourceLabel = note.isAudioVersionDeleted
        ? `Deleted version: ${note.audioVersionName ?? 'Unknown'}`
        : note.audioVersionName
            ? `Version: ${note.audioVersionName}`
            : 'General';
    const toggleActionLabel = isCompleted ? 'Reopen' : 'Complete';

    return (
        <article className={`${styles.noteItem} ${isCompleted ? styles.noteItemCompleted : ''} ${compact ? styles.noteItemCompact : ''}`}>
            <div className={styles.noteHeader}>
                <div className={styles.author}>
                    <div className={styles.avatar}>
                        {note.createdByAvatarUrl ? (
                            <img src={note.createdByAvatarUrl} alt="" />
                        ) : (
                            <span>{getProjectFallback(note.createdByDisplayName)}</span>
                        )}
                    </div>
                    <div>
                        <div className={styles.authorName}>{note.createdByDisplayName}</div>
                        {note.createdByMusicalRole && (
                            <div className={styles.role}>{note.createdByMusicalRole}</div>
                        )}
                        <span className={`${styles.sourceTag} ${note.isAudioVersionDeleted ? styles.sourceTagDeleted : ''}`}>
                            {sourceLabel}
                        </span>
                    </div>
                </div>

                <div className={styles.noteMeta}>
                    {range && <span className={styles.range}>{range}</span>}
                    <span>{formatDate(note.createdAt)}</span>
                </div>
            </div>

            <p className={styles.noteContent}>{note.content}</p>

            {!compact && (
                <div className={styles.noteActions}>
                    <button
                        type="button"
                        className={styles.iconButton}
                        aria-label={`${toggleActionLabel} note`}
                        data-tooltip={toggleActionLabel}
                        onClick={() => onToggleStatus?.(note)}
                        disabled={busy}
                    >
                        {isCompleted ? <ArrowUturnLeftIcon width={18} height={18} /> : <CheckCircleIcon width={18} height={18} />}
                    </button>
                    <button
                        type="button"
                        className={styles.iconButton}
                        aria-label="Edit note"
                        data-tooltip="Edit"
                        onClick={() => onEdit?.(note)}
                        disabled={busy}
                    >
                        <PencilSquareIcon width={18} height={18} />
                    </button>
                    <button
                        type="button"
                        className={`${styles.iconButton} ${styles.dangerButton}`}
                        aria-label="Delete note"
                        data-tooltip="Delete"
                        onClick={() => onDelete?.(note)}
                        disabled={busy}
                    >
                        <TrashIcon width={18} height={18} />
                    </button>
                </div>
            )}
        </article>
    );
};

export default ProjectNoteCard;
