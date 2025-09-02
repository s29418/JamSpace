import React, { useEffect, useMemo, useState, useCallback } from 'react';
import styles from './TeamCard.module.css';
import defaultTeamIcon from '../assets/defaultTeamIcon.jpg';
import TeamSettingsModal from './TeamSettingsModal/TeamSettingsModal';
import { CogIcon as SettingsIcon } from '@heroicons/react/24/outline';
import { useNavigate } from 'react-router-dom';
import { getCurrentUserId } from '../utils/auth';
import type { Team } from '../types/team';

type TeamCardProps = Pick<Team, 'id' | 'name' | 'teamPictureUrl' | 'members'> & {
    onClick: () => void;
};

const TeamCard: React.FC<TeamCardProps> = ({ id, name, teamPictureUrl, members, onClick }) => {
    const navigate = useNavigate();
    const currentUserId = useMemo(() => getCurrentUserId(), []);

    // lekki snapshot do natychmiastowego UI
    const [team, setTeam] = useState<Pick<Team, 'id' | 'name' | 'teamPictureUrl'>>({
        id,
        name,
        teamPictureUrl: teamPictureUrl ?? null,
    });

    const [imgSrc, setImgSrc] = useState<string>(team.teamPictureUrl || defaultTeamIcon);
    const membersCount = members?.length ?? 0;

    // sync po zmianie propsów (np. przeładowanie listy)
    useEffect(() => {
        setTeam({ id, name, teamPictureUrl: teamPictureUrl ?? null });
    }, [id, name, teamPictureUrl]);

    // aktualizacja obrazka przy zmianie URL
    useEffect(() => {
        setImgSrc(team.teamPictureUrl || defaultTeamIcon);
    }, [team.teamPictureUrl]);

    // live-patche z modala (rename/avatar)
    useEffect(() => {
        function onTeamUpdated(e: any) {
            const d = e?.detail;
            if (!d || d.teamId !== team.id) return;
            setTeam((prev) => ({
                ...prev,
                ...(d.patch?.name ? { name: d.patch.name } : {}),
                ...(d.patch?.teamPictureUrl ? { teamPictureUrl: d.patch.teamPictureUrl } : {}),
            }));
        }
        window.addEventListener('team:updated', onTeamUpdated);
        return () => window.removeEventListener('team:updated', onTeamUpdated);
    }, [team.id]);

    const [showModal, setShowModal] = useState(false);

    const handleCardClick = (e: React.MouseEvent) => {
        if ((e.target as HTMLElement).closest('button')) return;
        if (showModal) return;
        onClick();
    };

    const onImgError = useCallback(() => setImgSrc(defaultTeamIcon), []);

    return (
        <div className={styles.card} onClick={handleCardClick} role="button" tabIndex={0}>
            <div className={styles.avatar}>
                <img
                    src={imgSrc}
                    alt={team.name}
                    className={styles.avatarImage}
                    loading="lazy"
                    decoding="async"
                    onError={onImgError}
                />
            </div>

            <div className={styles.info}>
                <h3 className={styles.teamName} title={team.name}>{team.name}</h3>
                <span>{membersCount} members</span>
            </div>

            <div className={styles.actions}>
                <button
                    type="button"
                    className={styles.settingsButton}
                    aria-haspopup="dialog"
                    onClick={(e) => { e.stopPropagation(); setShowModal(true); }}
                >
                    <SettingsIcon className={styles.icon} /> Settings
                </button>
            </div>

            {showModal && (
                <TeamSettingsModal
                    isOpen={showModal}
                    teamId={team.id}
                    currentUserId={currentUserId ?? ''}
                    onClose={() => setShowModal(false)}
                    onTeamDeleted={() => { setShowModal(false); navigate('/teams'); }}
                    onLeftTeam={() => { setShowModal(false); navigate('/teams'); }}
                />
            )}
        </div>
    );
};

export default TeamCard;
