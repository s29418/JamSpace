import React, { useEffect, useMemo, useRef, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import styles from './TeamDetailsPage.module.css';
import defaultTeamIcon from '../../../shared/assets/defaultTeamIcon.jpg';
import { getCurrentUserId } from '../../../shared/lib/auth/token';
import TeamSettingsModal from '../../../widgets/team-settings/ui/TeamSettingsModal';
import { CogIcon as SettingsIcon } from '@heroicons/react/24/outline';
import { useTeam } from '../../../features/team/manage-team/model/useTeam';
import TeamCalendar from '../../../widgets/team-calendar/ui/TeamCalendar';
import TeamChat from '../../../widgets/team-chat/ui/TeamChat';
import TeamProjects from '../../../widgets/team-projects/ui/TeamProjects';
const TeamDetailsPage = () => {
    const { id } = useParams<{ id: string }>();
    const { team, loading, error } = useTeam(id);
    const [showModal, setShowModal] = useState(false);
    const [projectsHeight, setProjectsHeight] = useState<number | null>(null);
    const currentUserId = useMemo(() => getCurrentUserId(), []);
    const calendarAreaRef = useRef<HTMLDivElement | null>(null);
    const chatAreaRef = useRef<HTMLDivElement | null>(null);
    const projectsAreaRef = useRef<HTMLDivElement | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        const calendarElement = calendarAreaRef.current;
        const chatElement = chatAreaRef.current;
        const projectsElement = projectsAreaRef.current;

        if (!calendarElement || !chatElement || !projectsElement) {
            setProjectsHeight(null);
            return;
        }

        const updateProjectsHeight = () => {
            if (window.innerWidth <= 900) {
                setProjectsHeight(null);
                return;
            }

            const calendarPanel = calendarElement.firstElementChild as HTMLElement | null;
            const chatPanel = chatElement.firstElementChild as HTMLElement | null;
            const projectsPanel = projectsElement.firstElementChild as HTMLElement | null;

            if (!calendarPanel || !chatPanel || !projectsPanel) {
                setProjectsHeight(null);
                return;
            }

            const top = projectsPanel.getBoundingClientRect().top;
            const bottom = chatPanel.getBoundingClientRect().bottom;
            setProjectsHeight(Math.max(Math.round(bottom - top), 0));
        };

        updateProjectsHeight();

        const observer = new ResizeObserver(() => {
            updateProjectsHeight();
        });

        observer.observe(calendarElement);
        observer.observe(chatElement);
        observer.observe(projectsElement);
        window.addEventListener('resize', updateProjectsHeight);

        return () => {
            observer.disconnect();
            window.removeEventListener('resize', updateProjectsHeight);
        };
    }, [team?.id]);


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

            <div className={styles.contentGrid}>
                <div ref={calendarAreaRef} className={styles.calendarArea}>
                    <TeamCalendar
                        teamId={team.id}
                        currentUserId={currentUserId ?? ''}
                        currentUserRole={team.currentUserRole}
                    />
                </div>

                <div ref={projectsAreaRef} className={styles.projectsArea}>
                    <TeamProjects teamId={team.id} maxHeight={projectsHeight} />
                </div>

                <div ref={chatAreaRef} className={styles.chatArea}>
                    <TeamChat teamId={team.id} />
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
