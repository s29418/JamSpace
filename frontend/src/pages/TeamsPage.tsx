import React, {useEffect, useRef, useState} from 'react';
import { useNavigate } from 'react-router-dom';
import {
    getMyTeams,
    changeTeamPicture,
    createTeam,
} from '../services/teams.service';

import {
    fetchUserInvites,
    acceptInvite,
    rejectInvite
} from '../services/teamInvites.service';
import TeamCard from '../components/TeamCard';
import TeamInviteCard from '../components/TeamInviteCard';
import styles from './TeamsPage.module.css';

interface Member {
    userId: string;
    username: string;
    role: string;
}

interface Team {
    id: string;
    name: string;
    teamPictureUrl?: string;
    members: Member[];
}

interface TeamInvite {
    id: string;
    teamId: string;
    teamName: string;
    teamPictureUrl?: string;
    createdAt: string;
    invitedByUserName: string;
}

const TeamsPage = () => {
    const [teams, setTeams] = useState<Team[]>([]);
    const [invites, setInvites] = useState<TeamInvite[]>([]);

    const [showForm, setShowForm] = useState(false);
    const [teamName, setTeamName] = useState('');
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [fileLabel, setFileLabel] = useState('Upload Image (optional)');
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [successMessage, setSuccessMessage] = useState('');
    const [errorMessage, setErrorMessage] = useState('');

    const navigate = useNavigate();

    const fetchAll = async () => {
        try {
            const [teamData, inviteData] = await Promise.all([
                getMyTeams(),
                fetchUserInvites()
            ]);
            setTeams(teamData);
            setInvites(inviteData);
        } catch (err) {
            console.error('Failed to fetch teams or invites:', err);
        }
    };

    useEffect(() => {
        fetchAll();
    }, []);

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0] || null;
        setSelectedFile(file);
        setFileLabel(file?.name || 'No file chosen');
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        try {
            if (teamName.trim().length < 4) {
                setErrorMessage('Name of the team must be at least 4 characters long.');
                setTimeout(() => setErrorMessage(''), 7000);
                return;
            }
            if (teamName.trim().length > 25) {
                setErrorMessage('Name of the team must be less than 25 characters long.');
                setTimeout(() => setErrorMessage(''), 7000);
                return;
            }

            const createdTeam = await createTeam({
                name: teamName,
                teamPictureUrl: null,
            });

            if (selectedFile) {
                await changeTeamPicture(createdTeam.id, selectedFile);
            }

            setTeamName('');
            setSelectedFile(null);
            setShowForm(false);
            setErrorMessage('');

            setSuccessMessage('The team has been successfully created');
            setTimeout(() => setSuccessMessage(''), 7000);

            const updated = await getMyTeams();
            setTeams(updated);

        } catch (err: any) {
            console.error('Failed to create team:', err);
            const backendMsg = err?.message ?? 'Failed to create team. Please try again.';
            setErrorMessage(backendMsg);
            setTimeout(() => setErrorMessage(''), 7000);
        }
    };


    const handleAccept = async (inviteId: string) => {
        try {
            await acceptInvite(inviteId);
            setInvites(prev => prev.filter(i => i.id !== inviteId));
            const updated = await getMyTeams();
            setTeams(updated);
        } catch (err: any) {
            console.error('Failed to accept invite:', err);
            const backendMsg = err?.message ?? 'Failed to accept invite. Please try again.';
            setErrorMessage(backendMsg);
        }
    };

    const handleReject = async (inviteId: string) => {
        try {
            await rejectInvite(inviteId);
            setInvites(prev => prev.filter(i => i.id !== inviteId));
        } catch (err) {
            console.error('Failed to reject invite:', err);
        }
    };

        return (
        <div className={styles.wrapper}>

            {invites.length > 0 && (


                <div className={styles.inviteSection}>
                    <h1 className={styles.title}>Team Invites</h1>

                    {invites.map(invite => (

                        <TeamInviteCard
                            key={invite.id}
                            id={invite.id}
                            name={invite.teamName}
                            teamPictureUrl={invite.teamPictureUrl}
                            onAccept={handleAccept}
                            onReject={handleReject}
                        />
                    ))}

                    <hr className={styles.lineBreak}/>
                </div>
            )}

            <div className={styles.header}>
                <h1 className={styles.title}>Teams</h1>

                <button className={styles.createButton} onClick={() => setShowForm(true)}>
                    + Create new team
                </button>
            </div>

            <div
                className={`${styles.expandable} ${showForm ? styles.expanded : ""}`}
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
                                onKeyDown={(e) =>
                                    (e.key === "Enter" || e.key === " ") && fileInputRef.current?.click()}
                                title={fileLabel ?? "Upload Image"}
                                aria-live="polite"
                            >

                                  <span className={styles.uploadBtnText}>
                                      {fileLabel ?? "Upload Image"} </span>
                            </button>

                            <input
                                type="file"
                                ref={fileInputRef}
                                onChange={handleFileChange}
                                accept="image/*"
                                className={styles.hiddenInput}
                            />
                        </div>


                        <button type="submit">Create</button>
                        <button type="button"
                                onClick={() => setShowForm(false)}>Cancel
                        </button>

                    </form>
                </div>
            </div>

            {successMessage &&
                <p className={styles.successMessage}>{successMessage}</p>
            }
            {errorMessage &&
                <p className={styles.message}>{errorMessage}</p>
            }

            {teams.map(team => (
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