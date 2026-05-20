import React, { useCallback, useEffect, useRef, useState, useMemo } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import {
    ArrowLeftIcon,
    CheckCircleIcon,
    Cog6ToothIcon,
    PencilSquareIcon,
    PlusIcon,
    TrashIcon,
} from '@heroicons/react/24/outline';
import WaveSurfer from 'wavesurfer.js';
import { AudioWaveformPeaks, PostAudioPlayer } from 'entities/post/ui/PostAudioPlayer';
import { useTeamProjectWorkspace } from 'features/team/team-projects/model/useTeamProjectWorkspace';
import { toMediaProxyUrl } from 'shared/api/media';
import { isApiError } from 'shared/api/base';
import ConfirmDialog from 'shared/ui/confirm-dialog/ConfirmDialog';
import ProjectEditModal from 'widgets/team-projects/ui/ProjectEditModal';
import type { ProjectAudioVersion, ProjectNote } from 'entities/team/model/types';
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
const getErrorMessage = (error: unknown, fallback: string) =>
    isApiError(error) ? error.message : fallback;
const PRELOAD_VERSION_LIMIT = 6;

type WaveformCacheEntry = {
    peaks: AudioWaveformPeaks;
    duration: number;
};

const TeamProjectDetailsPage: React.FC = () => {
    const { teamId, projectId } = useParams<{ teamId: string; projectId: string }>();
    const navigate = useNavigate();
    const audioFileInputRef = useRef<HTMLInputElement | null>(null);
    const playbackRef = useRef({ currentTime: 0, isPlaying: false });
    const [editOpen, setEditOpen] = useState(false);
    const [versionResume, setVersionResume] = useState({ timeSeconds: 0, shouldAutoPlay: false });
    const [isVersionFormOpen, setIsVersionFormOpen] = useState(false);
    const [versionName, setVersionName] = useState('');
    const [versionFile, setVersionFile] = useState<File | null>(null);
    const [versionError, setVersionError] = useState<string | null>(null);
    const [savingVersion, setSavingVersion] = useState(false);
    const [deletingVersion, setDeletingVersion] = useState(false);
    const [versionToDelete, setVersionToDelete] = useState<ProjectAudioVersion | null>(null);
    const [waveformCache, setWaveformCache] = useState<Record<string, WaveformCacheEntry>>({});
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
        uploadVersion,
        removeVersion,
    } = useTeamProjectWorkspace(teamId, projectId);

    const activeTimestampNotes = useMemo(
        () => notes.filter(note => note.status === 'Active' && note.startTimeSeconds !== null && note.startTimeSeconds !== undefined),
        [notes]
    );

    useEffect(() => {
        let cancelled = false;
        const preloadedIds = new Set(Object.keys(waveformCache));
        const versionsToPreload = versions
            .filter(version => !preloadedIds.has(version.id))
            .slice(0, Math.max(PRELOAD_VERSION_LIMIT - preloadedIds.size, 0));

        if (versionsToPreload.length === 0) return;

        const preloadVersion = (version: ProjectAudioVersion) => new Promise<WaveformCacheEntry | null>((resolve) => {
            const container = document.createElement('div');
            container.style.position = 'fixed';
            container.style.left = '-10000px';
            container.style.top = '-10000px';
            container.style.width = '640px';
            container.style.height = '78px';
            document.body.appendChild(container);

            const wavesurfer = WaveSurfer.create({
                container,
                url: toMediaProxyUrl(version.fileUrl) ?? version.fileUrl,
                height: 78,
                waveColor: 'rgba(163,212,216,0.8)',
                progressColor: '#26cdd4',
                cursorColor: '#d9ffff',
                cursorWidth: 2,
                barWidth: 3,
                barGap: 2,
                barRadius: 999,
                barAlign: 'bottom',
                barMinHeight: 2,
                normalize: true,
                interact: false,
                hideScrollbar: true,
                autoScroll: false,
            });

            const cleanup = () => {
                wavesurfer.destroy();
                container.remove();
            };

            const unsubscribeReady = wavesurfer.on('ready', (duration) => {
                unsubscribeReady();
                unsubscribeError();

                const peaks = wavesurfer.exportPeaks({ maxLength: 12000 });
                cleanup();
                resolve({ peaks, duration });
            });

            const unsubscribeError = wavesurfer.on('error', () => {
                unsubscribeReady();
                unsubscribeError();
                cleanup();
                resolve(null);
            });
        });

        (async () => {
            for (const version of versionsToPreload) {
                if (cancelled) break;

                const entry = await preloadVersion(version);
                if (!entry || cancelled) continue;

                setWaveformCache(prev => (
                    prev[version.id] ? prev : { ...prev, [version.id]: entry }
                ));
            }
        })();

        return () => {
            cancelled = true;
        };
    }, [versions, waveformCache]);

    const handlePlayerTimeUpdate = useCallback((seconds: number) => {
        playbackRef.current.currentTime = seconds;
    }, []);

    const handlePlayerPlaybackStateChange = useCallback((isPlaying: boolean) => {
        playbackRef.current.isPlaying = isPlaying;
    }, []);

    const selectVersionAtCurrentPlayback = useCallback((versionId: string) => {
        if (versionId === selectedVersionId) return;

        setVersionResume({
            timeSeconds: playbackRef.current.currentTime,
            shouldAutoPlay: playbackRef.current.isPlaying,
        });
        setSelectedVersionId(versionId);
    }, [selectedVersionId, setSelectedVersionId]);

    const selectedWaveformCache = selectedVersion ? waveformCache[selectedVersion.id] : undefined;

    const resetVersionForm = () => {
        setIsVersionFormOpen(false);
        setVersionName('');
        setVersionFile(null);
        setVersionError(null);
        if (audioFileInputRef.current) audioFileInputRef.current.value = '';
    };

    const handleUploadVersion = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        setVersionError(null);

        const trimmedName = versionName.trim();
        if (!trimmedName) {
            setVersionError('Version name is required.');
            return;
        }

        if (!versionFile) {
            setVersionError('Audio file is required.');
            return;
        }

        try {
            setSavingVersion(true);
            await uploadVersion({ name: trimmedName, file: versionFile });
            resetVersionForm();
        } catch (e) {
            setVersionError(getErrorMessage(e, 'Failed to upload audio version.'));
        } finally {
            setSavingVersion(false);
        }
    };

    const handleDeleteVersion = async () => {
        if (!versionToDelete) return;

        try {
            setDeletingVersion(true);
            await removeVersion(versionToDelete.id);
            setVersionToDelete(null);
        } catch (e) {
            setVersionError(getErrorMessage(e, 'Failed to delete audio version.'));
            setVersionToDelete(null);
        } finally {
            setDeletingVersion(false);
        }
    };

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

            <ConfirmDialog
                isOpen={!!versionToDelete}
                title="Delete version"
                message={`Are you sure you want to delete "${versionToDelete?.name ?? 'this version'}"?`}
                confirmLabel="Delete"
                loading={deletingVersion}
                onConfirm={handleDeleteVersion}
                onCancel={() => {
                    if (!deletingVersion) setVersionToDelete(null);
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
                                precomputedPeaks={selectedWaveformCache?.peaks}
                                precomputedDuration={selectedWaveformCache?.duration}
                                initialTimeSeconds={versionResume.timeSeconds}
                                autoPlayOnReady={versionResume.shouldAutoPlay}
                                onTimeUpdate={handlePlayerTimeUpdate}
                                onPlaybackStateChange={handlePlayerPlaybackStateChange}
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

                    <button
                        type="button"
                        className={styles.addVersionButton}
                        onClick={() => {
                            setVersionError(null);
                            setIsVersionFormOpen(current => !current);
                        }}
                    >
                        <PlusIcon width={20} height={20} />
                        Add new version
                    </button>

                    {isVersionFormOpen && (
                        <form className={styles.versionForm} onSubmit={handleUploadVersion}>
                            <label className={styles.versionField}>
                                <span>Name</span>
                                <input
                                    className={styles.versionInput}
                                    value={versionName}
                                    onChange={(event) => setVersionName(event.target.value)}
                                    maxLength={100}
                                    disabled={savingVersion}
                                    required
                                />
                            </label>

                            <div className={styles.versionField}>
                                <span>Audio file</span>
                                <input
                                    ref={audioFileInputRef}
                                    type="file"
                                    accept="audio/*"
                                    className={styles.hiddenInput}
                                    onChange={(event) => setVersionFile(event.target.files?.[0] ?? null)}
                                />
                                <button
                                    type="button"
                                    className={styles.fileButton}
                                    onClick={() => audioFileInputRef.current?.click()}
                                    disabled={savingVersion}
                                >
                                    {versionFile ? 'Change file' : 'Choose file'}
                                </button>
                                {versionFile && <span className={styles.fileName}>{versionFile.name}</span>}
                            </div>

                            {versionError && <p className={styles.versionError}>{versionError}</p>}

                            <div className={styles.versionFormActions}>
                                <button
                                    type="button"
                                    className={styles.versionSecondaryButton}
                                    onClick={resetVersionForm}
                                    disabled={savingVersion}
                                >
                                    Cancel
                                </button>
                                <button
                                    type="submit"
                                    className={styles.versionPrimaryButton}
                                    disabled={savingVersion}
                                >
                                    {savingVersion ? 'Uploading...' : 'Upload'}
                                </button>
                            </div>
                        </form>
                    )}

                    {!isVersionFormOpen && versionError && <p className={styles.versionError}>{versionError}</p>}

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
                                    onClick={() => selectVersionAtCurrentPlayback(version.id)}
                                    onKeyDown={(event) => {
                                        if (event.key === 'Enter' || event.key === ' ') {
                                            event.preventDefault();
                                            selectVersionAtCurrentPlayback(version.id);
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
                                            setVersionToDelete(version);
                                        }}
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
