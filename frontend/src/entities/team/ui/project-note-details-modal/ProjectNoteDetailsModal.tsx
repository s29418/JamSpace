import React, { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { XMarkIcon } from '@heroicons/react/24/outline';
import type { ProjectNote } from 'entities/team/model/types';
import { formatDate, formatRange, getProjectFallback } from 'entities/team/lib/teamProjectFormatters';
import styles from './ProjectNoteDetailsModal.module.css';

type ProjectNoteDetailsModalProps = {
    note: ProjectNote | null;
    onClose: () => void;
};

const ProjectNoteDetailsModal: React.FC<ProjectNoteDetailsModalProps> = ({ note, onClose }) => {
    useEffect(() => {
        if (!note) return;

        const onKeyDown = (event: KeyboardEvent) => {
            if (event.key === 'Escape') onClose();
        };

        window.addEventListener('keydown', onKeyDown);
        return () => window.removeEventListener('keydown', onKeyDown);
    }, [note, onClose]);

    if (!note) return null;

    const range = formatRange(note);
    const sourceLabel = note.isAudioVersionDeleted
        ? `Deleted version: ${note.audioVersionName ?? 'Unknown'}`
        : note.audioVersionName
            ? `Version: ${note.audioVersionName}`
            : 'General';
    const scrollToTop = () => window.setTimeout(() => window.scrollTo({ top: 0, left: 0 }), 0);

    return (
        <div className={styles.overlay} role="presentation" onClick={onClose}>
            <div
                className={styles.modal}
                role="dialog"
                aria-modal="true"
                aria-labelledby="project-note-details-title"
                onClick={(event) => event.stopPropagation()}
            >
                <div className={styles.header}>
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
                                id="project-note-details-title"
                                className={styles.title}
                                onClick={scrollToTop}
                            >
                                {note.createdByDisplayName}
                            </Link>
                            {note.createdByMusicalRole && (
                                <p className={styles.role}>{note.createdByMusicalRole}</p>
                            )}
                        </div>
                    </div>

                    <button
                        type="button"
                        className={styles.closeButton}
                        aria-label="Close note details"
                        onClick={onClose}
                    >
                        <XMarkIcon width={22} height={22} />
                    </button>
                </div>

                <div className={styles.meta}>
                    <span>{sourceLabel}</span>
                    {range && <span>{range}</span>}
                    <span>{formatDate(note.createdAt)}</span>
                </div>

                <p className={styles.content}>{note.content}</p>
            </div>
        </div>
    );
};

export default ProjectNoteDetailsModal;
