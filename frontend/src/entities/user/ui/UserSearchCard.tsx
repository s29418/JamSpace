import React from "react";
import { Link } from "react-router-dom";
import type { UserSearchItem } from "entities/user/model/types";
import TagRow from "./TagRow";
import styles from "./UserSearchCard.module.css";

import {
    UserPlusIcon,
    ChatBubbleOvalLeftEllipsisIcon as ChatIcon,
} from "@heroicons/react/24/outline";
import { UserMinusIcon } from "@heroicons/react/24/solid";

type Props = {
    user: UserSearchItem;
    busy: boolean;
    onToggleFollow: () => void;
    onMessage?: (userId: string) => void | Promise<void>;
};

const UserSearchCard: React.FC<Props> = ({ user, busy, onToggleFollow, onMessage }) => {
    const loc = [user.location?.city, user.location?.country].filter(Boolean).join(", ");

    return (
        <div className={styles.card}>
            <div className={styles.left}>
                <Link to={`/profile/${user.id}`} className={styles.avatarLink}>
                    <div className={styles.avatarWrap}>
                        {user.profilePictureUrl ? (
                            <img
                                className={styles.avatar}
                                src={user.profilePictureUrl}
                                alt={`${user.username} avatar`}
                            />
                        ) : (
                            <div className={styles.avatarFallback}>
                                {user.username?.[0].toUpperCase() ?? "?"}
                            </div>
                        )}
                    </div>
                </Link>

                <div className={styles.meta}>
                    <Link to={`/profile/${user.id}`} className={styles.avatarLink}>
                        <div className={styles.displayName}>{user.displayName}</div>
                        <div className={styles.username}>@{user.username}</div>
                    </Link>

                    <div className={styles.location}>{loc}</div>
                    <div className={styles.followerss}>{user.followersCount} followers</div>

                    <TagRow tags={user.skills} />
                    <TagRow tags={user.genres} variant="primary" />
                </div>
            </div>

            <div className={styles.right}>
                {!user.isMe && (
                    <button
                        className={styles.messageBtn}
                        onClick={() => void onMessage?.(user.id)}
                        type="button"
                    >
                        <ChatIcon className={styles.icon} />
                        Message
                    </button>
                )}

                {!user.isMe && (
                    <button
                        className={`${styles.followBtn} ${user.isFollowing ? styles.following : styles.follow}`}
                        onClick={onToggleFollow}
                        disabled={busy}
                        type="button"
                    >
                        {user.isFollowing ? (
                            <>
                                <UserMinusIcon className={styles.icon} />
                                Following
                            </>
                        ) : (
                            <>
                                <UserPlusIcon className={styles.icon} />
                                Follow
                            </>
                        )}
                    </button>
                )}
            </div>
        </div>
    );
};

export default UserSearchCard;