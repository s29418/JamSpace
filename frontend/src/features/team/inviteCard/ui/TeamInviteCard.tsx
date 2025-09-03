import React from 'react';
import styles from '../../teamCard/ui/TeamCard.module.css';
import defaultTeamIcon from '../../../../shared/assets/defaultTeamIcon.jpg';
import { CheckIcon, XMarkIcon } from '@heroicons/react/24/solid';

interface TeamInviteCardProps {
    id: string;
    name: string;
    teamPictureUrl?: string | null;
    onAccept: (id: string) => void;
    onReject: (id: string) => void;
}

const TeamInviteCard = ({ id, name, teamPictureUrl, onAccept, onReject }: TeamInviteCardProps) => {
    return (
        <div className={styles.card}>

            <div className={styles.avatar}>
                <img src={teamPictureUrl || defaultTeamIcon} alt={name} />
            </div>

            <div className={styles.info}>
                <h3>{name}</h3>
            </div>

            <div className={styles.inviteActions}>

                <button className={styles.acceptButton} onClick={() => onAccept(id)}>
                    <CheckIcon className={styles.icon} />
                    <span>Accept</span>
                </button>

                <button className={styles.rejectButton} onClick={() => onReject(id)}>
                    <XMarkIcon className={styles.icon}/>
                    <span>Reject</span>
                </button>

            </div>
        </div>
    );
};

export default TeamInviteCard;
