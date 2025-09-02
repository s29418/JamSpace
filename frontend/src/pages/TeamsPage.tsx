import React, { useEffect, useMemo, useRef, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';

import { getMyTeams, changeTeamPicture, createTeam } from '../services/teams.service';
import { fetchUserInvites, acceptInvite, rejectInvite } from '../services/teamInvites.service';

import TeamCard from '../components/TeamCard';
import TeamInviteCard from '../components/TeamInviteCard';
import styles from './TeamsPage.module.css';

import type { Team, TeamMember, TeamRoleLabel } from '../types/team';
import { isTeamRoleCode, isTeamRoleLabel, roleCodeToLabel } from '../types/team';

function normalizeMember(raw: any): TeamMember {
    const userId =
        raw?.userId ?? raw?.id ?? raw?.user?.id ?? raw?.user?.userId ?? '';

    const username =
        raw?.username ??
        raw?.userName ??
        raw?.user?.username ??
        raw?.user?.userName ??
        raw?.name ??
        raw?.user?.name ??
        '—';

    const avatarUrl =
        raw?.avatarUrl ??
        raw?.userPictureUrl ??
        raw?.pictureUrl ??
        raw?.profilePictureUrl ??
        raw?.user?.avatarUrl ??
        raw?.user?.pictureUrl ??
        null;

    const rawRole = raw?.role ?? raw?.user?.role;
    const maybeNum =
        typeof rawRole === 'string' && /^\d+$/.test(rawRole) ? Number(rawRole) : rawRole;

    const role: TeamRoleLabel =
        isTeamRoleCode(maybeNum)
            ? roleCodeToLabel[maybeNum]
            : isTeamRoleLabel(rawRole)
                ? rawRole
                : 'Member';

    const musicalRole =
        raw?.musicalRole ?? raw?.userMusicalRole ?? raw?.user?.musicalRole ?? null;

    return {
        userId,
        username,
        role,
        musicalRole,
        userPictureUrl: avatarUrl,
    };
}

function normalizeTeams(raw: any[]): Team[] {
    return (raw ?? []).map((t: any) => ({
        id: t.id,
        name: t.name,
        teamPictureUrl: t.teamPictureUrl ?? null,
        currentUserRole: isTeamRoleLabel(t?.currentUserRole) ? t.currentUserRole : 'Member',
        members: Array.isArray(t.members) ? t.members.map(normalizeMember) : [],
    }));
}


type UserInvite = {
    id: string;
    teamId: string;
    teamName: string;
    teamPictureUrl?: string | null;
    createdAt: string;
    invitedByUserName: string;
};

const TeamsPage: React.FC = () => {
    const navigate = useNavigate();

    const [teams, setTeams] = useState<Team[]>([]);
    const [invites, setInvites] = useState<UserInvite[]>([]);

    const [showForm, setShowForm] = useState(false);
    const [teamName, setTeamName] = useState('');
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [fileLabel, setFileLabel] = useState('Upload Image (optional)');
    const fileInputRef = useRef<HTMLInputElement>(null);

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

    /* —————— Fetch list (teams + invites) —————— */
    const fetchAll = useCallback(async () => {
        try {
            const [teamData, inviteData] = await Promise.all([getMyTeams(), fetchUserInvites()]);
            setTeams(normalizeTeams(teamData));
            setInvites(inviteData);
        } catch (err) {
            console.error('Failed to fetch teams or invites:', err);
            setErrorMsg('Failed to load data.');
        }
    }, [setErrorMsg]);

    useEffect(() => {
        fetchAll();
    }, [fetchAll]);

    /* —————— Create team (z opcjonalnym uploadem) —————— */
    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0] || null;
        setSelectedFile(file);
        setFileLabel(file?.name || 'No file chosen');
    };

    const bust = (url: string) => url + (url.includes('?') ? '&' : '?') + 'v=' + Date.now();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const nm = teamName.trim();
        if (nm.length < 4) return setErrorMsg('Name of the team must be at least 4 characters long.');
        if (nm.length > 25) return setErrorMsg('Name of the team must be less than 25 characters long.');

        try {
            // 1) create
            const createdRaw = await createTeam({ name: nm, teamPictureUrl: null });
            const [created] = normalizeTeams([createdRaw]);

            // 2) optimistic add
            setTeams((prev) => [created, ...prev]);

            // 3) optional image upload (cache-bust + event, jak w useTeamActions)
            if (selectedFile) {
                try {
                    const url: string = await changeTeamPicture(created.id, selectedFile);
                    const freshUrl = bust(url);
                    setTeams((prev) =>
                        prev.map((t) => (t.id === created.id ? { ...t, teamPictureUrl: freshUrl } : t)),
                    );
                    window.dispatchEvent(
                        new CustomEvent('team:updated', {
                            detail: { teamId: created.id, patch: { teamPictureUrl: freshUrl } },
                        }),
                    );
                } catch (err) {
                    console.warn('Image upload failed after team creation:', err);
                    // zostawiamy bez obrazka — UI i tak działa
                }
            }

            // 4) reset form
            setTeamName('');
            setSelectedFile(null);
            setShowForm(false);
            setErrorMessage('');
            setSuccess('The team has been successfully created');
        } catch (err: any) {
            console.error('Failed to create team:', err);
            setErrorMsg(err?.message ?? 'Failed to create team. Please try again.');
        }
    };

    /* —————— Invites —————— */
    const handleAccept = async (inviteId: string) => {
        try {
            await acceptInvite(inviteId);
            setInvites((prev) => prev.filter((i) => i.id !== inviteId));
            // zespół pojawi się na liście
            await fetchAll();
            setSuccess('Invitation accepted.');
        } catch (err: any) {
            console.error('Failed to accept invite:', err);
            setErrorMsg(err?.message ?? 'Failed to accept invite. Please try again.');
        }
    };

    const handleReject = async (inviteId: string) => {
        try {
            await rejectInvite(inviteId);
            setInvites((prev) => prev.filter((i) => i.id !== inviteId));
            setSuccess('Invitation rejected.');
        } catch (err) {
            console.error('Failed to reject invite:', err);
        }
    };

    /* —————— Render —————— */
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
                <button className={styles.createButton} onClick={() => setShowForm(true)}>
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
                            >
                                <span className={styles.uploadBtnText}>{fileLabel ?? 'Upload Image'}</span>
                            </button>

                            <input
                                ref={fileInputRef}
                                type="file"
                                accept="image/*"
                                onChange={handleFileChange}
                                className={styles.hiddenInput}
                            />
                        </div>

                        <button type="submit">Create</button>
                        <button type="button" onClick={() => setShowForm(false)}>
                            Cancel
                        </button>
                    </form>
                </div>
            </div>

            {successMessage && <p className={styles.successMessage}>{successMessage}</p>}
            {errorMessage && <p className={styles.message}>{errorMessage}</p>}

            {teams.map((team) => (
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
