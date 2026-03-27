import React from 'react';
import { useAutoAnimate } from '@formkit/auto-animate/react';
import styles from '../TeamSettingsModal.module.css';
import { InviteItem } from './InviteItem';

export type InviteView = {
    id: string;
    invitedUserName: string;
    invitedUserId: string;
    invitedUserPictureUrl?: string | null;
    invitedByUserName: string;
};

type Props = {
    invites: InviteView[];
    onCancel: (inviteId: string) => void | Promise<void>;
    className?: string;
    emptyText?: string;
};

export const InvitesList: React.FC<Props> = ({
                                                 invites,
                                                 onCancel,
                                                 className,
                                                 emptyText = 'No pending invites',
                                             }) => {
    const [parent] = useAutoAnimate({ duration: 350, easing: 'cubic-bezier(0.22,1,0.36,1)' });

    return (
        <div className={className}>
            <ul className={styles.memberList} ref={parent}>
                {invites.map((invite) => (
                    <InviteItem
                        key={invite.id}
                        invite={invite}
                        onCancel={() => onCancel(invite.id)}
                    />
                ))}
            </ul>

            {invites.length === 0 && (
                <p className={styles.emptyNote}>{emptyText}</p>
            )}
        </div>
    );
};
