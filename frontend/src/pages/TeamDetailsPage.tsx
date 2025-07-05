import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import {
    getTeamById,
    inviteUserToTeam
} from '../services/teamService';
import styles from './TeamDetailsPage.module.css';
import defaultTeamIcon from '../assets/defaultTeamIcon.jpg';

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

const TeamDetailsPage = () => {
    const { id } = useParams<{ id: string }>();
    const [team, setTeam] = useState<Team | null>(null);
    const [loading, setLoading] = useState(true);
    const [inviteUsername, setInviteUsername] = useState('');
    const [message, setMessage] = useState('');

    useEffect(() => {
        const fetchTeam = async () => {
            try {
                if (!id) return;
                const data = await getTeamById(id);
                setTeam(data);
            } catch (error) {
                console.error('Failed to fetch team details:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchTeam();
    }, [id]);

    const handleInvite = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!team || !id) return;

        try {
            await inviteUserToTeam(inviteUsername, id);

            setMessage(`User "${inviteUsername}" has been invited.`);
            setInviteUsername('');

        } catch (err) {
            console.error('Invite failed:', err);
            setMessage('Failed to send invite.');
        }
    };

    if (loading) return <p>Loading...</p>;
    if (!team) return <p>Team not found</p>;

    return (
        <div className={styles.wrapper}>
            <h1 className={styles.title}>{team.name}</h1>

            <img src={team.teamPictureUrl || defaultTeamIcon} alt={team.name} className={styles.avatar} />

            <h2 className={styles.subtitle}>Members</h2>
            <ul className={styles.memberList}>
                {team.members.map(member => (
                    <li key={member.userId} className={styles.member}>
                        <span>{member.username} ({member.role})</span>
                    </li>
                ))}
            </ul>



            <form className={styles.inviteForm} onSubmit={handleInvite}>
                <input
                    className={styles.inviteInput}
                    type="text"
                    placeholder="Enter username"
                    value={inviteUsername}
                    onChange={(e) => setInviteUsername(e.target.value)}
                    required
                />
                <button className={styles.inviteButton} type="submit">Invite</button>
            </form>
            {message && <p>{message}</p>}
        </div>
    );
};

export default TeamDetailsPage;
