import React, { useId, useState } from 'react';
import {
    MusicalNoteIcon,
    PhotoIcon,
    TrashIcon,
    PaperAirplaneIcon
} from '@heroicons/react/24/outline';
import type { PortfolioTrack } from '../../../entities/portfolio-track/model/types';
import { PlatformLogo } from '../../../shared/ui/platform-logo/PlatformLogo';
import type { CreatePostSpotifyPlaylist } from '../../../entities/post/api/posts.api';
import type { SpotifyPlaylist } from '../../../entities/user/api/externalAccounts.api';
import styles from './PostComposer.module.css';

type Props = {
    onSubmit: (
        content: string,
        file?: File | null,
        portfolioTrackId?: string | null,
        spotifyPlaylist?: CreatePostSpotifyPlaylist | null,
    ) => void | Promise<void>;
    portfolioTracks?: PortfolioTrack[];
    spotifyPlaylists?: SpotifyPlaylist[];
    spotifyPlaylistsLoading?: boolean;
    spotifyPlaylistsError?: string | null;
    onRefreshSpotifyPlaylists?: () => void | Promise<void>;
};

const ACCEPTED_TYPES = 'image/*,audio/*,video/*';

function getTrackLabel(track: PortfolioTrack) {
    return track.title || `${track.source} track`;
}

function renderTrackIcon(track: PortfolioTrack) {
    if (track.source === 'Spotify' || track.source === 'SoundCloud') {
        return <PlatformLogo provider={track.source} size={20} />;
    }

    return <MusicalNoteIcon width={20} height={20} />;
}

