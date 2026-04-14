import React, { useEffect, useRef, useState } from 'react';
import Plyr from 'plyr';
import 'plyr/dist/plyr.css';
import plyrSprite from '../../../../node_modules/plyr/dist/plyr.svg';
import { ArrowsPointingOutIcon, PlayIcon } from '@heroicons/react/24/outline';
import styles from './PostVideoPlayer.module.css';

type Props = {
    src: string;
    autoPlay?: boolean;
    variant?: 'inline' | 'viewer';
    onOpenPreview?: () => void;
};

const controls = [
    'play-large',
    'play',
    'progress',
    'current-time',
    'duration',
    'mute',
    'volume',
    'settings',
    'pip',
    'fullscreen',
];

export const PostVideoPlayer: React.FC<Props> = ({
    src,
    autoPlay = false,
    variant = 'inline',
    onOpenPreview,
}) => {
    const playerHostRef = useRef<HTMLDivElement | null>(null);
    const videoRef = useRef<HTMLVideoElement | null>(null);
    const playerRef = useRef<any>(null);
    const [isPlaying, setIsPlaying] = useState(Boolean(autoPlay));

    useEffect(() => {
        if (!playerHostRef.current) {
            return;
        }

        setIsPlaying(Boolean(autoPlay));

        const hostElement = playerHostRef.current;
        const videoElement = document.createElement('video');
        videoElement.className = styles.videoElement;
        videoElement.src = src;
        videoElement.playsInline = true;
        videoElement.preload = 'metadata';
        videoElement.autoplay = autoPlay;
        videoElement.controls = true;
        hostElement.replaceChildren(videoElement);
        videoRef.current = videoElement;

        const syncPlayingState = () => setIsPlaying(!videoElement.paused && !videoElement.ended);

        videoElement.addEventListener('play', syncPlayingState);
        videoElement.addEventListener('pause', syncPlayingState);
        videoElement.addEventListener('ended', syncPlayingState);

        const player = new Plyr(videoElement, {
            controls,
            iconUrl: plyrSprite,
            loadSprite: true,
            settings: ['speed'],
            speed: {
                selected: 1,
                options: [0.5, 0.75, 1, 1.25, 1.5, 2],
            },
            tooltips: { controls: true, seek: true },
            keyboard: { focused: true, global: false },
            fullscreen: { enabled: true, iosNative: false },
            invertTime: false,
            ratio: variant === 'viewer' ? undefined : '16:9',
        });
        playerRef.current = player;

        if (autoPlay) {
            const playResult = player.play();

            if (playResult && typeof playResult.catch === 'function') {
                void playResult.catch(() => undefined);
            }
        }

        return () => {
            playerRef.current = null;
            videoElement.removeEventListener('play', syncPlayingState);
            videoElement.removeEventListener('pause', syncPlayingState);
            videoElement.removeEventListener('ended', syncPlayingState);
            player.destroy();
            hostElement.replaceChildren();
            videoRef.current = null;
        };
    }, [autoPlay, src, variant]);

    return (
        <div
            className={`${styles.videoShell} ${
                variant === 'viewer' ? styles.viewerShell : styles.inlineShell
            }`}
            onClick={(event) => event.stopPropagation()}
        >
            {variant === 'inline' && onOpenPreview && (
                <button
                    type="button"
                    className={styles.previewButton}
                    aria-label="Open video preview"
                    onClick={(event) => {
                        event.stopPropagation();
                        onOpenPreview();
                    }}
                >
                    <ArrowsPointingOutIcon width={18} height={18} />
                </button>
            )}

            {!isPlaying && (
                <button
                    type="button"
                    className={styles.centerPlayButton}
                    aria-label="Play video"
                    onClick={(event) => {
                        event.stopPropagation();
                        const videoElement = videoRef.current;

                        if (!videoElement) {
                            return;
                        }

                        const playResult = videoElement.play();

                        if (playResult && typeof playResult.catch === 'function') {
                            void playResult.catch(() => undefined);
                        }
                    }}
                >
                    <PlayIcon width={28} height={28} />
                </button>
            )}

            <div ref={playerHostRef} className={styles.playerMount} />
        </div>
    );
};
