import React, { useEffect, useId, useRef, useState } from 'react';
import {
    PauseIcon,
    PlayIcon,
    SpeakerWaveIcon,
    SpeakerXMarkIcon,
    ArrowPathRoundedSquareIcon,
    ArrowDownTrayIcon,
} from '@heroicons/react/24/solid';
import WaveSurfer from 'wavesurfer.js';
import styles from './PostAudioPlayer.module.css';

type Props = {
    src: string;
};

const AUDIO_PLAY_EVENT = 'jamspace:audio-player:play';
const PLAYBACK_RATES = [0.5, 0.75, 1, 1.25, 1.5, 2];

function formatAudioTime(seconds: number) {
    if (!Number.isFinite(seconds) || seconds < 0) {
        return '0:00';
    }

    const totalSeconds = Math.floor(seconds);
    const minutes = Math.floor(totalSeconds / 60);
    const remainingSeconds = totalSeconds % 60;

    return `${minutes}:${String(remainingSeconds).padStart(2, '0')}`;
}

function formatPlaybackRate(rate: number) {
    return `${Number(rate.toFixed(2)).toString()}x`;
}

export const PostAudioPlayer: React.FC<Props> = ({ src }) => {
    const waveformRef = useRef<HTMLDivElement | null>(null);
    const wavesurferRef = useRef<WaveSurfer | null>(null);
    const rateMenuRef = useRef<HTMLDivElement | null>(null);
    const playerId = useId();
    const [isReady, setIsReady] = useState(false);
    const [isPlaying, setIsPlaying] = useState(false);
    const [currentTime, setCurrentTime] = useState(0);
    const [duration, setDuration] = useState(0);
    const [volume, setVolume] = useState(0.9);
    const [isMuted, setIsMuted] = useState(false);
    const [playbackRate, setPlaybackRate] = useState(1);
    const [isRateMenuOpen, setIsRateMenuOpen] = useState(false);
    const [isLoopEnabled, setIsLoopEnabled] = useState(false);
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
        setVolume(0.9);
        setIsMuted(false);
        setPlaybackRate(1);
        setIsRateMenuOpen(false);
        setIsLoopEnabled(false);

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
        wavesurfer.setVolume(0.9);
        wavesurfer.setPlaybackRate(1);
        wavesurfer.getMediaElement().loop = false;

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

    useEffect(() => {
        function handleClickOutside(event: MouseEvent) {
            if (!rateMenuRef.current?.contains(event.target as Node)) {
                setIsRateMenuOpen(false);
            }
        }

        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

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

    function handleVolumeChange(event: React.ChangeEvent<HTMLInputElement>) {
        event.stopPropagation();

        const nextVolume = Number(event.target.value);
        const wavesurfer = wavesurferRef.current;

        setVolume(nextVolume);

        if (!wavesurfer) {
            return;
        }

        wavesurfer.setVolume(nextVolume);
        const muted = nextVolume === 0;
        wavesurfer.setMuted(muted);
        setIsMuted(muted);
    }

    function handleToggleMute(event: React.MouseEvent<HTMLButtonElement>) {
        event.stopPropagation();

        const wavesurfer = wavesurferRef.current;

        if (!wavesurfer || !isReady) {
            return;
        }

        const nextMuted = !isMuted;
        wavesurfer.setMuted(nextMuted);
        setIsMuted(nextMuted);
    }

    function updatePlaybackRate(nextRate: number) {
        const wavesurfer = wavesurferRef.current;

        if (!wavesurfer || !isReady) {
            return;
        }

        wavesurfer.setPlaybackRate(nextRate);
        setPlaybackRate(nextRate);
    }

    function handlePlaybackRateSliderChange(event: React.ChangeEvent<HTMLInputElement>) {
        event.stopPropagation();
        updatePlaybackRate(Number(event.target.value));
    }

    function handlePlaybackRatePresetChange(event: React.ChangeEvent<HTMLInputElement>) {
        event.stopPropagation();
        updatePlaybackRate(Number(event.target.value));
        setIsRateMenuOpen(false);
    }

    function handleToggleLoop(event: React.MouseEvent<HTMLButtonElement>) {
        event.stopPropagation();

        const wavesurfer = wavesurferRef.current;

        if (!wavesurfer || !isReady) {
            return;
        }

        const nextLoopState = !isLoopEnabled;
        wavesurfer.getMediaElement().loop = nextLoopState;
        setIsLoopEnabled(nextLoopState);
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

            <div className={styles.controlsRow}>
                <div className={styles.volumeGroup}>
                    <button
                        type="button"
                        className={`${styles.secondaryButton} ${isMuted ? styles.secondaryButtonActive : ''}`}
                        aria-label={isMuted ? 'Unmute audio' : 'Mute audio'}
                        onClick={handleToggleMute}
                        disabled={!isReady}
                    >
                        {isMuted || volume === 0 ? (
                            <SpeakerXMarkIcon width={18} height={18} />
                        ) : (
                            <SpeakerWaveIcon width={18} height={18} />
                        )}
                    </button>

                    <input
                        type="range"
                        min="0"
                        max="1"
                        step="0.01"
                        value={isMuted ? 0 : volume}
                        className={styles.volumeSlider}
                        aria-label="Audio volume"
                        onChange={handleVolumeChange}
                        onClick={(event) => event.stopPropagation()}
                        disabled={!isReady}
                    />
                </div>

                <div className={styles.utilityGroup}>
                    <div
                        className={styles.rateMenuWrap}
                        ref={rateMenuRef}
                        onClick={(event) => event.stopPropagation()}
                    >
                        <button
                            type="button"
                            className={`${styles.rateMenuButton} ${isRateMenuOpen ? styles.rateMenuButtonOpen : ''}`}
                            aria-haspopup="dialog"
                            aria-expanded={isRateMenuOpen}
                            onClick={() => setIsRateMenuOpen((current) => !current)}
                            disabled={!isReady}
                        >
                            {formatPlaybackRate(playbackRate)}
                        </button>

                        {isRateMenuOpen && (
                            <div className={styles.rateMenu}>
                                <div className={styles.rateSliderBlock}>
                                    <div className={styles.rateMenuLabel}>Tempo</div>
                                    <input
                                        type="range"
                                        min="0.5"
                                        max="2"
                                        step="0.05"
                                        value={playbackRate}
                                        className={styles.rateSlider}
                                        aria-label="Playback speed slider"
                                        onChange={handlePlaybackRateSliderChange}
                                    />
                                    <div className={styles.rateSliderLabels}>
                                        <span>0.5x</span>
                                        <span>{formatPlaybackRate(playbackRate)}</span>
                                        <span>2x</span>
                                    </div>
                                </div>

                                <div className={styles.ratePresetGrid} role="radiogroup" aria-label="Playback speed presets">
                                    {PLAYBACK_RATES.map((rate) => (
                                        <label
                                            key={rate}
                                            className={`${styles.ratePresetLabel} ${
                                                playbackRate === rate ? styles.ratePresetLabelActive : ''
                                            }`}
                                        >
                                            <input
                                                type="radio"
                                                name={`playback-rate-${playerId}`}
                                                value={rate}
                                                checked={playbackRate === rate}
                                                className={styles.ratePresetInput}
                                                onChange={handlePlaybackRatePresetChange}
                                            />
                                            <span>{formatPlaybackRate(rate)}</span>
                                        </label>
                                    ))}
                                </div>
                            </div>
                        )}
                    </div>

                    <button
                        type="button"
                        className={`${styles.secondaryButton} ${isLoopEnabled ? styles.secondaryButtonActive : ''}`}
                        aria-label={isLoopEnabled ? 'Disable loop' : 'Enable loop'}
                        onClick={handleToggleLoop}
                        disabled={!isReady}
                    >
                        <ArrowPathRoundedSquareIcon width={18} height={18} />
                    </button>

                    <a
                        href={src}
                        download
                        className={styles.secondaryLinkButton}
                        aria-label="Download audio"
                        onClick={(event) => event.stopPropagation()}
                    >
                        <ArrowDownTrayIcon width={18} height={18} />
                    </a>
                </div>
            </div>

            {error && <p className={styles.error}>{error}</p>}
        </section>
    );
};
