import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import WaveSurfer from 'wavesurfer.js';
import { AudioWaveformPeaks } from 'entities/post/ui/PostAudioPlayer';
import type { ProjectAudioVersion, ProjectNote } from 'entities/team/model/types';
import { toMediaProxyUrl } from 'shared/api/media';

const PRELOAD_VERSION_LIMIT = 6;
const DYNAMIC_NOTES_PAGE_SIZE = 3;

type WaveformCacheEntry = {
    peaks: AudioWaveformPeaks;
    duration: number;
};

type UseProjectAudioVersionPlaybackParams = {
    versions: ProjectAudioVersion[];
    notes: ProjectNote[];
    selectedVersion?: ProjectAudioVersion | null;
    selectedVersionId?: string | null;
    setSelectedVersionId: (versionId: string) => void;
};

export const useProjectAudioVersionPlayback = ({
    versions,
    notes,
    selectedVersion,
    selectedVersionId,
    setSelectedVersionId,
}: UseProjectAudioVersionPlaybackParams) => {
    const playbackRef = useRef({ currentTime: 0, isPlaying: false });
    const [currentPlayerSecond, setCurrentPlayerSecond] = useState(0);
    const [versionResume, setVersionResume] = useState({ timeSeconds: 0, shouldAutoPlay: false });
    const [waveformCache, setWaveformCache] = useState<Record<string, WaveformCacheEntry>>({});
    const [dynamicNotesPage, setDynamicNotesPage] = useState(0);

    const dynamicTimestampNotes = useMemo(
        () => notes.filter(note => {
            if (note.status !== 'Active') return false;
            if (note.startTimeSeconds === null || note.startTimeSeconds === undefined) return false;

            const start = note.startTimeSeconds;
            const end = note.endTimeSeconds ?? note.startTimeSeconds;
            const min = Math.min(start, end);
            const max = Math.max(start, end);

            return currentPlayerSecond >= min && currentPlayerSecond <= max;
        }),
        [currentPlayerSecond, notes]
    );

    const visibleDynamicNotes = useMemo(
        () => dynamicTimestampNotes.slice(
            dynamicNotesPage * DYNAMIC_NOTES_PAGE_SIZE,
            dynamicNotesPage * DYNAMIC_NOTES_PAGE_SIZE + DYNAMIC_NOTES_PAGE_SIZE
        ),
        [dynamicNotesPage, dynamicTimestampNotes]
    );

    const dynamicNotesPageCount = Math.ceil(dynamicTimestampNotes.length / DYNAMIC_NOTES_PAGE_SIZE);
    const canShowDynamicNoteControls = dynamicNotesPageCount > 1;
    const canGoToPreviousDynamicNotes = dynamicNotesPage > 0;
    const canGoToNextDynamicNotes = dynamicNotesPage < dynamicNotesPageCount - 1;

    useEffect(() => {
        setDynamicNotesPage(current => Math.min(current, Math.max(dynamicNotesPageCount - 1, 0)));
    }, [dynamicNotesPageCount]);

    useEffect(() => {
        let cancelled = false;
        const preloadedIds = new Set(Object.keys(waveformCache));
        const versionsToPreload = versions
            .filter(version => !preloadedIds.has(version.id))
            .slice(0, Math.max(PRELOAD_VERSION_LIMIT - preloadedIds.size, 0));

        if (versionsToPreload.length === 0) return;

        const preloadVersion = (version: ProjectAudioVersion) => new Promise<WaveformCacheEntry | null>((resolve) => {
            const container = document.createElement('div');
            container.style.position = 'fixed';
            container.style.left = '-10000px';
            container.style.top = '-10000px';
            container.style.width = '640px';
            container.style.height = '78px';
            document.body.appendChild(container);

            const wavesurfer = WaveSurfer.create({
                container,
                url: toMediaProxyUrl(version.fileUrl) ?? version.fileUrl,
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
                interact: false,
                hideScrollbar: true,
                autoScroll: false,
            });

            const cleanup = () => {
                wavesurfer.destroy();
                container.remove();
            };

            const unsubscribeReady = wavesurfer.on('ready', (duration) => {
                unsubscribeReady();
                unsubscribeError();

                const peaks = wavesurfer.exportPeaks({ maxLength: 12000 });
                cleanup();
                resolve({ peaks, duration });
            });

            const unsubscribeError = wavesurfer.on('error', () => {
                unsubscribeReady();
                unsubscribeError();
                cleanup();
                resolve(null);
            });
        });

        (async () => {
            for (const version of versionsToPreload) {
                if (cancelled) break;

                const entry = await preloadVersion(version);
                if (!entry || cancelled) continue;

                setWaveformCache(prev => (
                    prev[version.id] ? prev : { ...prev, [version.id]: entry }
                ));
            }
        })();

        return () => {
            cancelled = true;
        };
    }, [versions, waveformCache]);

    const handlePlayerTimeUpdate = useCallback((seconds: number) => {
        playbackRef.current.currentTime = seconds;
        const nextSecond = Math.floor(seconds);
        setCurrentPlayerSecond(current => current === nextSecond ? current : nextSecond);
    }, []);

    const handlePlayerPlaybackStateChange = useCallback((isPlaying: boolean) => {
        playbackRef.current.isPlaying = isPlaying;
    }, []);

    const selectVersionAtCurrentPlayback = useCallback((versionId: string) => {
        if (versionId === selectedVersionId) return;

        setVersionResume({
            timeSeconds: playbackRef.current.currentTime,
            shouldAutoPlay: playbackRef.current.isPlaying,
        });
        setSelectedVersionId(versionId);
    }, [selectedVersionId, setSelectedVersionId]);

    const selectedWaveformCache = selectedVersion ? waveformCache[selectedVersion.id] : undefined;

    return {
        currentPlayerSecond,
        versionResume,
        selectedWaveformCache,
        dynamicTimestampNotes,
        visibleDynamicNotes,
        canShowDynamicNoteControls,
        canGoToPreviousDynamicNotes,
        canGoToNextDynamicNotes,
        getCurrentTimeSeconds: () => playbackRef.current.currentTime,
        handlePlayerTimeUpdate,
        handlePlayerPlaybackStateChange,
        selectVersionAtCurrentPlayback,
        showPreviousDynamicNotes: () => setDynamicNotesPage(current => Math.max(current - 1, 0)),
        showNextDynamicNotes: () => setDynamicNotesPage(current => Math.min(current + 1, dynamicNotesPageCount - 1)),
    };
};
