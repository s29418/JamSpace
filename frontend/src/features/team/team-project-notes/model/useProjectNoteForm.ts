import React, { useState } from 'react';
import type { CreateProjectNoteRequest, EditProjectNoteRequest, ProjectNote } from 'entities/team/model/types';
import { getErrorMessage } from 'entities/team/lib/teamProjectFormatters';

type UseProjectNoteFormParams = {
    selectedVersionId?: string | null;
    getCurrentTimeSeconds: () => number;
    addNote: (request: CreateProjectNoteRequest) => Promise<ProjectNote>;
    updateNote: (noteId: string, request: EditProjectNoteRequest) => Promise<ProjectNote>;
    completeNote: (noteId: string) => Promise<ProjectNote>;
    reopenNote: (noteId: string) => Promise<ProjectNote>;
    removeNote: (noteId: string) => Promise<void>;
};

export const useProjectNoteForm = ({
    selectedVersionId,
    getCurrentTimeSeconds,
    addNote,
    updateNote,
    completeNote,
    reopenNote,
    removeNote,
}: UseProjectNoteFormParams) => {
    const [formOpen, setFormOpen] = useState(false);
    const [editingNote, setEditingNote] = useState<ProjectNote | null>(null);
    const [content, setContent] = useState('');
    const [audioVersionId, setAudioVersionId] = useState('');
    const [attachCurrentTime, setAttachCurrentTime] = useState(false);
    const [startTime, setStartTime] = useState('');
    const [endTime, setEndTime] = useState('');
    const [error, setError] = useState<string | null>(null);
    const [saving, setSaving] = useState(false);
    const [updatingNoteId, setUpdatingNoteId] = useState<string | null>(null);
    const [noteToDelete, setNoteToDelete] = useState<ProjectNote | null>(null);

    const setCurrentTimeRange = () => {
        const currentSecond = Math.floor(getCurrentTimeSeconds());
        setStartTime(String(currentSecond));
        setEndTime(String(currentSecond));
    };

    const openCreateForm = () => {
        const currentSecond = Math.floor(getCurrentTimeSeconds());

        setEditingNote(null);
        setContent('');
        setAudioVersionId(selectedVersionId ?? '');
        setAttachCurrentTime(false);
        setStartTime(String(currentSecond));
        setEndTime(String(currentSecond));
        setError(null);
        setFormOpen(true);
    };

    const openEditForm = (note: ProjectNote) => {
        const currentSecond = Math.floor(getCurrentTimeSeconds());

        setEditingNote(note);
        setContent(note.content);
        setAudioVersionId(note.audioVersionId ?? '');
        setAttachCurrentTime(note.startTimeSeconds !== null && note.startTimeSeconds !== undefined);
        setStartTime(note.startTimeSeconds !== null && note.startTimeSeconds !== undefined
            ? String(note.startTimeSeconds)
            : String(currentSecond));
        setEndTime(note.endTimeSeconds !== null && note.endTimeSeconds !== undefined
            ? String(note.endTimeSeconds)
            : String(note.startTimeSeconds ?? currentSecond));
        setError(null);
        setFormOpen(true);
    };

    const closeForm = () => {
        if (saving) return;

        setFormOpen(false);
        setEditingNote(null);
        setContent('');
        setAudioVersionId('');
        setAttachCurrentTime(false);
        setStartTime('');
        setEndTime('');
        setError(null);
    };

    const setAttachCurrentTimeValue = (checked: boolean) => {
        setAttachCurrentTime(checked);
        if (checked && !startTime) {
            setCurrentTimeRange();
        }
    };

    const saveNote = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        setError(null);

        const trimmedContent = content.trim();
        if (!trimmedContent) {
            setError('Note content is required.');
            return;
        }

        const parsedStartTime = Number(startTime);
        const parsedEndTime = Number(endTime || startTime);

        if (attachCurrentTime && (!Number.isInteger(parsedStartTime) || parsedStartTime < 0)) {
            setError('Start time must be a non-negative whole second.');
            return;
        }

        if (attachCurrentTime && (!Number.isInteger(parsedEndTime) || parsedEndTime < 0)) {
            setError('End time must be a non-negative whole second.');
            return;
        }

        const selectedAudioVersionId = audioVersionId || null;
        const timestampPayload = attachCurrentTime
            ? {
                audioVersionId: selectedAudioVersionId,
                startTimeSeconds: Math.min(parsedStartTime, parsedEndTime),
                endTimeSeconds: Math.max(parsedStartTime, parsedEndTime),
            }
            : {
                audioVersionId: selectedAudioVersionId,
                startTimeSeconds: null,
                endTimeSeconds: null,
            };

        try {
            setSaving(true);
            if (editingNote) {
                await updateNote(editingNote.id, {
                    content: trimmedContent,
                    ...timestampPayload,
                });
            } else {
                await addNote({
                    content: trimmedContent,
                    ...timestampPayload,
                });
            }
            closeForm();
        } catch (e) {
            setError(getErrorMessage(e, editingNote ? 'Failed to update note.' : 'Failed to add note.'));
        } finally {
            setSaving(false);
        }
    };

    const toggleStatus = async (note: ProjectNote) => {
        try {
            setUpdatingNoteId(note.id);
            if (note.status === 'Completed') {
                await reopenNote(note.id);
            } else {
                await completeNote(note.id);
            }
        } catch (e) {
            setError(getErrorMessage(e, 'Failed to update note status.'));
        } finally {
            setUpdatingNoteId(null);
        }
    };

    const deleteSelectedNote = async () => {
        if (!noteToDelete) return;

        try {
            setUpdatingNoteId(noteToDelete.id);
            await removeNote(noteToDelete.id);
            setNoteToDelete(null);
        } catch (e) {
            setError(getErrorMessage(e, 'Failed to delete note.'));
            setNoteToDelete(null);
        } finally {
            setUpdatingNoteId(null);
        }
    };

    return {
        formOpen,
        editingNote,
        content,
        audioVersionId,
        attachCurrentTime,
        startTime,
        endTime,
        error,
        saving,
        updatingNoteId,
        noteToDelete,
        openCreateForm,
        openEditForm,
        closeForm,
        saveNote,
        toggleStatus,
        requestDeleteNote: setNoteToDelete,
        cancelDeleteNote: () => {
            if (!updatingNoteId) setNoteToDelete(null);
        },
        deleteSelectedNote,
        setContent,
        setAudioVersionId,
        setAttachCurrentTime: setAttachCurrentTimeValue,
        setStartTime,
        setEndTime,
        setCurrentTimeRange,
    };
};
