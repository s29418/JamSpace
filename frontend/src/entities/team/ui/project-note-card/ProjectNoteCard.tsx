import React from 'react';
import { Link } from 'react-router-dom';
import {
    ArrowUturnLeftIcon,
    CheckCircleIcon,
    EyeIcon,
    PencilSquareIcon,
    TrashIcon,
} from '@heroicons/react/24/outline';
import type { ProjectNote } from 'entities/team/model/types';
import { formatDate, formatRange, getProjectFallback } from 'entities/team/lib/teamProjectFormatters';
import styles from './ProjectNoteCard.module.css';

type ProjectNoteCardProps = {
    note: ProjectNote;
    compact?: boolean;
    previewContent?: boolean;
    footerDetails?: boolean;
    busy?: boolean;
    onOpenDetails?: (note: ProjectNote) => void;
    onToggleStatus?: (note: ProjectNote) => void | Promise<void>;
    onEdit?: (note: ProjectNote) => void;
    onDelete?: (note: ProjectNote) => void;
};

const ProjectNoteCard: React.FC<ProjectNoteCardProps> = ({
    note,
    compact = false,
    previewContent = false,
    footerDetails = false,
    busy = false,
    onOpenDetails,
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
    const scrollToTop = () => window.setTimeout(() => window.scrollTo({ top: 0, left: 0 }), 0);

    return (
        <article className={`${styles.noteItem} ${isCompleted ? styles.noteItemCompleted : ''} ${compact ? styles.noteItemCompact : ''} ${previewContent ? styles.noteItemPreview : ''}`}>
            <div className={styles.noteHeader}>
                <div className={styles.author}>
                    <Link
                        to={`/profile/${note.createdById}`}
                        className={styles.avatarLink}
                        aria-label={`Open ${note.createdByDisplayName} profile`}
                        onClick={scrollToTop}
                    >
                    <div className={styles.avatar}>
                        {note.createdByAvatarUrl ? (
                            <img src={note.createdByAvatarUrl} alt="" />
                        ) : (
                            <span>{getProjectFallback(note.createdByDisplayName)}</span>
                        )}
                    </div>
                    </Link>
                    <div>
                        <Link
                            to={`/profile/${note.createdById}`}
                            className={styles.authorName}
                            onClick={scrollToTop}
                        >
                            {note.createdByDisplayName}
                        </Link>
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

            <p className={`${styles.noteContent} ${previewContent ? styles.noteContentPreview : ''}`}>
                {note.content}
            </p>

            {previewContent && !footerDetails && (
                <button
                    type="button"
                    className={styles.detailsButton}
                    onClick={() => onOpenDetails?.(note)}
                >
                    <EyeIcon width={17} height={17} />
                    View details
                </button>
            )}

            {!compact && (
                <div className={styles.noteFooter}>
                    {previewContent && footerDetails && (
                        <button
                            type="button"
                            className={styles.detailsButton}
                            onClick={() => onOpenDetails?.(note)}
                        >
                            <EyeIcon width={17} height={17} />
                            View details
                        </button>
                    )}

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
                </div>
            )}
        </article>
    );
};

export default ProjectNoteCard;
