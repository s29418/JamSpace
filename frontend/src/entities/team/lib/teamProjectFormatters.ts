import type { ProjectNote } from 'entities/team/model/types';
import { isApiError } from 'shared/api/base';

export const formatDate = (value: string) =>
    new Intl.DateTimeFormat('en', { day: 'numeric', month: 'short' }).format(new Date(value));

export const formatTime = (seconds?: number | null) => {
    if (seconds === null || seconds === undefined) return null;

    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${String(remainingSeconds).padStart(2, '0')}`;
};

export const formatRange = (note: ProjectNote) => {
    const start = formatTime(note.startTimeSeconds);
    const end = formatTime(note.endTimeSeconds);

    if (!start) return null;
    return end ? `${start} - ${end}` : start;
};

export const getProjectFallback = (name?: string | null) => name?.trim().charAt(0).toUpperCase() || '?';

export const getErrorMessage = (error: unknown, fallback: string) =>
    isApiError(error) ? error.message : fallback;
