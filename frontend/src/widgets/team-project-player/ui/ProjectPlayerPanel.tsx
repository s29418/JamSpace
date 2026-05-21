import React from 'react';
import { ChevronLeftIcon, ChevronRightIcon } from '@heroicons/react/24/outline';
import type { ProjectAudioVersion, ProjectNote } from 'entities/team/model/types';
import ProjectNoteCard from 'entities/team/ui/project-note-card/ProjectNoteCard';
import { AudioWaveformPeaks, PostAudioPlayer } from 'entities/post/ui/PostAudioPlayer';
import { toMediaProxyUrl } from 'shared/api/media';
import { formatTime } from 'entities/team/lib/teamProjectFormatters';
import styles from './ProjectPlayerPanel.module.css';

type ProjectPlayerPanelProps = {
    selectedVersion?: ProjectAudioVersion | null;
    artworkUrl?: string | null;
    waveformPeaks?: AudioWaveformPeaks;
    waveformDuration?: number;
    resumeTimeSeconds: number;
    shouldAutoPlay: boolean;
    currentPlayerSecond: number;
    visibleDynamicNotes: ProjectNote[];
    dynamicNotesCount: number;
    canShowDynamicNoteControls: boolean;
    canGoToPreviousDynamicNotes: boolean;
    canGoToNextDynamicNotes: boolean;
    showOnlySelectedVersionNotes: boolean;
    onTimeUpdate: (seconds: number) => void;
    onPlaybackStateChange: (isPlaying: boolean) => void;
    onToggleOnlySelectedVersionNotes: () => void;
    onPreviousDynamicNotes: () => void;
    onNextDynamicNotes: () => void;
};

const ProjectPlayerPanel: React.FC<ProjectPlayerPanelProps> = ({
    selectedVersion,
    artworkUrl,
    waveformPeaks,
    waveformDuration,
    resumeTimeSeconds,
    shouldAutoPlay,
    currentPlayerSecond,
    visibleDynamicNotes,
    dynamicNotesCount,
    canShowDynamicNoteControls,
    canGoToPreviousDynamicNotes,
    canGoToNextDynamicNotes,
    showOnlySelectedVersionNotes,
    onTimeUpdate,
    onPlaybackStateChange,
    onToggleOnlySelectedVersionNotes,
    onPreviousDynamicNotes,
    onNextDynamicNotes,
}) => (
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
                artworkUrl={artworkUrl}
                precomputedPeaks={waveformPeaks}
                precomputedDuration={waveformDuration}
                initialTimeSeconds={resumeTimeSeconds}
                autoPlayOnReady={shouldAutoPlay}
                onTimeUpdate={onTimeUpdate}
                onPlaybackStateChange={onPlaybackStateChange}
            />
        ) : (
            <div className={styles.emptyPlayer}>Upload a version to start listening.</div>
        )}

        <div className={styles.liveNotes}>
            <div className={styles.liveNotesHeader}>
                <h3 className={styles.subTitle}>Current notes</h3>
                <div className={styles.liveNotesControls}>
                    <button
                        type="button"
                        className={`${styles.versionFilterButton} ${showOnlySelectedVersionNotes ? styles.versionFilterButtonActive : ''}`}
                        onClick={onToggleOnlySelectedVersionNotes}
                        disabled={!selectedVersion}
                    >
                        Current version notes only
                    </button>
                    {canShowDynamicNoteControls && (
                        <>
                            <button
                                type="button"
                                className={styles.liveNotePageButton}
                                aria-label="Show previous current notes"
                                onClick={onPreviousDynamicNotes}
                                disabled={!canGoToPreviousDynamicNotes}
                            >
                                <ChevronLeftIcon width={18} height={18} />
                            </button>
                            <button
                                type="button"
                                className={styles.liveNotePageButton}
                                aria-label="Show next current notes"
                                onClick={onNextDynamicNotes}
                                disabled={!canGoToNextDynamicNotes}
                            >
                                <ChevronRightIcon width={18} height={18} />
                            </button>
                        </>
                    )}
                    <span className={styles.liveTime}>{formatTime(currentPlayerSecond)}</span>
                </div>
            </div>
            {dynamicNotesCount === 0 && (
                <p className={styles.muted}>No active notes at this time.</p>
            )}
            {dynamicNotesCount > 0 && (
                <div className={styles.liveNotesScroller}>
                    {visibleDynamicNotes.map(note => (
                        <ProjectNoteCard key={note.id} note={note} compact />
                    ))}
                </div>
            )}
        </div>
    </section>
);

export default ProjectPlayerPanel;
