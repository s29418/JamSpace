import React from 'react';
import styles from './TeamCard.module.css';
import defaultTeamIcon from '../assets/defaultTeamIcon.jpg';

interface Member {
    userId: string;
    username: string;
    role: string;
}

interface TeamCardProps {
    name: string;
    teamPictureUrl?: string;
    members: Member[];
}

const TeamCard = ({ name, teamPictureUrl, members }: TeamCardProps) => {
    return (
        <div className={styles.card}>
            <div className={styles.avatar}>
                <img src={teamPictureUrl || defaultTeamIcon} alt={name} />
            </div>
            <div className={styles.info}>
                <h3>{name}</h3>
                <span>{members.length} members</span>
            </div>
            <div className={styles.actions}>
                <button className={styles.settingsButton}>
                    <span>⚙ Settings </span>
                </button>
            </div>
        </div>
    );
};

export default TeamCard;
