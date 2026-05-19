import React, { useState } from 'react';
import {
    ArrowTopRightOnSquareIcon,
    MusicalNoteIcon,
    TrashIcon
} from '@heroicons/react/24/outline';
import type { PortfolioTrack } from '../../../entities/portfolio-track/model/types';
import { PostAudioPlayer } from '../../../entities/post/ui/PostAudioPlayer';
import { PlatformLogo } from '../../../shared/ui/platform-logo/PlatformLogo';
import {
    AddExternalPortfolioTrackRequest,
    UploadPortfolioTrackRequest
} from '../../../entities/portfolio-track/api/portfolioTracks.api';
import { AddPortfolioTrackForm } from './AddPortfolioTrackForm';
import { toMediaProxyUrl } from '../../../shared/api/media';
import { applyExternalTrackEmbedTheme } from '../../../entities/portfolio-track/lib/externalTrackEmbed';
import styles from './PortfolioTracksSection.module.css';

type Props = {
    tracks: PortfolioTrack[];
    loading: boolean;
    error?: string | null;
    canAdd?: boolean;
    onAddExternalTrack?: (request: AddExternalPortfolioTrackRequest) => Promise<void> | void;
    onUploadTrack?: (request: UploadPortfolioTrackRequest) => Promise<void> | void;
    canDelete?: boolean;
    onDeleteTrack?: (trackId: string) => Promise<void> | void;
    canShare?: boolean;
    onShareTrack?: (trackId: string, content?: string) => Promise<void> | void;
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

export const PortfolioTracksSection: React.FC<Props> = ({
    tracks,
    loading,
    error,
    canAdd = false,
    onAddExternalTrack,
    onUploadTrack,
    canDelete = false,
    onDeleteTrack,
    canShare = false,
    onShareTrack,
}) => {
    const [sharingTrackId, setSharingTrackId] = useState<string | null>(null);
    const [shareContent, setShareContent] = useState('');
    const [sharingBusy, setSharingBusy] = useState(false);
    const addForm = canAdd && onAddExternalTrack && onUploadTrack ? (
        <AddPortfolioTrackForm
            onAddExternalTrack={onAddExternalTrack}
            onUploadTrack={onUploadTrack}
        />
    ) : null;

    async function handleShare(trackId: string) {
        if (!onShareTrack || sharingBusy) return;

        setSharingBusy(true);
        try {
            await onShareTrack(trackId, shareContent.trim());
            setShareContent('');
            setSharingTrackId(null);
        } finally {
            setSharingBusy(false);
        }
    }

    if (loading) {
        return (
            <section className={styles.section}>
                {addForm}
                <div className={styles.message}>Loading portfolio...</div>
            </section>
        );
    }

    if (error) {
        return (
            <section className={styles.section}>
                {addForm}
                <div className={`${styles.message} ${styles.error}`}>{error}</div>
            </section>
        );
    }

    if (!tracks.length) {
        return (
            <section className={styles.section}>
                {addForm}
                <div className={styles.message}>No portfolio tracks yet.</div>
            </section>
        );
    }

    return (
        <section className={styles.section}>
            {addForm}

            <div className={styles.list}>
                {tracks.map((track) => {
                    const meta = resolveTrackMeta(track);
                    const playerTitle = track.source === 'Upload' || !track.embedUrl
                        ? track.title
                        : `${track.source} player`;
                    const artworkUrl = track.source === 'Upload'
                        ? toMediaProxyUrl(track.artworkUrl)
                        : track.artworkUrl;
                    const fileUrl = track.source === 'Upload'
                        ? toMediaProxyUrl(track.fileUrl)
                        : track.fileUrl;
                    const embedUrl = track.embedUrl && track.source !== 'Upload'
                        ? applyExternalTrackEmbedTheme(track.source, track.embedUrl)
                        : track.embedUrl;

                    return (
                        <article key={track.id} className={styles.track}>
                            <div className={styles.header}>
                                <div className={styles.info}>
                                    <div className={styles.sourceRow}>
                                        {renderSourceIcon(track)}
                                        <span>{track.source}</span>
                                    </div>
                                    {track.source !== 'Upload' && (
                                        <h3>{track.title}</h3>
                                    )}
                                    {meta && <p>{meta}</p>}
                                </div>

                                <div className={styles.trackActions}>
                                    {track.externalUrl && (
                                        <a
                                            href={track.externalUrl}
                                            target="_blank"
                                            rel="noreferrer"
                                            className={styles.openLink}
                                            aria-label={`Open ${playerTitle}`}
                                        >
                                            <ArrowTopRightOnSquareIcon width={18} height={18} />
                                        </a>
                                    )}

                                    {canDelete && onDeleteTrack && (
                                        <button
                                            type="button"
                                            className={styles.deleteButton}
                                            aria-label={`Delete ${playerTitle}`}
                                            onClick={() => void onDeleteTrack(track.id)}
                                        >
                                            <TrashIcon width={18} height={18} />
                                        </button>
                                    )}

                                    {canShare && onShareTrack && (
                                        <button
                                            type="button"
                                            className={styles.shareButton}
                                            onClick={() => {
                                                setSharingTrackId((current) => current === track.id ? null : track.id);
                                                setShareContent('');
                                            }}
                                        >
                                            Post
                                        </button>
                                    )}
                                </div>
                            </div>

                            {sharingTrackId === track.id && (
                                <div className={styles.shareComposer}>
                                    <textarea
                                        className={styles.shareTextarea}
                                        placeholder="Add text to your post..."
                                        value={shareContent}
                                        onChange={(event) => setShareContent(event.target.value)}
                                        disabled={sharingBusy}
                                    />

                                    <div className={styles.shareActions}>
                                        <button
                                            type="button"
                                            className={styles.shareCancel}
                                            onClick={() => {
                                                setSharingTrackId(null);
                                                setShareContent('');
                                            }}
                                            disabled={sharingBusy}
                                        >
                                            Cancel
                                        </button>

                                        <button
                                            type="button"
                                            className={styles.sharePublish}
                                            onClick={() => void handleShare(track.id)}
                                            disabled={sharingBusy}
                                        >
                                            {sharingBusy ? 'Posting...' : 'Publish'}
                                        </button>
                                    </div>
                                </div>
                            )}

                            {embedUrl && (
                                <iframe
                                    className={styles.embed}
                                    src={embedUrl}
                                    title={playerTitle}
                                    allow="autoplay; clipboard-write; encrypted-media; fullscreen; picture-in-picture"
                                    loading="lazy"
                                />
                            )}

                            {!track.embedUrl && fileUrl && (
                                <PostAudioPlayer
                                    src={fileUrl}
                                    title={track.title}
                                    artworkUrl={artworkUrl}
                                />
                            )}
                        </article>
                    );
                })}
            </div>
        </section>
    );
};
