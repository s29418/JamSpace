import React from 'react';
import { useAutoAnimate } from '@formkit/auto-animate/react';
import styles from '../TeamSettingsModal.module.css';
import defaultTeamIcon from '../../../assets/defaultTeamIcon.jpg';

import { TeamMember, TeamRoleCode } from '../../../types/team'
import { MemberItem } from './MemberItem';

type PermissionFn = (member: TeamMember) => boolean;

type Props = {
    members: TeamMember[];
    currentUserId?: string;
    avatarFallbackSrc?: string;

    onChangeRole: (userId: string, role: TeamRoleCode) => void | Promise<void>;
    onChangeMusicalRole: (userId: string, musicalRole: string) => void | Promise<void>;
    onKick?: (userId: string) => void | Promise<void>;

    canChangeRole?: PermissionFn;
    canEditMusicalRole?: PermissionFn;
    canKick?: PermissionFn;

    emptyText?: string;
    className?: string;
};

export const MembersList: React.FC<Props> = ({
                                                 members,
                                                 currentUserId,
                                                 avatarFallbackSrc = defaultTeamIcon,
                                                 onChangeRole,
                                                 onChangeMusicalRole,
                                                 onKick,
                                                 canChangeRole = () => false,
                                                 canEditMusicalRole = () => false,
                                                 canKick = () => false,
                                                 emptyText = 'No members',
                                                 className,
                                             }) => {
    const [parent] = useAutoAnimate({ duration: 350, easing: 'cubic-bezier(0.22,1,0.36,1)' });

    return (
        <div className={className}>
            <ul className={styles.memberList} ref={parent}>
                {members.map((m) => (
                    <MemberItem
                        key={m.userId}
                        member={m}
                        avatarSrc={m.userPictureUrl || avatarFallbackSrc}
                        isSelf={currentUserId === m.userId}
                        canChangeRole={canChangeRole(m)}
                        canEditMusicalRole={canEditMusicalRole(m)}
                        canKick={!!onKick && canKick(m) && currentUserId !== m.userId}
                        onChangeRole={(role) => onChangeRole(m.userId, role)}
                        onChangeMusicalRole={(value) => onChangeMusicalRole(m.userId, value)}
                        onKick={() => onKick?.(m.userId)}
                    />
                ))}
            </ul>

            {members.length === 0 && <p className={styles.emptyNote}>{emptyText}</p>}
        </div>
    );
};
