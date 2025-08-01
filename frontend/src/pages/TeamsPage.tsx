import React, {useEffect, useRef, useState} from 'react';
import { useNavigate } from 'react-router-dom';
import {
    getMyTeams,
    uploadTeamPicture,
    createTeam,
    getTeamInvites,
    acceptTeamInvite,
    rejectTeamInvite
} from '../services/teamService';
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
    const [teamPictureUrl, setTeamPictureUrl] = useState('');
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [fileLabel, setFileLabel] = useState('No file chosen');
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [successMessage, setSuccessMessage] = useState('');
    const [errorMessage, setErrorMessage] = useState('');

    const navigate = useNavigate();

    const fetchAll = async () => {
        try {
            const [teamData, inviteData] = await Promise.all([
                getMyTeams(),
                getTeamInvites()
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
                return;
            }

            let uploadedUrl = teamPictureUrl.trim();

            if (!selectedFile) {
            } else {
                uploadedUrl = await uploadTeamPicture(selectedFile);
            }

            await createTeam({
                name: teamName,
                teamPictureUrl: uploadedUrl || null,
            });

            setTeamName('');
            setSelectedFile(null);
            setShowForm(false);
            setErrorMessage('');

            setSuccessMessage('The team has been successfully created');
            setTimeout(() => setSuccessMessage(''), 3000);

            const updated = await getMyTeams();
            setTeams(updated);
        } catch (err) {
            console.error('Failed to create team:', err);
            setErrorMessage('Failed to create team. Please try again.');
        }
    };


    const handleAccept = async (inviteId: string) => {
        try {
            await acceptTeamInvite(inviteId);
            setInvites(prev => prev.filter(i => i.id !== inviteId));
            const updated = await getMyTeams();
            setTeams(updated);
        } catch (err) {
            console.error('Failed to accept invite:', err);
        }
    };

    const handleReject = async (inviteId: string) => {
        try {
            await rejectTeamInvite(inviteId);
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

            {successMessage &&
                <p className={styles.successMessage}>{successMessage}</p>
            }
            {errorMessage &&
                <p className={styles.errorMessage}>{errorMessage}</p>
            }

            {showForm && (
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
                        <label>Team picture (optional)</label>
                        <div className={styles.fileUploadRow}>
                            <button type="button" onClick={() => fileInputRef.current?.click()}>
                                Upload Image
                            </button>
                            <span className={styles.fileLabel}>{fileLabel}</span>
                        </div>
                        <input
                            type="file"
                            ref={fileInputRef}
                            onChange={handleFileChange}
                            accept="image/*"
                            className={styles.hiddenInput}
                        />
                    </div>


                    <button type="submit">Create</button>
                    <button type="button" onClick={() => setShowForm(false)}>Cancel</button>

                </form>
            )}

            {teams.map(team => (
                <div>
                    <TeamCard
                        id={team.id}
                        name={team.name}
                        teamPictureUrl={team.teamPictureUrl}
                        members={team.members}
                        onClick={() => navigate(`/teams/${team.id}`)}
                    />
                </div>
            ))}
        </div>
        );
        };

        export default TeamsPage;
