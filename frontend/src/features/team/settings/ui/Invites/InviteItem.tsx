import React from 'react';
import styles from '../TeamSettingsModal.module.css';

import type { InviteView } from './InvitesList';
import {Link} from "react-router-dom";

type Props = {
    invite: InviteView;
    onCancel: () => void | Promise<void>;
};

const _InviteItem: React.FC<Props> = ({ invite, onCancel }) => {
    return (
        <li className={styles.member}>
            <Link to={`/profile/${invite.invitedUserId}`} className={styles.userAvatarWrapper + " " + styles.noUnderline}>
                {invite.invitedUserPictureUrl ? (

                    <img
                        src={invite.invitedUserPictureUrl}
                        alt={invite.invitedUserName}
                        className={styles.userAvatar}
                    />

                ) : (

                    <div className={styles.avatarFallback}>
                        {invite.invitedUserName?.[0].toUpperCase() ?? "?"}
                    </div>

                )}
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
