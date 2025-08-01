import React, {useState} from 'react';
import styles from './TeamCard.module.css';
import defaultTeamIcon from '../assets/defaultTeamIcon.jpg';
import TeamSettingsModal from "../components/modals/TeamSettingsModal";
import {
    CogIcon as SettingsIcon,
} from '@heroicons/react/24/outline';

interface Member {
    userId: string;
    username: string;
    role: string;
}

interface TeamCardProps {
    id: string;
    name: string;
    teamPictureUrl?: string;
    members: Member[];
    onClick: () => void;
}

interface Team {
    id: string;
    name: string;
    teamPictureUrl?: string;
    members: {
        userId: string;
        username: string;
        role: string;
        userPictureUrl?: string;
    }[];
}

const TeamCard = ({ id, name, teamPictureUrl, members, onClick }: TeamCardProps) => {
    const [showModal, setShowModal] = useState(false);

    const [team, setTeam] = useState<{
        id: string;
        name: string;
        teamPictureUrl?: string;
        members: Member[];
    }>({
        id,
        name,
        teamPictureUrl,
        members,
    });

    const handleCardClick = (e: React.MouseEvent) => {
        if ((e.target as HTMLElement).closest('button')) return;
        if (showModal) return;
        onClick();
    };

    const handleTeamUpdate = (updatedTeam: Team) => {
        setTeam(updatedTeam);
    };

    return (
        <div className={styles.card} onClick={handleCardClick}>
            <div className={styles.avatar}>
                <img src={team.teamPictureUrl || defaultTeamIcon} alt={team.name} />
            </div>

            <div className={styles.info}>
                <h3>{team.name}</h3>
                <span>{team.members.length} members</span>
            </div>

            <div className={styles.actions}>
                <button
                    onClick={(e) => {
                        e.stopPropagation();
                        setShowModal(true);
                    }}
                    className={styles.settingsButton}
                >
                    <SettingsIcon className={styles.icon}/> Settings
                </button>
            </div>

            {showModal && (
                <TeamSettingsModal
                    teamId={team.id}
                    onClose={() => setShowModal(false)}
                    onTeamNameUpdate={handleTeamUpdate}
                />
            )}
        </div>
    );
};


export default TeamCard;
