import React, { useState } from 'react';
import styles from '../TeamSettingsModal.module.css';
import { TeamMember, TeamRoleLabel } from '../../../../../entities/team/model/types';
import { RoleSelect } from './RoleSelect';
import { MusicalRoleEditor } from './MusicalRoleEditor';
import { PencilSquareIcon } from '@heroicons/react/24/outline';

type Props = {
    member: TeamMember;
    avatarSrc: string;
    isSelf?: boolean;

    canChangeRole: boolean;
    canEditMusicalRole: boolean;
    canKick: boolean;

    onChangeRole: (role: TeamRoleLabel) => void | Promise<void>;
    onChangeMusicalRole: (musicalRole: string) => void | Promise<void>;
    onKick?: () => void | Promise<void>;
};

const _MemberItem: React.FC<Props> = ({
                                          member,
                                          avatarSrc,
                                          isSelf = false,
                                          canChangeRole,
                                          canEditMusicalRole,
                                          canKick,
                                          onChangeRole,
                                          onChangeMusicalRole,
                                          onKick,
                                      }) => {
    const [editingRole, setEditingRole] = useState(false);
    const [editingMusical, setEditingMusical] = useState(false);

    return (
        <li className={styles.member}>
            {/* Avatar */}
            <div className={styles.userAvatarWrapper}>
                <img src={avatarSrc} alt={member.username} className={styles.userAvatar} />
            </div>

            {/* Dane + akcje */}
            <div className={styles.invitedUserDetails}>
                <div>
                    <p className={styles.username}>{member.username}</p>

                    {/* Rola w zespole */}
                    <p className={styles.role}>
                        Team role: {member.role}
                        {canChangeRole && (

                                <PencilSquareIcon
                                    className={styles.editRoleIcon}
                                    type="button"
                                    onClick={() => setEditingRole((s) => !s)}
                                    aria-expanded={editingRole}
                                    aria-label={`Edit team role for ${member.username}`}
                                />

                        )}
                    </p>

                    {editingRole && canChangeRole && (
                        <RoleSelect
                            initialValue={undefined}
                            onPick={async (role) => {
                                await onChangeRole(role);
                                setEditingRole(false);
                            }}
                            onClose={() => setEditingRole(false)}
                        />
                    )}

                    {/* Rola muzyczna */}
                    <p
                        className={styles.role}
                    >
                        Musical role: {member.musicalRole?.trim() || '—'}
                        {canEditMusicalRole && (
                            <PencilSquareIcon
                                type="button"
                                className={styles.editRoleIcon}
                                onClick={() => setEditingMusical((s) => !s)}
                                aria-expanded={editingMusical}
                                aria-label={`Edit musical role for ${member.username}`}
                            />
                        )}
                    </p>

                    {editingMusical && canEditMusicalRole && (
                        <MusicalRoleEditor
                            initialValue={member.musicalRole || ''}
                            onSave={async (value) => {
                                await onChangeMusicalRole(value);
                                setEditingMusical(false);
                            }}
                            onCancel={() => setEditingMusical(false)}
                        />
                    )}
                </div>

                {/* Kick */}
                {canKick && !isSelf && (
                    <button
                        type="button"
                        className={styles.userActionButton}
                        onClick={() => onKick?.()}
                        aria-label={`Remove ${member.username} from team`}
                    >
                        ✖ Kick from team
                    </button>
                )}
            </div>
        </li>
    );
};

export const MemberItem = React.memo(_MemberItem);
