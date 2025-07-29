import ReactDOM from 'react-dom';
import React, { useEffect, useState } from 'react';
import { getTeamById, inviteUserToTeam } from '../../services/teamService';
import modalStyles from './TeamSettingsModal.module.css';
import styles from '../../pages/TeamDetailsPage.module.css';
import defaultTeamIcon from '../../assets/defaultTeamIcon.jpg';

interface Props {
    teamId: string;
    onClose: () => void;
}

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

const TeamSettingsModal: React.FC<Props> = ({ teamId, onClose }) => {
    const [team, setTeam] = useState<Team | null>(null);
    const [loading, setLoading] = useState(true);
    const [inviteUsername, setInviteUsername] = useState('');
    const [message, setMessage] = useState('');

    useEffect(() => {
        const fetchTeam = async () => {
            try {
                const data = await getTeamById(teamId);
                setTeam(data);
            } catch (error) {
                console.error('Failed to fetch team details:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchTeam();
    }, [teamId]);

    if (loading || !team) return null;

    return ReactDOM.createPortal(
        <div className={modalStyles.backdrop} onClick={onClose}>
            <div className={modalStyles.modal} onClick={e => e.stopPropagation()}>
                <button className={modalStyles.close} onClick={onClose}>✖</button>

                <div className={styles.teamInfo}>
                    <img src={team.teamPictureUrl || defaultTeamIcon} alt={team.name} className={styles.avatar} />
                    <h1 className={styles.title}>{team.name}</h1>
                </div>

                <h2 className={styles.subtitle}>Members</h2>
                <ul className={styles.memberList}>
                    {team.members.map(member => (
                        <li key={member.userId} className={styles.member}>
                            <img src={member.userPictureUrl || defaultTeamIcon} alt={member.username} className={styles.userAvatar} />
                            <span className={styles.username}>
                {member.username} ({member.role})
              </span>
                        </li>
                    ))}
                </ul>

                <form className={styles.inviteForm} onSubmit={(e) => {
                    e.preventDefault();
                    inviteUserToTeam(inviteUsername, team.id)
                        .then(() => {
                            setMessage(`User "${inviteUsername}" has been invited.`);
                            setInviteUsername('');
                        })
                        .catch(() => setMessage('Failed to send invite.'));
                }}>
                    <input
                        className={styles.inviteInput}
                        type="text"
                        placeholder="Enter username"
                        value={inviteUsername}
                        onChange={(e) => setInviteUsername(e.target.value)}
                        required
                    />
                    <button className={styles.inviteButton} type="submit">Invite</button>
                </form>
                {message && <p>{message}</p>}
            </div>
        </div>,
        document.getElementById('modal-root') as HTMLElement
    );
};

export default TeamSettingsModal;
