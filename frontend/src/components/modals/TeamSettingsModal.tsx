import ReactDOM from 'react-dom';
import React, { useEffect, useState } from 'react';
import { getTeamById, inviteUserToTeam } from '../../services/teamService';
import modalStyles from './TeamSettingsModal.module.css';
import styles from '../../pages/TeamDetailsPage.module.css';
import defaultTeamIcon from '../../assets/defaultTeamIcon.jpg';
import {
    TrashIcon as DeleteIcon,
    PencilSquareIcon as EditIcon
} from '@heroicons/react/24/outline';
import {
    changeTeamName,
    deleteTeam,
    getTeamInvitesByTeamId,
    cancelTeamInvite,
    changeTeamMemberRole,
    changeTeamMemberMusicalRole
} from '../../services/teamService';
import {useNavigate} from "react-router-dom";

interface Props {
    teamId: string;
    onClose: () => void;
    onTeamNameUpdate?: (team: Team) => void;
}

interface Member {
    userId: string;
    username: string;
    role: string;
    musicalRole?: string;
    userPictureUrl?: string;
}

interface Team {
    id: string;
    name: string;
    teamPictureUrl?: string;
    members: Member[];
}

interface Invite {
    id: string;
    invitedUserName: string;
    invitedUserPictureUrl?: string;
}

