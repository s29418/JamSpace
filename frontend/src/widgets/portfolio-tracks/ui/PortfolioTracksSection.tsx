import React from 'react';
import { ArrowTopRightOnSquareIcon, MusicalNoteIcon } from '@heroicons/react/24/outline';
import type { PortfolioTrack } from '../../../entities/portfolio-track/model/types';
import { PostAudioPlayer } from '../../../entities/post/ui/PostAudioPlayer';
import { PlatformLogo } from '../../../shared/ui/platform-logo/PlatformLogo';
import styles from './PortfolioTracksSection.module.css';

type Props = {
    tracks: PortfolioTrack[];
    loading: boolean;
    error?: string | null;
};

function formatDuration(durationMs?: number | null) {
    if (!durationMs || durationMs < 0) return null;

    const totalSeconds = Math.floor(durationMs / 1000);
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;

    return `${minutes}:${String(seconds).padStart(2, '0')}`;
}

function resolveTrackMeta(track: PortfolioTrack) {
    return [
        track.artistName,
        track.albumTitle,
        formatDuration(track.durationMs),
    ].filter(Boolean).join(' • ');
}

function renderSourceIcon(track: PortfolioTrack) {
    if (track.source === 'Spotify' || track.source === 'SoundCloud') {
        return <PlatformLogo provider={track.source} size={24} />;
    }

    return <MusicalNoteIcon width={24} height={24} />;
}

export const PortfolioTracksSection: React.FC<Props> = ({ tracks, loading, error }) => {
    if (loading) {
        return (
            <section className={styles.section}>
                <h2 className={styles.title}>Portfolio</h2>
                <div className={styles.message}>Loading portfolio...</div>
            </section>
        );
    }

    if (error) {
        return (
            <section className={styles.section}>
                <h2 className={styles.title}>Portfolio</h2>
                <div className={`${styles.message} ${styles.error}`}>{error}</div>
            </section>
        );
    }

    if (!tracks.length) {
        return (
            <section className={styles.section}>
                <h2 className={styles.title}>Portfolio</h2>
                <div className={styles.message}>No portfolio tracks yet.</div>
            </section>
        );
    }

    return (
        <section className={styles.section}>
            <h2 className={styles.title}>Portfolio</h2>

            <div className={styles.list}>
                {tracks.map((track) => {
                    const meta = resolveTrackMeta(track);

                    return (
                        <article key={track.id} className={styles.track}>
                            <div className={styles.header}>
                                <div className={styles.artwork}>
                                    {track.artworkUrl ? (
                                        <img src={track.artworkUrl} alt="" />
                                    ) : (
                                        renderSourceIcon(track)
                                    )}
                                </div>

                                <div className={styles.info}>
                                    <div className={styles.sourceRow}>
                                        {renderSourceIcon(track)}
                                        <span>{track.source}</span>
                                    </div>
                                    <h3>{track.title}</h3>
                                    {meta && <p>{meta}</p>}
                                </div>

                                {track.externalUrl && (
                                    <a
                                        href={track.externalUrl}
                                        target="_blank"
                                        rel="noreferrer"
                                        className={styles.openLink}
                                        aria-label={`Open ${track.title}`}
                                    >
                                        <ArrowTopRightOnSquareIcon width={18} height={18} />
                                    </a>
                                )}
                            </div>

                            {track.embedUrl && (
                                <iframe
                                    className={styles.embed}
                                    src={track.embedUrl}
                                    title={track.title}
                                    allow="autoplay; clipboard-write; encrypted-media; fullscreen; picture-in-picture"
                                    loading="lazy"
                                />
                            )}

                            {!track.embedUrl && track.fileUrl && (
                                <PostAudioPlayer src={track.fileUrl} />
                            )}
                        </article>
                    );
                })}
            </div>
        </section>
    );
};
