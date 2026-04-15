import React, { useEffect, useId, useRef, useState } from 'react';
import { PauseIcon, PlayIcon } from '@heroicons/react/24/solid';
import WaveSurfer from 'wavesurfer.js';
import styles from './PostAudioPlayer.module.css';

type Props = {
    src: string;
};

const AUDIO_PLAY_EVENT = 'jamspace:audio-player:play';

function formatAudioTime(seconds: number) {
    if (!Number.isFinite(seconds) || seconds < 0) {
        return '0:00';
    }

    const totalSeconds = Math.floor(seconds);
    const minutes = Math.floor(totalSeconds / 60);
    const remainingSeconds = totalSeconds % 60;

    return `${minutes}:${String(remainingSeconds).padStart(2, '0')}`;
}

export const PostAudioPlayer: React.FC<Props> = ({ src }) => {
    const waveformRef = useRef<HTMLDivElement | null>(null);
    const wavesurferRef = useRef<WaveSurfer | null>(null);
    const playerId = useId();
    const [isReady, setIsReady] = useState(false);
    const [isPlaying, setIsPlaying] = useState(false);
    const [currentTime, setCurrentTime] = useState(0);
    const [duration, setDuration] = useState(0);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!waveformRef.current) {
            return;
        }

        setIsReady(false);
        setIsPlaying(false);
        setCurrentTime(0);
        setDuration(0);
        setError(null);

        const wavesurfer = WaveSurfer.create({
            container: waveformRef.current,
            url: src,
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
            dragToSeek: true,
            hideScrollbar: true,
            interact: true,
            autoScroll: false,
        });

        wavesurferRef.current = wavesurfer;

        const unsubscribeReady = wavesurfer.on('ready', (audioDuration) => {
            setIsReady(true);
            setDuration(audioDuration);
        });

        const unsubscribeTimeUpdate = wavesurfer.on('timeupdate', (time) => {
            setCurrentTime(time);
        });

        const unsubscribePlay = wavesurfer.on('play', () => {
            setIsPlaying(true);
            window.dispatchEvent(
                new CustomEvent(AUDIO_PLAY_EVENT, {
                    detail: { playerId },
                }),
            );
        });

        const unsubscribePause = wavesurfer.on('pause', () => {
            setIsPlaying(false);
        });

        const unsubscribeFinish = wavesurfer.on('finish', () => {
            setIsPlaying(false);
            setCurrentTime(wavesurfer.getDuration());
        });

        const unsubscribeError = wavesurfer.on('error', (waveError) => {
            setError(waveError.message || 'Failed to load audio.');
        });

        function handleExternalPlay(event: Event) {
            const customEvent = event as CustomEvent<{ playerId?: string }>;

            if (customEvent.detail?.playerId !== playerId && wavesurfer.isPlaying()) {
                wavesurfer.pause();
            }
        }

        window.addEventListener(AUDIO_PLAY_EVENT, handleExternalPlay);

        return () => {
            window.removeEventListener(AUDIO_PLAY_EVENT, handleExternalPlay);
            unsubscribeReady();
            unsubscribeTimeUpdate();
            unsubscribePlay();
            unsubscribePause();
            unsubscribeFinish();
            unsubscribeError();
            wavesurfer.destroy();
            wavesurferRef.current = null;
        };
    }, [playerId, src]);

    async function handleTogglePlayback(event: React.MouseEvent<HTMLButtonElement>) {
        event.stopPropagation();

        const wavesurfer = wavesurferRef.current;

        if (!wavesurfer || !isReady) {
            return;
        }

        try {
            await wavesurfer.playPause();
        } catch {
            setError('Failed to control audio playback.');
        }
    }

    return (
        <section
            className={styles.player}
            onClick={(event) => event.stopPropagation()}
        >
            <div className={styles.mainRow}>
                <button
                    type="button"
                    className={styles.playButton}
                    aria-label={isPlaying ? 'Pause audio' : 'Play audio'}
                    onClick={handleTogglePlayback}
                    disabled={!isReady}
                >
                    {isPlaying ? <PauseIcon width={20} height={20} /> : <PlayIcon width={20} height={20} />}
                </button>

                <div className={styles.waveformBlock}>
                    <div ref={waveformRef} className={styles.waveform} />

                    <div className={styles.metaRow}>
                        <span className={styles.timeValue}>{formatAudioTime(currentTime)}</span>
                        <span className={styles.timeDivider}>/</span>
                        <span className={styles.timeValueMuted}>
                            {isReady ? formatAudioTime(duration) : '--:--'}
                        </span>
                    </div>
                </div>
            </div>

            {error && <p className={styles.error}>{error}</p>}
        </section>
    );
};
