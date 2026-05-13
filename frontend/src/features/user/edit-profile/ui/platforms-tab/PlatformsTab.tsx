import React, { FC, useCallback, useEffect, useMemo, useState } from 'react';
import {
    disconnectExternalAccount,
    ExternalMusicProvider,
    getMyExternalAccounts,
    startExternalAccountConnection,
    UserExternalAccount,
} from '../../../../../entities/user/api/externalAccounts.api';
import { isApiError } from '../../../../../shared/api/base';
import styles from '../EditProfilePanel.module.css';

type Props = {
    refreshKey?: number;
};

export const PlatformsTab: FC<Props> = ({ refreshKey = 0 }) => {
    const [accounts, setAccounts] = useState<UserExternalAccount[]>([]);
    const [loading, setLoading] = useState(false);
    const [busyProvider, setBusyProvider] = useState<ExternalMusicProvider | null>(null);
    const [error, setError] = useState<string | null>(null);

    const accountByProvider = useMemo(
        () => new Map(accounts.map((account) => [account.provider, account])),
        [accounts],
    );

    const loadAccounts = useCallback(async () => {
        setLoading(true);
        setError(null);

        try {
            setAccounts(await getMyExternalAccounts());
        } catch (e) {
            setError(isApiError(e) ? e.message : 'Failed to load connected platforms.');
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        void loadAccounts();
    }, [loadAccounts, refreshKey]);

    async function connectSpotify() {
        setBusyProvider('Spotify');
        setError(null);

        try {
            const returnUrl = `${window.location.origin}/profile?settingsTab=platforms`;
            const result = await startExternalAccountConnection('Spotify', returnUrl);
            window.location.assign(result.url);
        } catch (e) {
            setError(isApiError(e) ? e.message : 'Failed to start Spotify connection.');
            setBusyProvider(null);
        }
    }

    async function disconnect(provider: ExternalMusicProvider) {
        setBusyProvider(provider);
        setError(null);

        try {
            await disconnectExternalAccount(provider);
            await loadAccounts();
        } catch (e) {
            setError(isApiError(e) ? e.message : `Failed to disconnect ${provider}.`);
        } finally {
            setBusyProvider(null);
        }
    }

    const spotify = accountByProvider.get('Spotify');

    return (
        <div className={styles.platformsTab}>
            {loading && <p className={styles.help}>Loading platforms...</p>}
            {error && <p className={`${styles.help} ${styles.error}`}>{error}</p>}

            <section className={styles.platformItem}>
                <div className={styles.platformMain}>
                    <div className={`${styles.platformIcon} ${styles.spotifyIcon}`}>S</div>
                    <div>
                        <h3>Spotify</h3>
                        {spotify ? (
                            <p className={styles.help}>Connected as {spotify.displayName}</p>
                        ) : (
                            <p className={styles.help}>Connect your Spotify profile.</p>
                        )}
                    </div>
                </div>

                {spotify ? (
                    <div className={styles.platformActions}>
                        <a
                            className={`${styles.button} ${styles.buttonGhost}`}
                            href={spotify.profileUrl}
                            target="_blank"
                            rel="noreferrer"
                        >
                            Open profile
                        </a>
                        <button
                            className={`${styles.button} ${styles.buttonGhost}`}
                            disabled={busyProvider === 'Spotify'}
                            onClick={() => void disconnect('Spotify')}
                            type="button"
                        >
                            Disconnect
                        </button>
                    </div>
                ) : (
                    <button
                        className={`${styles.button} ${styles.buttonPrimary}`}
                        disabled={busyProvider === 'Spotify'}
                        onClick={() => void connectSpotify()}
                        type="button"
                    >
                        {busyProvider === 'Spotify' ? 'Connecting...' : 'Connect Spotify'}
                    </button>
                )}
            </section>

            <section className={styles.platformItem}>
                <div className={styles.platformMain}>
                    <div className={`${styles.platformIcon} ${styles.soundCloudIcon}`}>SC</div>
                    <div>
                        <h3>SoundCloud</h3>
                        <p className={styles.help}>Temporarily unavailable.</p>
                    </div>
                </div>

                <button
                    className={`${styles.button} ${styles.buttonGhost}`}
                    disabled
                    type="button"
                >
                    Connect SoundCloud
                </button>
            </section>
        </div>
    );
};
