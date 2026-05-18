import React, { useEffect, useId, useState } from 'react';
import {
    AddExternalPortfolioTrackRequest,
    UploadPortfolioTrackRequest
} from '../../../entities/portfolio-track/api/portfolioTracks.api';
import { PlatformLogo } from '../../../shared/ui/platform-logo/PlatformLogo';
import {
    resolveExternalTrackEmbedUrl,
    resolveExternalTrackTitle
} from '../../../entities/portfolio-track/lib/externalTrackEmbed';
import { MusicalNoteIcon } from '@heroicons/react/24/outline';
import styles from './PortfolioTracksSection.module.css';

type Props = {
    onAddExternalTrack: (request: AddExternalPortfolioTrackRequest) => Promise<void> | void;
    onUploadTrack: (request: UploadPortfolioTrackRequest) => Promise<void> | void;
};

type AddMode = 'Upload' | 'Spotify' | 'SoundCloud';

const AUDIO_TYPES = 'audio/mpeg,audio/wav,audio/ogg';
const IMAGE_TYPES = 'image/jpeg,image/png';
const MODES: AddMode[] = ['Upload', 'Spotify', 'SoundCloud'];

function renderModeIcon(mode: AddMode) {
    if (mode === 'Spotify' || mode === 'SoundCloud') {
        return <PlatformLogo provider={mode} size={22} />;
    }

    return <MusicalNoteIcon width={22} height={22} />;
}

export const AddPortfolioTrackForm: React.FC<Props> = ({
    onAddExternalTrack,
    onUploadTrack,
}) => {
    const audioInputId = useId();
    const artworkInputId = useId();
    const [isExpanded, setIsExpanded] = useState(false);
    const [mode, setMode] = useState<AddMode>('Upload');
    const [externalUrl, setExternalUrl] = useState('');
    const [title, setTitle] = useState('');
    const [file, setFile] = useState<File | null>(null);
    const [artworkFile, setArtworkFile] = useState<File | null>(null);
    const [artworkPreviewUrl, setArtworkPreviewUrl] = useState<string | null>(null);
    const [busy, setBusy] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const canSubmit = mode === 'Upload'
        ? Boolean(title.trim() && file)
        : Boolean(externalUrl.trim());

    useEffect(() => {
        if (!artworkFile) {
            setArtworkPreviewUrl(null);
            return;
        }

        const objectUrl = URL.createObjectURL(artworkFile);
        setArtworkPreviewUrl(objectUrl);

        return () => URL.revokeObjectURL(objectUrl);
    }, [artworkFile]);

    function resetForm() {
        setExternalUrl('');
        setTitle('');
        setFile(null);
        setArtworkFile(null);
        setError(null);
    }

    async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();

        if (!canSubmit || busy) return;

        setBusy(true);
        setError(null);

        try {
            if (mode === 'Upload') {
                if (!file) return;

                await onUploadTrack({
                    title: title.trim(),
                    file,
                    artworkFile,
                });
            } else {
                const embedUrl = resolveExternalTrackEmbedUrl(mode, externalUrl);
                if (!embedUrl) {
                    throw new Error(`Paste a valid ${mode} track link.`);
                }

                await onAddExternalTrack({
                    source: mode,
                    title: resolveExternalTrackTitle(mode, externalUrl),
                    embedUrl,
                    externalUrl: externalUrl.trim(),
                });
            }

            resetForm();
            setIsExpanded(false);
            setMode('Upload');
        } catch (e) {
            setError(e instanceof Error ? e.message : 'Failed to add track.');
        } finally {
            setBusy(false);
        }
    }

    return (
        <section className={styles.addFormShell}>
            {!isExpanded ? (
                <button
                    type="button"
                    className={styles.addTrackButton}
                    onClick={() => setIsExpanded(true)}
                >
                    Add track
                </button>
            ) : (
                <form className={styles.addForm} onSubmit={handleSubmit}>
                    <div className={styles.formHeader}>
                        <h3>Add track</h3>
                        <button
                            type="button"
                            className={styles.formCancel}
                            onClick={() => {
                                setIsExpanded(false);
                                resetForm();
                            }}
                            disabled={busy}
                        >
                            Cancel
                        </button>
                    </div>

                    <div className={styles.sourcePicker}>
                        {MODES.map((option) => (
                            <label
                                key={option}
                                className={`${styles.sourceOption} ${mode === option ? styles.sourceOptionActive : ''}`}
                            >
                                <input
                                    type="radio"
                                    name="portfolio-track-add-mode"
                                    value={option}
                                    checked={mode === option}
                                    onChange={() => {
                                        setMode(option);
                                        setError(null);
                                    }}
                                    disabled={busy}
                                />
                                {renderModeIcon(option)}
                                <span>{option}</span>
                            </label>
                        ))}
                    </div>

                    {mode === 'Upload' ? (
                        <>
                            <input
                                className={styles.input}
                                value={title}
                                onChange={(event) => setTitle(event.target.value)}
                                placeholder="Track title"
                                disabled={busy}
                            />

                            <div className={styles.filePickerRow}>
                                <input
                                    id={audioInputId}
                                    className={styles.hiddenFileInput}
                                    type="file"
                                    accept={AUDIO_TYPES}
                                    onChange={(event) => {
                                        setFile(event.target.files?.[0] ?? null);
                                        setError(null);
                                    }}
                                    disabled={busy}
                                />
                                <label className={styles.filePickerButton} htmlFor={audioInputId}>
                                    Audio file
                                </label>
                                <span className={styles.fileName}>{file?.name ?? 'MP3, WAV or OGG'}</span>
                            </div>

                            <div className={styles.filePickerRow}>
                                <input
                                    id={artworkInputId}
                                    className={styles.hiddenFileInput}
                                    type="file"
                                    accept={IMAGE_TYPES}
                                    onChange={(event) => {
                                        setArtworkFile(event.target.files?.[0] ?? null);
                                        setError(null);
                                    }}
                                    disabled={busy}
                                />
                                <label className={styles.filePickerButton} htmlFor={artworkInputId}>
                                    Artwork
                                </label>
                                <span className={styles.fileName}>{artworkFile?.name ?? 'Optional JPG or PNG'}</span>
                            </div>

                            {artworkPreviewUrl && (
                                <div className={styles.artworkPreview}>
                                    <img src={artworkPreviewUrl} alt="" />
                                    <span>Artwork selected</span>
                                </div>
                            )}
                        </>
                    ) : (
                        <input
                            className={styles.input}
                            value={externalUrl}
                            onChange={(event) => setExternalUrl(event.target.value)}
                            placeholder={`${mode} track link`}
                            disabled={busy}
                        />
                    )}

                    {error && <p className={styles.formError}>{error}</p>}

                    <div className={styles.formActions}>
                        <button
                            type="submit"
                            className={styles.submitButton}
                            disabled={!canSubmit || busy}
                        >
                            {busy ? 'Adding...' : 'Add to portfolio'}
                        </button>
                    </div>
                </form>
            )}
        </section>
    );
};
