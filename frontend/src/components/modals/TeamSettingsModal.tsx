import ReactDOM from 'react-dom';
import React, {useEffect, useRef, useState} from 'react';
import modalStyles from './TeamSettingsModal.module.css';
import styles from '../../pages/TeamDetailsPage.module.css';
import defaultTeamIcon from '../../assets/defaultTeamIcon.jpg';
import {
    TrashIcon as DeleteIcon,
    PencilSquareIcon as EditIcon,
    CameraIcon as ChangePictureIcon
} from '@heroicons/react/24/outline';
import {
    getTeamById,
    changeTeamName,
    deleteTeam,
    changeTeamPicture
} from '../../services/teams.service';

import {
    inviteUserToTeam,
    cancelInvite,
    fetchTeamInvitesByTeamId
} from '../../services/teamInvites.service';

import {
    changeTeamMemberMusicalRole,
    changeTeamMemberRole,
    kickTeamMember,
} from '../../services/teamMembers.service';

import ExpandableMessage from '../common/ExpandableMessage';
import {useNavigate} from "react-router-dom";

interface Props {
    teamId: string;
    onClose: () => void;
    onTeamUpdate?: (team: Team) => void;
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
    currentUserRole?: string;
}

interface Invite {
    id: string;
    invitedUserName: string;
    invitedUserPictureUrl?: string;
    invitedByUserName: string;
}