const TeamSettingsModal: React.FC<Props> = ({ teamId, onClose, onTeamNameUpdate }) => {
    const [team, setTeam] = useState<Team | null>(null);
    const [loading, setLoading] = useState(true);
    const [inviteUsername, setInviteUsername] = useState('');
    const [message, setMessage] = useState('');
    const [isEditingName, setIsEditingName] = useState(false);
    const [newTeamName, setNewTeamName] = useState('');
    const [invites, setInvites] = useState<Invite[]>([]);
    const [editingRoleUserId, setEditingRoleUserId] = useState<string | null>(null);
    const [editingMusicalUserId, setEditingMusicalUserId] = useState<string | null>(null);
    const [newMusicalRole, setNewMusicalRole] = useState('');
    const navigate = useNavigate();

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

    useEffect(() => {
        const fetchInvites = async () => {
            try {
                const data = await getTeamInvitesByTeamId(teamId);
                setInvites(data);
            } catch (error) {
                console.error('Failed to fetch team invites:', error);
            }
        };

        fetchInvites();
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
            navigate('/teams');
            window.location.reload();
        } catch (error) {
            console.error('Failed to delete team:', error);
            setMessage('Failed to delete team.');
        }
    };

    const cancelInvite = async (inviteId: string) => {
        try {
            await cancelTeamInvite(inviteId);
            setMessage('Invite cancelled successfully.');
            setInvites(prev => prev.filter(i => i.id !== inviteId));
        } catch (error) {
            console.error('Failed to cancel invite:', error);
            setMessage('Failed to cancel invite.');
        }
    };

    const handleChangeMemberRole = async (userId: string, newRole: string) => {
        if (!newRole.trim()) return;

        try {
            await changeTeamMemberRole(teamId, userId, newRole);
            const updated = await getTeamById(teamId);
            setTeam(updated);
            setMessage('Member role updated.');
        } catch (err) {
            console.error('Failed to change member role:', err);
            setMessage('Failed to change member role.');
        }
    }

    const handleChangeMemberMusicalRole = async (userId: string, newMusicalRole: string) => {
        if (!newMusicalRole.trim()) return;

        try {
            await changeTeamMemberMusicalRole(teamId, userId, newMusicalRole);
            const updated = await getTeamById(teamId);
            setTeam(updated);
            setMessage('Member musical role updated.');
        } catch (err) {
            console.error('Failed to change member musical role:', err);
            setMessage('Failed to change member musical role.');
        }
    }

    const handleInviteUserToTeam = async (username: string, teamId: string) => {
        if (!username.trim()) {
            setMessage('Username cannot be empty.');
            return;
        }

        try {
            await inviteUserToTeam(username, teamId);
            const updatedInvites = await getTeamInvitesByTeamId(teamId);
            setInvites(updatedInvites);
            setMessage(`User "${username}" has been invited.`);
            setInviteUsername('');
        } catch (error) {
            console.error('Failed to invite user:', error);
            setMessage('Failed to send invite. User may not exist or is already a member.');
        }
    }

    if (loading || !team) return <div/>

    return ReactDOM.createPortal(
        <div className={modalStyles.backdrop} onClick={onClose}>
            <div className={modalStyles.modal} onClick={e => e.stopPropagation()}>
                <button className={modalStyles.close} onClick={onClose}>✖</button>

                <div className={styles.teamInfo}>
                    <img src={team.teamPictureUrl || defaultTeamIcon} alt={team.name} className={styles.avatar}/>
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

                <hr className={styles.lineBreak}/>

                <h2 className={styles.subtitle}>Members</h2>

                <ul className={styles.memberList}>
                    {team && team.members && team.members.map(member => (

                        <li key={member.userId} className={styles.member}>
                            <img src={member.userPictureUrl || defaultTeamIcon}
                                 alt={member.username}
                                 className={styles.userAvatar}/>
                            <div>
                                <p className={styles.username}>{member.username}</p>

                                <p className={styles.role}>
                                    Team role: {member.role}
                                    <EditIcon
                                        className={styles.editRoleIcon}
                                        onClick={() =>
                                            setEditingRoleUserId(editingRoleUserId === member.userId ? null : member.userId)
                                        }
                                    />
                                </p>

                                {editingRoleUserId === member.userId && (
                                    <div className={`${styles.expandable} ${styles.expanded}`}>
                                        <select
                                            className={styles.roleSelect}
                                            value={""}
                                            onChange={(e) => {
                                                handleChangeMemberRole(member.userId, e.target.value);
                                                setEditingRoleUserId(null);
                                            }}
                                        >
                                            <option value="" disabled>Select role</option>
                                            <option value="2">Member</option>
                                            <option value="1">Admin</option>
                                            <option value="0">Leader</option>
                                        </select>
                                    </div>
                                )}

                                <p className={styles.musicalRole}>
                                    Musical role: {member.musicalRole}
                                    <EditIcon
                                        className={styles.editMusicalRoleIcon}
                                        onClick={() => {
                                            setEditingMusicalUserId(editingMusicalUserId === member.userId ? null : member.userId);
                                            setNewMusicalRole(member.musicalRole || '');
                                        }}
                                    />
                                </p>

                                {editingMusicalUserId === member.userId && (
                                    <div className={`${styles.expandable} ${styles.expanded}`}>
                                        <input
                                            type="text"
                                            value={newMusicalRole}
                                            onChange={(e) => setNewMusicalRole(e.target.value)}
                                            className={styles.musicalRoleInput}
                                            placeholder="Enter new musical role"
                                        />
                                        <div className={styles.editButtonsRow}>
                                            <button
                                                className={styles.userActionButton}
                                                onClick={() => {
                                                    handleChangeMemberMusicalRole(member.userId, newMusicalRole);
                                                    setEditingMusicalUserId(null);
                                                }}
                                            >
                                                Save
                                            </button>
                                            <button
                                                className={styles.userActionButton}
                                                onClick={() => setEditingMusicalUserId(null)}
                                            >
                                                Cancel
                                            </button>
                                        </div>
                                    </div>
                                )}
                            </div>
                        </li>
                    ))}
                </ul>

                <hr className={styles.lineBreak}/>

                {invites.length > 0 && (

                    <div className={styles.memberSection}>

                        <h2 className={styles.subtitle}>Invites</h2>

                        <ul className={styles.memberList}>
                            {invites.map((invite: Invite) => (

                                <li key={invite.id} className={styles.member}>
                                    <img src={invite.invitedUserPictureUrl || defaultTeamIcon}
                                         alt={invite.invitedUserName}
                                         className={styles.userAvatar}/>

                                    <div className={styles.invitedUserDetails}>
                                        <span className={styles.username}>{invite.invitedUserName}</span>

                                        <button className={styles.userActionButton} onClick={() =>
                                            cancelInvite(invite.id)}>
                                            ✖ Cancel Invite
                                        </button>
                                    </div>
                                </li>
                            ))}
                        </ul>
                    </div>
                )}

                <h2 className={styles.subtitle}>Invite User</h2>

                <form className={styles.inviteForm}
                      onSubmit={(e) => {
                          e.preventDefault();
                          handleInviteUserToTeam(inviteUsername, teamId);
                          setInviteUsername('');
                      }}
                >
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
