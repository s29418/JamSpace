import React, { useMemo, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import styles from './TeamDetailsPage.module.css';
import defaultTeamIcon from '../../../shared/assets/defaultTeamIcon.jpg';
import { getCurrentUserId } from '../../../shared/lib/auth/token';
import TeamSettingsModal from '../../../widgets/team-settings/ui/TeamSettingsModal';
import { CogIcon as SettingsIcon } from '@heroicons/react/24/outline';
import { useTeam } from '../../../features/team/manage-team/model/useTeam';
import TeamCalendar from '../../../widgets/team-calendar/ui/TeamCalendar';
import TeamChat from '../../../widgets/team-chat/ui/TeamChat';

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

            <TeamCalendar
                teamId={team.id}
                currentUserId={currentUserId ?? ''}
                currentUserRole={team.currentUserRole}
            />

            <TeamChat teamId={team.id} />

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
