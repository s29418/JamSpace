import React from 'react';
import styles from '../TeamSettingsModal.module.css';

import type { InviteView } from './InvitesList';

type Props = {
    invite: InviteView;
    avatarSrc: string;
    onCancel: () => void | Promise<void>;
};

const _InviteItem: React.FC<Props> = ({ invite, avatarSrc, onCancel }) => {
    return (
        <li className={styles.member}>
            <div className={styles.userAvatarWrapper}>
                <img
                    src={avatarSrc}
                    alt={invite.invitedUserName}
                    className={styles.userAvatar}
                />
            </div>

            <div className={styles.invitedUserDetails}>
                <div>
                    <p className={styles.username}>{invite.invitedUserName}</p>
                    <p className={styles.role}>Invited by: {invite.invitedByUserName}</p>
                </div>

                <button
                    type="button"
                    className={styles.userActionButton}
                    onClick={onCancel}
                    aria-label={`Cancel invite for ${invite.invitedUserName}`}
                >
                    ✖ Cancel Invite
                </button>
            </div>
        </li>
    );
};

export const InviteItem = React.memo(_InviteItem);