export const PostComposer: React.FC<Props> = ({
    onSubmit,
    portfolioTracks = [],
    spotifyPlaylists = [],
    spotifyPlaylistsLoading = false,
    spotifyPlaylistsError = null,
    onRefreshSpotifyPlaylists,
}) => {
    const inputId = useId();
    const [content, setContent] = useState('');
    const [file, setFile] = useState<File | null>(null);
    const [selectedTrack, setSelectedTrack] = useState<PortfolioTrack | null>(null);
    const [trackPickerOpen, setTrackPickerOpen] = useState(false);
    const [playlistPickerOpen, setPlaylistPickerOpen] = useState(false);
    const [selectedPlaylist, setSelectedPlaylist] = useState<CreatePostSpotifyPlaylist | null>(null);
    const [busy, setBusy] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const canSubmit = Boolean(content.trim() || file || selectedTrack || selectedPlaylist);

    async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();

        if (!canSubmit || busy) {
            return;
        }

        setBusy(true);
        setError(null);

        try {
            await onSubmit(content, file, selectedTrack?.id ?? null, selectedPlaylist);
            setContent('');
            setFile(null);
            setSelectedTrack(null);
            setSelectedPlaylist(null);
            setTrackPickerOpen(false);
            setPlaylistPickerOpen(false);
        } catch (e) {
            setError(e instanceof Error ? e.message : 'Failed to create post.');
        } finally {
            setBusy(false);
        }
    }

    return (
        <section className={styles.card}>
            <form className={styles.form} onSubmit={handleSubmit}>
                <div className={styles.title}>Create post</div>

                <textarea
                    className={styles.textarea}
                    placeholder="Share something with the community..."
                    value={content}
                    onChange={(event) => setContent(event.target.value)}
                    disabled={busy}
                />

                <div className={styles.toolbar}>
                    <div className={styles.left}>
                        <input
                            id={inputId}
                            className={styles.fileInput}
                            type="file"
                            accept={ACCEPTED_TYPES}
                            disabled={busy}
                            onChange={(event) => {
                                setFile(event.target.files?.[0] ?? null);
                                setSelectedTrack(null);
                                setSelectedPlaylist(null);
                                setTrackPickerOpen(false);
                                setPlaylistPickerOpen(false);
                                setError(null);
                            }}
                        />

                        <label htmlFor={inputId} className={styles.fileButton}>
                            <PhotoIcon width={18} height={18} />
                            Add media
                        </label>

                        {file && (
                            <div className={styles.fileMeta}>
                                <span className={styles.fileName}>{file.name}</span>
                                <button
                                    type="button"
                                    className={styles.removeFileButton}
                                    onClick={() => setFile(null)}
                                    disabled={busy}
                                >
                                    <TrashIcon width={18} height={18} />
                                    Remove
                                </button>
                            </div>
                        )}

                        {portfolioTracks.length > 0 && (
                            <div className={styles.trackPicker}>
                                <button
                                    type="button"
                                    className={styles.trackButton}
                                    onClick={() => {
                                        setTrackPickerOpen((open) => !open);
                                        setPlaylistPickerOpen(false);
                                    }}
                                    disabled={busy}
                                >
                                    <MusicalNoteIcon width={18} height={18} />
                                    Add portfolio track
                                </button>

                                {trackPickerOpen && (
                                    <div className={styles.trackMenu}>
                                        {portfolioTracks.map((track) => (
                                            <button
                                                key={track.id}
                                                type="button"
                                                className={styles.trackOption}
                                                onClick={() => {
                                                    setSelectedTrack(track);
                                                    setFile(null);
                                                    setSelectedPlaylist(null);
                                                    setTrackPickerOpen(false);
                                                    setPlaylistPickerOpen(false);
                                                    setError(null);
                                                }}
                                                disabled={busy}
                                            >
                                                <span className={styles.trackOptionIcon}>
                                                    {renderTrackIcon(track)}
                                                </span>
                                                <span className={styles.trackOptionText}>
                                                    <strong>{getTrackLabel(track)}</strong>
                                                    <small>{track.source}</small>
                                                </span>
                                            </button>
                                        ))}
                                    </div>
                                )}
                            </div>
                        )}

                        {selectedTrack && (
                            <div className={styles.selectedTrack}>
                                <span className={styles.selectedTrackIcon}>
                                    {renderTrackIcon(selectedTrack)}
                                </span>
                                <span className={styles.selectedTrackName}>
                                    {getTrackLabel(selectedTrack)}
                                </span>
                                <button
                                    type="button"
                                    className={styles.removeFileButton}
                                    onClick={() => setSelectedTrack(null)}
                                    disabled={busy}
                                >
                                    <TrashIcon width={18} height={18} />
                                    Remove
                                </button>
                            </div>
                        )}

                        <div className={styles.playlistPicker}>
                            <button
                                type="button"
                                className={styles.trackButton}
                                onClick={() => {
                                    setPlaylistPickerOpen((open) => !open);
                                    setTrackPickerOpen(false);
                                    setError(null);
                                    if (!playlistPickerOpen && onRefreshSpotifyPlaylists) {
                                        void onRefreshSpotifyPlaylists();
                                    }
                                }}
                                disabled={busy}
                            >
                                <PlatformLogo provider="Spotify" size={18} />
                                Add Spotify playlist
                            </button>

                            {playlistPickerOpen && (
                                <div className={styles.trackMenu}>
                                    {spotifyPlaylistsLoading && (
                                        <div className={styles.trackMenuMessage}>Loading Spotify playlists...</div>
                                    )}

                                    {!spotifyPlaylistsLoading && spotifyPlaylistsError && (
                                        <div className={styles.trackMenuMessage}>{spotifyPlaylistsError}</div>
                                    )}

                                    {!spotifyPlaylistsLoading && !spotifyPlaylistsError && spotifyPlaylists.length === 0 && (
                                        <div className={styles.trackMenuMessage}>No Spotify playlists found.</div>
                                    )}

                                    {!spotifyPlaylistsLoading && !spotifyPlaylistsError && spotifyPlaylists.map((playlist) => (
                                        <button
                                            key={playlist.id}
                                            type="button"
                                            className={styles.trackOption}
                                            onClick={() => {
                                                setSelectedPlaylist({
                                                    title: playlist.name,
                                                    externalUrl: playlist.externalUrl,
                                                    embedUrl: playlist.embedUrl,
                                                });
                                                setFile(null);
                                                setSelectedTrack(null);
                                                setPlaylistPickerOpen(false);
                                                setError(null);
                                            }}
                                            disabled={busy}
                                        >
                                            {playlist.imageUrl ? (
                                                <img
                                                    className={styles.playlistImage}
                                                    src={playlist.imageUrl}
                                                    alt=""
                                                />
                                            ) : (
                                                <span className={styles.trackOptionIcon}>
                                                    <PlatformLogo provider="Spotify" size={20} />
                                                </span>
                                            )}
                                            <span className={styles.trackOptionText}>
                                                <strong>{playlist.name}</strong>
                                                <small>Spotify playlist</small>
                                            </span>
                                        </button>
                                    ))}
                                </div>
                            )}
                        </div>

                        {selectedPlaylist && (
                            <div className={styles.selectedTrack}>
                                <span className={styles.selectedTrackIcon}>
                                    <PlatformLogo provider="Spotify" size={20} />
                                </span>
                                <span className={styles.selectedTrackName}>
                                    {selectedPlaylist.title}
                                </span>
                                <button
                                    type="button"
                                    className={styles.removeFileButton}
                                    onClick={() => setSelectedPlaylist(null)}
                                    disabled={busy}
                                >
                                    <TrashIcon width={18} height={18} />
                                    Remove
                                </button>
                            </div>
                        )}
                    </div>

                    <button
                        type="submit"
                        className={styles.submitButton}
                        disabled={!canSubmit || busy}
                    >
                        {busy ? 'Posting...' : 'Post'}
                        <PaperAirplaneIcon width={18} height={18} />
                    </button>
                </div>

                {error && <div className={styles.error}>{error}</div>}
            </form>
        </section>
    );
};
