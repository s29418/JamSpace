import React, { useState, useMemo } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import {
    ArrowLeftIcon,
    CheckCircleIcon,
    Cog6ToothIcon,
    PencilSquareIcon,
    TrashIcon,
} from '@heroicons/react/24/outline';
import { PostAudioPlayer } from 'entities/post/ui/PostAudioPlayer';
import { useTeamProjectWorkspace } from 'features/team/team-projects/model/useTeamProjectWorkspace';
import { toMediaProxyUrl } from 'shared/api/media';
import ProjectEditModal from 'widgets/team-projects/ui/ProjectEditModal';
import type { ProjectNote } from 'entities/team/model/types';
import styles from './TeamProjectDetailsPage.module.css';

const formatDate = (value: string) =>
    new Intl.DateTimeFormat('en', { day: 'numeric', month: 'short' }).format(new Date(value));

const formatTime = (seconds?: number | null) => {
    if (seconds === null || seconds === undefined) return null;

    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${String(remainingSeconds).padStart(2, '0')}`;
};

const formatRange = (note: ProjectNote) => {
    const start = formatTime(note.startTimeSeconds);
    const end = formatTime(note.endTimeSeconds);

    if (!start) return null;
    return end ? `${start} - ${end}` : start;
};

const getProjectFallback = (name?: string | null) => name?.trim().charAt(0).toUpperCase() || '?';

const TeamProjectDetailsPage: React.FC = () => {
    const { teamId, projectId } = useParams<{ teamId: string; projectId: string }>();
    const navigate = useNavigate();
    const [editOpen, setEditOpen] = useState(false);
    const {
        project,
        versions,
        notes,
        selectedVersion,
        selectedVersionId,
        setSelectedVersionId,
        loading,
        error,
        updateProject,
        updateProjectPicture,
        removeProject,
    } = useTeamProjectWorkspace(teamId, projectId);

    const activeTimestampNotes = useMemo(
        () => notes.filter(note => note.status === 'Active' && note.startTimeSeconds !== null && note.startTimeSeconds !== undefined),
        [notes]
    );

    if (loading) {
        return <main className={styles.page}><p className={styles.state}>Loading project...</p></main>;
    }

    if (error) {
        return <main className={styles.page}><p className={styles.error}>{error}</p></main>;
    }

    if (!project || !teamId) {
        return <main className={styles.page}><p className={styles.state}>Project not found.</p></main>;
    }

    return (
        <main className={styles.page}>
            <Link to={`/teams/${teamId}`} className={styles.backLink}>
                <ArrowLeftIcon width={18} height={18} />
                Team
            </Link>

            <header className={styles.header}>
                <div className={styles.projectArtwork}>
                    {project.pictureUrl ? (
                        <img src={project.pictureUrl} alt={project.name} />
                    ) : (
                        <span>{getProjectFallback(project.name)}</span>
                    )}
                </div>

                <div className={styles.headerText}>
                    <h1 className={styles.title}>{project.name}</h1>
                    {project.description && <p className={styles.description}>{project.description}</p>}
                    <p className={styles.meta}>Updated {formatDate(project.updatedAt)}</p>

                    <button
                        type="button"
                        className={styles.editProjectButton}
                        onClick={() => setEditOpen(true)}
                    >
                        <Cog6ToothIcon width={20} height={20} />
                        Edit
                    </button>
                </div>


            </header>

            <ProjectEditModal
                isOpen={editOpen}
                project={project}
                onClose={() => setEditOpen(false)}
                onSave={async ({ name, description, picture }) => {
                    await updateProject({ name, description });
                    if (picture) {
                        await updateProjectPicture(picture);
                    }
                }}
                onDelete={async () => {
                    await removeProject();
                    navigate(`/teams/${teamId}`);
                }}
            />

            <section className={styles.workspace}>
                <div className={styles.mainColumn}>
                    <section className={styles.playerPanel}>
                        <div className={styles.sectionHeader}>
                            <div>
                                <h2 className={styles.sectionTitle}>Current version</h2>
                                <p className={styles.sectionMeta}>
                                    {selectedVersion ? selectedVersion.name : 'No audio version selected'}
                                </p>
                            </div>
                        </div>

                        {selectedVersion ? (
                            <PostAudioPlayer
                                src={toMediaProxyUrl(selectedVersion.fileUrl) ?? selectedVersion.fileUrl}
                                title={selectedVersion.name}
                                artworkUrl={project.pictureUrl}
                            />
                        ) : (
                            <div className={styles.emptyPlayer}>Upload a version to start listening.</div>
                        )}

                        <div className={styles.liveNotes}>
                            <h3 className={styles.subTitle}>Active timestamp notes</h3>
                            {activeTimestampNotes.length === 0 && (
                                <p className={styles.muted}>No active timestamp notes yet.</p>
                            )}
                            {activeTimestampNotes.slice(0, 3).map(note => (
                                <NoteRow key={note.id} note={note} compact />
                            ))}
                        </div>
                    </section>

                    <section className={styles.notesPanel}>
                        <div className={styles.sectionHeader}>
                            <h2 className={styles.sectionTitle}>All notes</h2>
                            <span className={styles.count}>{notes.length}</span>
                        </div>

                        <div className={styles.notesList}>
                            {notes.length === 0 && <p className={styles.muted}>No notes yet.</p>}
                            {notes.map(note => (
                                <NoteRow key={note.id} note={note} />
                            ))}
                        </div>
                    </section>
                </div>

                <aside className={styles.versionsPanel}>
                    <div className={styles.sectionHeader}>
                        <h2 className={styles.sectionTitle}>Versions</h2>
                        <span className={styles.count}>{versions.length}</span>
                    </div>

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
                                    onClick={() => setSelectedVersionId(version.id)}
                                    onKeyDown={(event) => {
                                        if (event.key === 'Enter' || event.key === ' ') {
                                            event.preventDefault();
                                            setSelectedVersionId(version.id);
                                        }
                                    }}
                                >
                                    <span className={styles.versionName}>{version.name}</span>
                                    <span className={styles.versionDate}>{formatDate(version.createdAt)}</span>
                                    <button
                                        type="button"
                                        className={styles.deleteVersionButton}
                                        aria-label={`Delete ${version.name}`}
                                        onClick={(event) => event.stopPropagation()}
                                        disabled
                                    >
                                        <TrashIcon width={18} height={18} />
                                    </button>
                                </article>
                            );
                        })}
                    </div>
                </aside>
            </section>
        </main>
    );
};

type NoteRowProps = {
    note: ProjectNote;
    compact?: boolean;
};

const NoteRow: React.FC<NoteRowProps> = ({ note, compact = false }) => {
    const range = formatRange(note);
    const isCompleted = note.status === 'Completed';

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
                    <button type="button" className={styles.iconButton} aria-label={isCompleted ? 'Reopen note' : 'Complete note'} disabled>
                        <CheckCircleIcon width={18} height={18} />
                    </button>
                    <button type="button" className={styles.iconButton} aria-label="Edit note" disabled>
                        <PencilSquareIcon width={18} height={18} />
                    </button>
                    <button type="button" className={`${styles.iconButton} ${styles.dangerButton}`} aria-label="Delete note" disabled>
                        <TrashIcon width={18} height={18} />
                    </button>
                </div>
            )}
        </article>
    );
};

export default TeamProjectDetailsPage;
