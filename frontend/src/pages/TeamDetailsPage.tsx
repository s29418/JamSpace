import React, { useMemo, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import styles from './TeamDetailsPage.module.css';
import defaultTeamIcon from '../assets/defaultTeamIcon.jpg';
import { getCurrentUserId } from '../utils/auth';
import TeamSettingsModal from '../components/TeamSettingsModal/TeamSettingsModal';
import { CogIcon as SettingsIcon } from '@heroicons/react/24/outline';
import { useTeam } from '../hooks/useTeam';

const TeamDetailsPage = () => {
    const { id } = useParams<{ id: string }>();
    const { team, loading, error } = useTeam(id);
    const [showModal, setShowModal] = useState(false);
    const currentUserId = useMemo(() => getCurrentUserId(), []);
    const navigate = useNavigate();

    if (loading) return <p>Loading...</p>;
    if (error) return <p>{error}</p>;
    if (!team) return <p>Team not found</p>;

    return (
        <div className={styles.wrapper}>
            <div className={styles.teamInfo}>
                <div className={styles.avatarWrapper}>
                    <img
                        src={team.teamPictureUrl || defaultTeamIcon}
                        alt={team.name}
                        className={styles.avatar}
                        loading="lazy"
                        decoding="async"
                    />
                </div>

                <div>
                    <h1 className={styles.title}>{team.name}</h1>
                    <button
                        onClick={(e) => { e.stopPropagation(); setShowModal(true); }}
                        className={styles.editButton}
                    >
                        <SettingsIcon className={styles.icon} /> Settings
                    </button>
                </div>
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

export default TeamDetailsPage;
