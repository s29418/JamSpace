import React, { useRef, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';

import { useMyTeams } from './model/useMyTeams';
import { useTeamInvites } from '../../features/team/inviteCard/model/useTeamInvites';
import { useCreateTeam } from './model/useCreateTeam';

import TeamCard from '../../features/team/teamCard/ui/TeamCard';
import TeamInviteCard from '../../features/team/inviteCard/ui/TeamInviteCard';
import styles from './TeamsPage.module.css';

const TeamsPage: React.FC = () => {
    const navigate = useNavigate();

    // Hooki – źródła prawdy dla list
    const {
        teams,
        loading: teamsLoading,
        error: teamsError,
        refresh: refreshTeams,
    } = useMyTeams();

    const {
        invites,
        loading: invitesLoading,
        error: invitesError,
        refresh: refreshInvites,
        acceptInvite,
        rejectInvite,
    } = useTeamInvites();

    // Tworzenie zespołu (odśwież po sukcesie obie listy)
    const { create, loading: creating, error: createError } = useCreateTeam({
        onDone: async () => {
            await Promise.all([refreshTeams(), refreshInvites()]);
        },
    });

    // UI (formularz)
    const [showForm, setShowForm] = useState(false);
    const [teamName, setTeamName] = useState('');
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [fileLabel, setFileLabel] = useState('Upload Image (optional)');
    const fileInputRef = useRef<HTMLInputElement>(null);

    // Komunikaty
    const [successMessage, setSuccessMessage] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const setSuccess = useCallback((msg: string) => {
        setSuccessMessage(msg);
        window.setTimeout(() => setSuccessMessage(''), 7000);
    }, []);
    const setErrorMsg = useCallback((msg: string) => {
        setErrorMessage(msg);
        window.setTimeout(() => setErrorMessage(''), 7000);
    }, []);

    // Zmiana pliku (avatar nowej drużyny)
    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0] || null;
        setSelectedFile(file);
        setFileLabel(file?.name || 'No file chosen');
    };

    // Tworzenie zespołu przez hook
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const nm = teamName.trim();
        if (nm.length < 4) return setErrorMsg('Name of the team must be at least 4 characters long.');
        if (nm.length > 25) return setErrorMsg('Name of the team must be less than 25 characters long.');

        try {
            await create(nm, selectedFile); // hook sam wrzuci obrazek i odświeży listy przez onDone
            setTeamName('');
            setSelectedFile(null);
            setShowForm(false);
            setErrorMessage('');
            setSuccess('The team has been successfully created');
        } catch (err: any) {
            // useCreateTeam już ustawia własny error, ale możemy dołożyć komunikat UI
            setErrorMsg(err?.message ?? 'Failed to create team. Please try again.');
        }
    };

    // Akcje na zaproszeniach (po akceptacji odświeżamy też listę drużyn)
    const handleAccept = async (inviteId: string) => {
        try {
            await acceptInvite(inviteId);     // hook sam odświeży listę zaproszeń
            await refreshTeams();             // nowa drużyna pojawi się na liście
            setSuccess('Invitation accepted.');
        } catch (err: any) {
            setErrorMsg(err?.message ?? 'Failed to accept invite. Please try again.');
        }
    };

    const handleReject = async (inviteId: string) => {
        try {
            await rejectInvite(inviteId);     // hook sam odświeży listę zaproszeń
            setSuccess('Invitation rejected.');
        } catch (err: any) {
            setErrorMsg(err?.message ?? 'Failed to reject invite. Please try again.');
        }
    };

    // Render
    return (
        <div className={styles.wrapper}>
            {/* Sekcja zaproszeń */}
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

            {/* Nagłówek + przycisk tworzenia */}
            <div className={styles.header}>
                <h1 className={styles.title}>Teams</h1>
                <button
                    className={styles.createButton}
                    onClick={() => setShowForm(true)}
                    disabled={creating}
                >
                    + Create new team
                </button>
            </div>

            {/* Formularz tworzenia */}
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

            {/* Komunikaty */}
            {successMessage && <p className={styles.successMessage}>{successMessage}</p>}
            {(errorMessage || createError || teamsError || invitesError) && (
                <p className={styles.message}>
                    {errorMessage || createError || teamsError || invitesError}
                </p>
            )}

            {/* Lista drużyn */}
            {teamsLoading && <p className={styles.message}>Loading teams…</p>}
            {!teamsLoading && teams.map((team) => (
                <TeamCard
                    key={team.id}
                    id={team.id}
                    name={team.name}
                    teamPictureUrl={team.teamPictureUrl}
                    members={team.members}
                    onClick={() => navigate(`/teams/${team.id}`)}
                />
            ))}
        </div>
    );
};

export default TeamsPage;
