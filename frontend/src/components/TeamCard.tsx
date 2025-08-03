import React, {useState} from 'react';
import styles from './TeamCard.module.css';
import defaultTeamIcon from '../assets/defaultTeamIcon.jpg';
import TeamSettingsModal from "../components/modals/TeamSettingsModal";
import {
    CogIcon as SettingsIcon,
    ArrowRightStartOnRectangleIcon as LeaveIcon
} from '@heroicons/react/24/outline';
import { leaveTeam } from '../services/teamService';

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
    onLeave?: (teamId: string) => void;
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

const TeamCard = ({ id, name, teamPictureUrl, members, onClick, onLeave }: TeamCardProps) => {
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

    const handleLeaveTeam = async () => {
        try {
            if(!window.confirm("Are you sure you want to leave the team? This action cannot be undone.")) return;

            await leaveTeam(team.id);
            if (onLeave) {
                onLeave(team.id);
            }
        } catch (error) {
            console.error('Failed to leave team:', error);
        }
    }

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

                <button
                    onClick={(e) => {
                        e.stopPropagation();
                        handleLeaveTeam();
                    }}
                    className={styles.settingsButton}>

                    <LeaveIcon className={styles.icon}/> Leave team
                </button>

            </div>

            {showModal && (
                <TeamSettingsModal
                    teamId={team.id}
                    onClose={() => setShowModal(false)}
                    onTeamUpdate={handleTeamUpdate}
                />
            )}
        </div>
    );
};


export default TeamCard;