const TeamSettingsModal: React.FC<Props> = ({ teamId, onClose, onTeamUpdate }) => {
    const [team, setTeam] = useState<Team | null>(null);
    const [loading, setLoading] = useState(true);
    const [inviteUsername, setInviteUsername] = useState('');
    const [inviteMessage, setInviteMessage] = useState<MessageState>(null);
    const [teamMemberMessage, setTeamMemberMessage] = useState<MessageState>(null);
    const [teamMessage, setTeamMessage] = useState<MessageState>(null);
    const [isEditingName, setIsEditingName] = useState(false);
    const [newTeamName, setNewTeamName] = useState('');
    const [invites, setInvites] = useState<Invite[]>([]);
    const [editingRoleUserId, setEditingRoleUserId] = useState<string | null>(null);
    const [editingMusicalUserId, setEditingMusicalUserId] = useState<string | null>(null);
    const [newMusicalRole, setNewMusicalRole] = useState('');
    const navigate = useNavigate();
    const [showUpload, setShowUpload] = useState(false);
    const fileInputRef = useRef<HTMLInputElement>(null);

    type MessageState = { text: string; color: string } | null;

    function showMessage(
        setMessage: React.Dispatch<React.SetStateAction<MessageState>>,
        text: string,
        type: 'success' | 'error'
    ) {
        const color = type === 'success' ? '#26cdd4' : '#ef4444';
        setMessage({ text, color });

        setTimeout(() => {
            setMessage(null);
        }, 5000);
    }

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
                const data = await fetchTeamInvitesByTeamId(teamId);
                setInvites(data);
            } catch (error) {
                console.error('Failed to fetch team invites:', error);
            }
        };

        fetchInvites();
    }, [teamId]);

    const handleAvatarClick = () => {
        setShowUpload((prev) => !prev);
    };

    const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) return;

        try {
            if (!team) return;
            const successMsg = await changeTeamPicture(team.id, file);
            const updated = await getTeamById(team.id);

            setTeam(updated);
            setShowUpload(false);
            showMessage(setInviteMessage, successMsg, 'success');
            onTeamUpdate?.(updated);
        } catch (error: any) {
            console.error("Upload failed", error);
            const backendMsg = error?.message ?? 'Failed to upload team picture.';
            showMessage(setTeamMessage, backendMsg, 'error');
        }
    };

    const handleChangeTeamName = async (newName: string) => {
        if (!newName.trim()) return;

        try {
            const updatedTeam = await changeTeamName(teamId, newName);
            setTeam(updatedTeam);
            showMessage(setInviteMessage, 'Team name updated.', 'success');

            onTeamUpdate?.(updatedTeam);
            setIsEditingName(false);
        } catch (err: any) {
            console.error('Failed to change name:', err);
            showMessage(setTeamMessage, err?.message ?? 'Failed to change team name.', 'error');
        }
    }

    const handleDeleteTeam = async () => {
        try {
            const confirmed = window.confirm('Are you sure you want to delete this team?');
            if (!confirmed) return;

            const successMsg = await deleteTeam(teamId);
            showMessage(setTeamMessage, successMsg, 'success');
            onClose();
            navigate('/teams');
            window.location.reload();
        } catch (error: any) {
            console.error('Failed to delete team:', error);
            const backendMsg = error?.message ?? 'Failed to delete team.';
            showMessage(setTeamMessage, backendMsg, 'error');
        }
    };

    const handleChangeMemberRole = async (userId: string, newRole: string) => {
        try {
            const successMsg = await changeTeamMemberRole(teamId, userId, newRole);
            const updated = await getTeamById(teamId);
            setTeam(updated);
            showMessage(setTeamMemberMessage, successMsg, 'success');
        } catch (error: any) {
            console.error('Failed to change member role:', error);
            const backendMsg = error?.message ?? 'Failed to change member role.';
            showMessage(setTeamMemberMessage, backendMsg, 'error');
        }
    }

    const handleChangeMemberMusicalRole = async (userId: string, newMusicalRole: string) => {
        if (!newMusicalRole.trim()) return;

        try {
            const successMsg = await changeTeamMemberMusicalRole(teamId, userId, newMusicalRole);
            const updated = await getTeamById(teamId);
            setTeam(updated);
            showMessage(setTeamMemberMessage, successMsg, 'success');
        } catch (error: any) {
            console.error('Failed to change member musical role:', error);
            const backendMsg = error?.message ?? 'Failed to change member musical role.';
            showMessage(setTeamMemberMessage, backendMsg, 'error');
        }
    }

    const handleKickMember = async (userId: string) => {
        try {
            const confirmed = window.confirm('Are you sure you want to kick this member from the team?');
            if (!confirmed) return;

            const successMsg = await kickTeamMember(teamId, userId);
            const updated = await getTeamById(teamId);
            setTeam(updated);
            showMessage(setTeamMemberMessage, successMsg, 'success');
        } catch (error: any) {
            console.error('Failed to kick member:', error);
            const backendMsg = error?.message ?? 'Failed to kick member.';
            showMessage(setTeamMemberMessage, backendMsg, 'error');
        }
    }

    const handleCancelInvite = async (inviteId: string) => {
        try {
            const successMsg = await cancelInvite(inviteId);
            showMessage(setInviteMessage, successMsg, 'success');
            setInvites(prev => prev.filter(i => i.id !== inviteId));
        } catch (error: any) {
            console.error('Failed to invite user:', error);
            const backendMsg = error?.message ?? 'Failed to cancel invite.';
            showMessage(setInviteMessage, backendMsg, 'error');
        }
    };

    const handleInviteUserToTeam = async (username: string, teamId: string) => {
        try {
            const successMsg = await inviteUserToTeam(username, teamId);
            const updatedInvites = await fetchTeamInvitesByTeamId(teamId);
            setInvites(updatedInvites);
            showMessage(setInviteMessage, successMsg, 'success');
            setInviteUsername('');
        } catch (error: any) {
            console.error('Failed to invite user:', error);
            const backendMsg = error?.message ?? 'Failed to send invite.';
            showMessage(setInviteMessage, backendMsg, 'error');
        }
    };

    if (loading || !team) return <div/>

    return ReactDOM.createPortal(
        <div className={modalStyles.backdrop} onClick={onClose}>
            <div className={modalStyles.modal} onClick={e => e.stopPropagation()}>
                <button className={modalStyles.close} onClick={onClose}>✖</button>

                <div className={styles.teamInfo}>

                    <div
                        className={styles.avatarWrapper}
                        onClick={
                            (team.currentUserRole === 'Leader' || team.currentUserRole === 'Admin')
                                ? handleAvatarClick
                                : undefined
                        }
                        style={{
                            cursor: team.currentUserRole === 'Leader' ? 'pointer' : 'default'
                        }}
                    >
                        <img
                            src={team.teamPictureUrl || defaultTeamIcon}
                            alt={team.name}
                            className={styles.avatarModal}
                        />

                        {(team.currentUserRole === 'Leader' || team.currentUserRole === 'Admin') && (
                            <ChangePictureIcon className={styles.cameraIcon}/>
                        )}
                    </div>

                    <div className={styles.textCol}>
                        <h1 className={styles.title} title={team.name}>{team.name}</h1>

                        {(team.currentUserRole === 'Leader' || team.currentUserRole === 'Admin') && (
                            <div>
                                <button
                                    className={styles.editButton}
                                    onClick={() => {
                                        setNewTeamName(team.name);
                                        setIsEditingName(true);
                                    }}
                                >
                                    <EditIcon className={styles.icon}/> Change name
                                </button>

                                <button
                                    className={styles.deleteButton}
                                    onClick={handleDeleteTeam}
                                >
                                    <DeleteIcon className={styles.icon}/> Delete team
                                </button>

                                <div
                                    className={
                                        modalStyles.expandable +
                                        (isEditingName ? ' ' + modalStyles.expanded : '')
                                    }
                                >
                                    <input
                                        className={styles.teamNameInput}
                                        value={newTeamName}
                                        onChange={(e) => setNewTeamName(e.target.value)}
                                    />

                                    <div className={styles.changeNameOptions}>
                                        <button
                                            className={styles.inviteButton}
                                            onClick={() => handleChangeTeamName(newTeamName)}
                                        >
                                            Save
                                        </button>

                                        <button
                                            className={styles.inviteButton}
                                            onClick={() => setIsEditingName(false)}
                                        >
                                            Cancel
                                        </button>
                                    </div>
                                </div>
                            </div>
                        )}

                        <ExpandableMessage message={teamMessage}/>
                    </div>
                </div>

                <div className={`${modalStyles.uploadPanel} ${showUpload ? modalStyles.visible : ''}`}>
                    <input
                        type="file"
                        ref={fileInputRef}
                        accept="image/*"
                        className={modalStyles.hiddenInput}
                        onChange={handleFileChange}
                    />
                    <button className={styles.uploadButton}
                            onClick={() => fileInputRef.current?.click()}>
                    Choose file
                    </button>
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

                                    {team.currentUserRole === 'Leader' && (
                                        <EditIcon
                                            className={styles.editRoleIcon}
                                            onClick={() =>
                                                setEditingRoleUserId(
                                                    editingRoleUserId === member.userId ? null : member.userId)
                                            }
                                        />
                                    )}

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

                                {(team.currentUserRole === 'Leader' || team.currentUserRole === 'Admin') ? (
                                    <p className={styles.musicalRole}>
                                        Musical role: {member.musicalRole || 'None'}
                                        <EditIcon
                                            className={styles.editMusicalRoleIcon}
                                            onClick={() => {
                                                setEditingMusicalUserId(
                                                    editingMusicalUserId === member.userId ? null : member.userId
                                                );
                                                setNewMusicalRole(member.musicalRole || '');
                                            }}
                                        />
                                    </p>
                                ) : (
                                    member.musicalRole && (
                                        <p className={styles.musicalRole}>
                                            Musical role: {member.musicalRole}
                                        </p>
                                    )
                                )}


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

                                {team.currentUserRole === 'Leader' && (
                                    <button className={styles.userActionButton} style={{marginTop: "4px"}}
                                            onClick={() =>
                                                handleKickMember(member.userId)}>
                                        ✖ Kick from team
                                    </button>
                                )}

                            </div>
                        </li>
                    ))}
                </ul>

                <ExpandableMessage message={teamMemberMessage}/>

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
                                        <div>
                                            <p className={styles.username}>{invite.invitedUserName}</p>
                                            <p className={styles.role}>
                                                Invited by: {invite.invitedByUserName} </p>
                                        </div>


                                        <button className={styles.userActionButton} onClick={() =>
                                            handleCancelInvite(invite.id)}>
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

                <ExpandableMessage message={inviteMessage} />

            </div>
        </div>,
        document.getElementById('modal-root') as HTMLElement
    );
};

export default TeamSettingsModal;
