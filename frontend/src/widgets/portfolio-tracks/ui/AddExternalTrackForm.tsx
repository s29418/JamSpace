import React, { useState } from 'react';
import {
    AddExternalPortfolioTrackRequest
} from '../../../entities/portfolio-track/api/portfolioTracks.api';
import { PlatformLogo } from '../../../shared/ui/platform-logo/PlatformLogo';
import {
    resolveExternalTrackEmbedUrl,
    resolveExternalTrackTitle
} from '../../../entities/portfolio-track/lib/externalTrackEmbed';
import styles from './PortfolioTracksSection.module.css';

type Props = {
    onSubmit: (request: AddExternalPortfolioTrackRequest) => Promise<void> | void;
};

type Source = AddExternalPortfolioTrackRequest['source'];

const SOURCE_OPTIONS: Source[] = ['Spotify', 'SoundCloud'];

export const AddExternalTrackForm: React.FC<Props> = ({ onSubmit }) => {
    const [source, setSource] = useState<Source>('Spotify');
    const [externalUrl, setExternalUrl] = useState('');
    const [isExpanded, setIsExpanded] = useState(false);
    const [busy, setBusy] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const canSubmit = Boolean(externalUrl.trim());

    async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();

        if (!canSubmit || busy) return;

        setBusy(true);
        setError(null);

        try {
            const embedUrl = resolveExternalTrackEmbedUrl(source, externalUrl);
            if (!embedUrl) {
                throw new Error(`Paste a valid ${source} track link.`);
            }

            await onSubmit({
                source,
                title: resolveExternalTrackTitle(source, externalUrl),
                embedUrl,
                externalUrl: externalUrl.trim(),
            });

            setExternalUrl('');
            setIsExpanded(false);
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
                                setError(null);
                            }}
                            disabled={busy}
                        >
                            Cancel
                        </button>
                    </div>

                    <div className={styles.sourcePicker}>
                        {SOURCE_OPTIONS.map((option) => (
                            <label
                                key={option}
                                className={`${styles.sourceOption} ${source === option ? styles.sourceOptionActive : ''}`}
                            >
                                <input
                                    type="radio"
                                    name="portfolio-track-source"
                                    value={option}
                                    checked={source === option}
                                    onChange={() => setSource(option)}
                                    disabled={busy}
                                />
                                <PlatformLogo provider={option} size={22} />
                                <span>{option}</span>
                            </label>
                        ))}
                    </div>

                    <input
                        className={styles.input}
                        value={externalUrl}
                        onChange={(event) => setExternalUrl(event.target.value)}
                        placeholder={`${source} track link`}
                        disabled={busy}
                    />

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
