import React, { useCallback, useMemo, useState, useEffect } from 'react';
import styles from './TeamCard.module.css';
import defaultTeamIcon from '../../../shared/assets/defaultTeamIcon.jpg';
import { CogIcon as SettingsIcon } from '@heroicons/react/24/outline';
import type { Team } from '../model/types';

type TeamCardProps = Pick<Team, 'id' | 'name' | 'teamPictureUrl' | 'members'> & {
    onClick: () => void;
    onOpenSettings?: (teamId: string) => void;
};

const TeamCard: React.FC<TeamCardProps> = ({
                                               id,
                                               name,
                                               teamPictureUrl,
                                               members,
                                               onClick,
                                               onOpenSettings,
                                           }) => {
    const [imgSrc, setImgSrc] = useState<string>(teamPictureUrl || defaultTeamIcon);

    useEffect(() => {
        setImgSrc(teamPictureUrl || defaultTeamIcon);
    }, [teamPictureUrl]);

    const membersCount = useMemo(() => members?.length ?? 0, [members]);

    const handleCardClick = (e: React.MouseEvent) => {
        if ((e.target as HTMLElement).closest('button')) return;
        onClick();
    };

    const onImgError = useCallback(() => setImgSrc(defaultTeamIcon), []);

    return (
        <div className={styles.card} onClick={handleCardClick} role="button" tabIndex={0}>
            <div className={styles.avatar}>
                <img
                    src={imgSrc}
                    alt={name}
                    className={styles.avatarImage}
                    loading="lazy"
                    decoding="async"
                    onError={onImgError}
                />
            </div>

            <div className={styles.info}>
                <h3 className={styles.teamName} title={name}>
                    {name}
                </h3>
                <span>{membersCount} members</span>
            </div>

            <div className={styles.actions}>
                <button
                    type="button"
                    className={styles.settingsButton}
                    aria-haspopup="dialog"
                    onClick={(e) => {
                        e.stopPropagation();
                        onOpenSettings?.(id);
                    }}
                >
                    <SettingsIcon className={styles.icon} /> Settings
                </button>
            </div>
        </div>
    );
};

export default TeamCard;