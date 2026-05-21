import React, { useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { ArrowLeftIcon } from '@heroicons/react/24/outline';
import { useTeamProjectWorkspace } from 'features/team/team-projects/model/useTeamProjectWorkspace';
import { useProjectAudioVersionForm } from 'features/team/team-project-audio-versions/model/useProjectAudioVersionForm';
import { useProjectAudioVersionPlayback } from 'features/team/team-project-audio-versions/model/useProjectAudioVersionPlayback';
import { useProjectNoteForm } from 'features/team/team-project-notes/model/useProjectNoteForm';
import ConfirmDialog from 'shared/ui/confirm-dialog/ConfirmDialog';
import ProjectEditModal from 'widgets/team-projects/ui/ProjectEditModal';
import ProjectNotesPanel from 'features/team/team-project-notes/ui/ProjectNotesPanel';
import ProjectVersionsPanel from 'features/team/team-project-audio-versions/ui/ProjectVersionsPanel';
import ProjectHeader from 'widgets/team-project-header/ui/ProjectHeader';
import ProjectPlayerPanel from 'widgets/team-project-player/ui/ProjectPlayerPanel';
import styles from './TeamProjectDetailsPage.module.css';

const TeamProjectDetailsPage: React.FC = () => {
    const { teamId, projectId } = useParams<{ teamId: string; projectId: string }>();
    const navigate = useNavigate();
    const [editOpen, setEditOpen] = useState(false);
    const {
        project,
        versions,
        notes,
        selectedVersion,
        selectedVersionId,
        setSelectedVersionId,
        loading,
        error,
        updateProject,
        updateProjectPicture,
        removeProject,
        uploadVersion,
        removeVersion,
        addNote,
        updateNote,
        completeNote,
        reopenNote,
        removeNote,
    } = useTeamProjectWorkspace(teamId, projectId);

    const playback = useProjectAudioVersionPlayback({
        versions,
        notes,
        selectedVersion,
        selectedVersionId,
        setSelectedVersionId,
    });

    const audioVersionsForm = useProjectAudioVersionForm({
        uploadVersion,
        removeVersion,
    });

    const noteForm = useProjectNoteForm({
        versions,
        versionDurationSecondsById: playback.versionDurationSecondsById,
        selectedVersionId,
        getCurrentTimeSeconds: playback.getCurrentTimeSeconds,
        addNote,
        updateNote,
        completeNote,
        reopenNote,
        removeNote,
    });

    if (loading) {
        return <main className={styles.page}><p className={styles.state}>Loading project...</p></main>;
    }

    if (error) {
        return <main className={styles.page}><p className={styles.error}>{error}</p></main>;
    }

    if (!project || !teamId) {
        return <main className={styles.page}><p className={styles.state}>Project not found.</p></main>;
    }

    return (
        <main className={styles.page}>
            <Link to={`/teams/${teamId}`} className={styles.backLink}>
                <ArrowLeftIcon width={18} height={18} />
                Team
            </Link>

            <ProjectHeader project={project} onEdit={() => setEditOpen(true)} />

            <ProjectEditModal
                isOpen={editOpen}
                project={project}
                onClose={() => setEditOpen(false)}
                onSave={async ({ name, description, picture }) => {
                    await updateProject({ name, description });
                    if (picture) {
                        await updateProjectPicture(picture);
                    }
                }}
                onDelete={async () => {
                    await removeProject();
                    navigate(`/teams/${teamId}`);
                }}
            />

            <ConfirmDialog
                isOpen={!!audioVersionsForm.versionToDelete}
                title="Delete version"
                message={`Are you sure you want to delete "${audioVersionsForm.versionToDelete?.name ?? 'this version'}"?`}
                confirmLabel="Delete"
                loading={audioVersionsForm.deleting}
                onConfirm={audioVersionsForm.deleteSelectedVersion}
                onCancel={audioVersionsForm.cancelDeleteVersion}
            />

            <ConfirmDialog
                isOpen={!!noteForm.noteToDelete}
                title="Delete note"
                message="Are you sure you want to delete this note?"
                confirmLabel="Delete"
                loading={noteForm.updatingNoteId === noteForm.noteToDelete?.id}
                onConfirm={noteForm.deleteSelectedNote}
                onCancel={noteForm.cancelDeleteNote}
            />

            <section className={styles.workspace}>
                <div className={styles.mainColumn}>
                    <ProjectPlayerPanel
                        selectedVersion={selectedVersion}
                        artworkUrl={project.pictureUrl}
                        waveformPeaks={playback.selectedWaveformCache?.peaks}
                        waveformDuration={playback.selectedWaveformCache?.duration}
                        resumeTimeSeconds={playback.versionResume.timeSeconds}
                        shouldAutoPlay={playback.versionResume.shouldAutoPlay}
                        currentPlayerSecond={playback.currentPlayerSecond}
                        visibleDynamicNotes={playback.visibleDynamicNotes}
                        dynamicNotesCount={playback.dynamicTimestampNotes.length}
                        canShowDynamicNoteControls={playback.canShowDynamicNoteControls}
                        canGoToPreviousDynamicNotes={playback.canGoToPreviousDynamicNotes}
                        canGoToNextDynamicNotes={playback.canGoToNextDynamicNotes}
                        showOnlySelectedVersionNotes={playback.showOnlySelectedVersionNotes}
                        onTimeUpdate={playback.handlePlayerTimeUpdate}
                        onPlaybackStateChange={playback.handlePlayerPlaybackStateChange}
                        onToggleOnlySelectedVersionNotes={playback.toggleOnlySelectedVersionNotes}
                        onPreviousDynamicNotes={playback.showPreviousDynamicNotes}
                        onNextDynamicNotes={playback.showNextDynamicNotes}
                    />

                    <ProjectNotesPanel
                        notes={notes}
                        versions={versions}
                        formOpen={noteForm.formOpen}
                        editingNote={noteForm.editingNote}
                        content={noteForm.content}
                        audioVersionId={noteForm.audioVersionId}
                        attachCurrentTime={noteForm.attachCurrentTime}
                        startTime={noteForm.startTime}
                        endTime={noteForm.endTime}
                        error={noteForm.error}
                        saving={noteForm.saving}
                        updatingNoteId={noteForm.updatingNoteId}
                        onOpenCreate={noteForm.openCreateForm}
                        onSubmit={noteForm.saveNote}
                        onContentChange={noteForm.setContent}
                        onAudioVersionChange={noteForm.setAudioVersionId}
                        onAttachCurrentTimeChange={noteForm.setAttachCurrentTime}
                        onStartTimeChange={noteForm.setStartTime}
                        onEndTimeChange={noteForm.setEndTime}
                        onUsePlayerTime={noteForm.setCurrentTimeRange}
                        onCloseForm={noteForm.closeForm}
                        onToggleStatus={noteForm.toggleStatus}
                        onEdit={noteForm.openEditForm}
                        onDelete={noteForm.requestDeleteNote}
                    />
                </div>

                <ProjectVersionsPanel
                    versions={versions}
                    selectedVersionId={selectedVersionId}
                    isFormOpen={audioVersionsForm.isFormOpen}
                    name={audioVersionsForm.name}
                    file={audioVersionsForm.file}
                    error={audioVersionsForm.error}
                    saving={audioVersionsForm.saving}
                    fileInputRef={audioVersionsForm.fileInputRef}
                    onToggleForm={audioVersionsForm.toggleForm}
                    onNameChange={audioVersionsForm.setName}
                    onFileChange={audioVersionsForm.setFile}
                    onSubmit={audioVersionsForm.submit}
                    onCancel={audioVersionsForm.resetForm}
                    onSelectVersion={playback.selectVersionAtCurrentPlayback}
                    onDeleteVersion={audioVersionsForm.requestDeleteVersion}
                />
            </section>
        </main>
    );
};

export default TeamProjectDetailsPage;
