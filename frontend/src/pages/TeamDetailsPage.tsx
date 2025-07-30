import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import {
    getTeamById,
    inviteUserToTeam
} from '../services/teamService';
import styles from './TeamDetailsPage.module.css';
import defaultTeamIcon from '../assets/defaultTeamIcon.jpg';
import TeamSettingsModal from "../components/modals/TeamSettingsModal";
import {
    CogIcon as SettingsIcon,
} from '@heroicons/react/24/outline';

interface Member {
    userId: string;
    username: string;
    role: string;
    userPictureUrl?: string;
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
    const [showModal, setShowModal] = useState(false);

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

    const handleSettingsClick = () => {
        setShowModal(true);
    };

    if (loading) return <p>Loading...</p>;
    if (!team) return <p>Team not found</p>;

    return (
        <div className={styles.wrapper}>

            <div className={styles.teamInfo}>
                <img src={team.teamPictureUrl || defaultTeamIcon} alt={team.name} className={styles.avatar}/>
                <div>
                    <h1 className={styles.title}>{team.name}</h1>
                    <button
                        onClick={(e) => {
                            e.stopPropagation();
                            setShowModal(true);
                        }}
                        className={styles.editButton}>
                        <SettingsIcon className={styles.icon} onClick={handleSettingsClick}/> Settings
                    </button>
                </div>

            </div>

            {showModal && (
                <TeamSettingsModal teamId={team.id} onClose={() => setShowModal(false)} />
            )}

        </div>
    );
};

export default TeamDetailsPage;
