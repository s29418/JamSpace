import ReactDOM from 'react-dom';
import React, { useEffect, useState } from 'react';
import { getTeamById, inviteUserToTeam } from '../../services/teamService';
import modalStyles from './TeamSettingsModal.module.css';
import styles from '../../pages/TeamDetailsPage.module.css';
import defaultTeamIcon from '../../assets/defaultTeamIcon.jpg';
import {
    TrashIcon as DeleteIcon,
    PencilSquareIcon as EditIcon,
} from '@heroicons/react/24/outline';
import {
    changeTeamName,
    deleteTeam
} from '../../services/teamService';

interface Props {
    teamId: string;
    onClose: () => void;
    onTeamNameUpdate?: (team: Team) => void;
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

const TeamSettingsModal: React.FC<Props> = ({ teamId, onClose, onTeamNameUpdate }) => {
    const [team, setTeam] = useState<Team | null>(null);
    const [loading, setLoading] = useState(true);
    const [inviteUsername, setInviteUsername] = useState('');
    const [message, setMessage] = useState('');
    const [isEditingName, setIsEditingName] = useState(false);
    const [newTeamName, setNewTeamName] = useState('');

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

    const handleChangeTeamName = async (newName: string) => {
        if (!newName.trim()) return;

        try {
            const updated = await changeTeamName(teamId, newName);
            setTeam(updated);
            setMessage('Team name updated.');

            onTeamNameUpdate?.(updated);
            setIsEditingName(false);
        } catch (err) {
            console.error('Failed to change name:', err);
            setMessage('Failed to change team name.');
        }
    }

    const handleDeleteTeam = async () => {
        try {
            const confirmed = window.confirm('Are you sure you want to delete this team?');
            if (!confirmed) return;

            await deleteTeam(teamId);
            setMessage('Team deleted successfully.');
            onClose();
            window.location.reload();
        } catch (error) {
            console.error('Failed to delete team:', error);
            setMessage('Failed to delete team.');
        }
    };

    if (loading || !team) return null;

    return ReactDOM.createPortal(
        <div className={modalStyles.backdrop} onClick={onClose}>
            <div className={modalStyles.modal} onClick={e => e.stopPropagation()}>
                <button className={modalStyles.close} onClick={onClose}>✖</button>

                <div className={styles.teamInfo}>
                    <img src={team.teamPictureUrl || defaultTeamIcon} alt={team.name} className={styles.avatar} />
                    <div>
                        <h1 className={styles.title}>{team.name}</h1>
                        <div>

                            <button className={styles.editButton} onClick={() => {
                                setNewTeamName(team.name);
                                setIsEditingName(true);
                            }}>
                                <EditIcon className={styles.icon}/> Change name
                            </button>

                            <button className={styles.deleteButton} onClick={handleDeleteTeam}>
                                <DeleteIcon className={styles.icon}/> Delete team
                            </button>

                            <div
                                className={modalStyles.expandable + (isEditingName ? ' ' + modalStyles.expanded : '')}>

                                <input
                                    className={styles.teamNameInput}
                                    value={newTeamName}
                                    onChange={(e) => setNewTeamName(e.target.value)}
                                />

                                <div className={styles.changeNameOptions}>
                                    <button className={styles.inviteButton}
                                            onClick={() => handleChangeTeamName(newTeamName)}>
                                        Save
                                    </button>

                                    <button className={styles.inviteButton}
                                            onClick={() => setIsEditingName(false)}>
                                        Cancel
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

                <h2 className={styles.subtitle}>Members</h2>
                <ul className={styles.memberList}>
                    {team.members.map(member => (
                        <li key={member.userId} className={styles.member}>
                            <img src={member.userPictureUrl || defaultTeamIcon}
                                 alt={member.username}
                                 className={styles.userAvatar}/>
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
                {message && <p className={styles.message}>{message}</p>}
            </div>
        </div>,
        document.getElementById('modal-root') as HTMLElement
    );
};

export default TeamSettingsModal;
