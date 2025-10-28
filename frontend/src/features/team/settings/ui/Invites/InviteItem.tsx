import React from 'react';
import styles from '../TeamSettingsModal.module.css';

import type { InviteView } from './InvitesList';
import {Link} from "react-router-dom";

type Props = {
    invite: InviteView;
    avatarSrc: string;
    onCancel: () => void | Promise<void>;
};

const _InviteItem: React.FC<Props> = ({ invite, avatarSrc, onCancel }) => {
    return (
        <li className={styles.member}>
            <Link to={`/profile/${invite.invitedUserId}`} className={styles.userAvatarWrapper}>
                <img
                    src={avatarSrc}
                    alt={invite.invitedUserName}
                    className={styles.userAvatar}
                />
            </Link>

            <div className={styles.invitedUserDetails}>
                <div>
                    <Link to={`/profile/${invite.invitedUserId}`} className={styles.userLink}>
                        <p className={styles.username}>
                            {invite.invitedUserName}
                        </p>
                    </Link>
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
