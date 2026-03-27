import React, { useRef, useState, useCallback, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';

import { useMyTeams } from '../../../features/team/my-teams/model/useMyTeams';
import { useTeamInvites } from '../../../features/team/team-invites/model/useTeamInvites';
import { useCreateTeam } from '../../../features/team/create-team/model/useCreateTeam';

import TeamCard from '../../../entities/team/ui/TeamCard';
import TeamInviteCard from '../../../entities/team/ui/TeamInviteCard';
import TeamCardSkeleton from '../../../entities/team/ui/TeamCardSkeleton';
import TeamSettingsModal from '../../../widgets/team-settings/ui/TeamSettingsModal';
import { getCurrentUserId } from '../../../shared/lib/auth/token';
import styles from './TeamsPage.module.css';

const TeamsPage: React.FC = () => {
    const navigate = useNavigate();
    const currentUserId = useMemo(() => getCurrentUserId(), []);

    const {
        teams,
        loading: teamsLoading,
        error: teamsError,
        refresh: refreshTeams,
    } = useMyTeams();

    const {
        invites,
        error: invitesError,
        refresh: refreshInvites,
        acceptInvite,
        rejectInvite,
    } = useTeamInvites();

    const { create, loading: creating, error: createError } = useCreateTeam({
        onDone: async () => {
            await Promise.all([refreshTeams(), refreshInvites()]);
        },
    });

    const [showForm, setShowForm] = useState(false);
    const [teamName, setTeamName] = useState('');
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [fileLabel, setFileLabel] = useState('Upload Image (optional)');
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [successMessage, setSuccessMessage] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const [settingsTeamId, setSettingsTeamId] = useState<string | null>(null);

    const setSuccess = useCallback((msg: string) => {
        setSuccessMessage(msg);
        window.setTimeout(() => setSuccessMessage(''), 7000);
    }, []);

    const setErrorMsg = useCallback((msg: string) => {
        setErrorMessage(msg);
        window.setTimeout(() => setErrorMessage(''), 7000);
    }, []);

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0] || null;
        setSelectedFile(file);
        setFileLabel(file?.name || 'No file chosen');
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const nm = teamName.trim();

        if (nm.length < 4) return setErrorMsg('Name of the team must be at least 4 characters long.');
        if (nm.length > 25) return setErrorMsg('Name of the team must be less than 25 characters long.');

        try {
            await create(nm, selectedFile);
            setTeamName('');
            setSelectedFile(null);
            setFileLabel('Upload Image (optional)');
            setShowForm(false);
            setErrorMessage('');
            setSuccess('The team has been successfully created');
        } catch (err: any) {
            setErrorMsg(err?.message ?? 'Failed to create team. Please try again.');
        }
    };

    const handleAccept = async (inviteId: string) => {
        try {
            await acceptInvite(inviteId);
            await refreshTeams();
            setSuccess('Invitation accepted.');
        } catch (err: any) {
            setErrorMsg(err?.message ?? 'Failed to accept invite. Please try again.');
        }
    };

    const handleReject = async (inviteId: string) => {
        try {
            await rejectInvite(inviteId);
            setSuccess('Invitation rejected.');
        } catch (err: any) {
            setErrorMsg(err?.message ?? 'Failed to reject invite. Please try again.');
        }
    };

    return (
        <div className={styles.wrapper}>
            {invites.length > 0 && (
                <div className={styles.inviteSection}>
                    <h1 className={styles.title}>Team Invites</h1>

                    {invites.map((invite) => (
                        <TeamInviteCard
                            key={invite.id}
                            id={invite.id}
                            name={invite.teamName}
                            teamPictureUrl={invite.teamPictureUrl}
                            onAccept={handleAccept}
                            onReject={handleReject}
                        />
                    ))}

                    <hr className={styles.lineBreak} />
                </div>
            )}

            <div className={styles.header}>
                <h1 className={styles.title}>Teams</h1>
                <button
                    className={styles.createButton}
                    onClick={() => setShowForm(true)}
                    disabled={creating}
                    type="button"
                >
                    + Create new team
                </button>
            </div>

            <div
                className={`${styles.expandable} ${showForm ? styles.expanded : ''}`}
                aria-hidden={!showForm}
            >
                <div className={styles.expandInner}>
                    <form className={styles.form} onSubmit={handleSubmit}>
                        <input
                            type="text"
                            value={teamName}
                            onChange={(e) => setTeamName(e.target.value)}
                            placeholder="Team name"
                            required
                            className={styles.teamsName}
                            disabled={creating}
                        />

                        <div className={styles.customFileUpload}>
                            <button
                                type="button"
                                className={styles.uploadBtn}
                                onClick={() => fileInputRef.current?.click()}
                                onKeyDown={(e) => {
                                    if (e.key === 'Enter' || e.key === ' ') fileInputRef.current?.click();
                                }}
                                title={fileLabel ?? 'Upload Image'}
                                aria-live="polite"
                                disabled={creating}
                            >
                                <span className={styles.uploadBtnText}>{fileLabel ?? 'Upload Image'}</span>
                            </button>

                            <input
                                ref={fileInputRef}
                                type="file"
                                accept="image/*"
                                onChange={handleFileChange}
                                className={styles.hiddenInput}
                                disabled={creating}
                            />
                        </div>

                        <button type="submit" disabled={creating}>Create</button>
                        <button type="button" onClick={() => setShowForm(false)} disabled={creating}>
                            Cancel
                        </button>
                    </form>
                </div>
            </div>

            {successMessage && <p className={styles.successMessage}>{successMessage}</p>}

            {(errorMessage || createError || teamsError || invitesError) && (
                <p className={styles.message}>
                    {errorMessage || createError || teamsError || invitesError}
                </p>
            )}

            {teamsLoading && (
                <>
                    <TeamCardSkeleton />
                    <TeamCardSkeleton />
                    <TeamCardSkeleton />
                </>
            )}

            {!teamsLoading &&
                teams.map((team) => (
                    <TeamCard
                        key={team.id}
                        id={team.id}
                        name={team.name}
                        teamPictureUrl={team.teamPictureUrl}
                        members={team.members}
                        onClick={() => navigate(`/teams/${team.id}`)}
                        onOpenSettings={setSettingsTeamId}
                    />
                ))}

            {settingsTeamId && (
                <TeamSettingsModal
                    isOpen={true}
                    teamId={settingsTeamId}
                    currentUserId={currentUserId ?? ''}
                    onClose={async () => {
                        setSettingsTeamId(null);
                    }}
                    onTeamDeleted={async () => {
                        setSettingsTeamId(null);
                        await refreshTeams();
                    }}
                    onLeftTeam={async () => {
                        setSettingsTeamId(null);
                        await refreshTeams();
                    }}
                />
            )}
        </div>
    );
};

export default TeamsPage;