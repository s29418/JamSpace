import React from "react";
import { useNavigate } from "react-router-dom";
import { getOrCreateDirectConversation } from "entities/chat/api/conversations.api";
import styles from "./UserSearchCard.module.css";
import type { UserSearchItem } from "entities/user/model/types";
import {Link} from "react-router-dom";

import { UserPlusIcon, ChatBubbleOvalLeftEllipsisIcon as ChatIcon } from "@heroicons/react/24/outline";
import { UserMinusIcon } from "@heroicons/react/24/solid";

type Props = {
    user: UserSearchItem;
    busy: boolean;
    onToggleFollow: () => void;
};

const UserSearchCard: React.FC<Props> = ({ user, busy, onToggleFollow }) => {
    const loc = [user.location?.city, user.location?.country].filter(Boolean).join(", ");

    const navigate = useNavigate();

    const handleMessageClick = async () => {
        try {
            const result = await getOrCreateDirectConversation({
                otherUserId: user.id,
            });

            navigate(`/chat?conversationId=${result.conversationId}`);
        } catch (error) {
            console.error("Failed to open direct conversation", error);
        }
    };

    return (
        <div className={styles.card}>
            <div className={styles.left}>
                <Link to={`/profile/${user.id}`} className={styles.avatarLink}>
                    <div className={styles.avatarWrap}>
                        {user.profilePictureUrl ? (
                            <img className={styles.avatar} src={user.profilePictureUrl} alt={`${user.username} avatar`} />
                        ) : (
                            <div className={styles.avatarFallback}>{user.username?.[0].toUpperCase() ?? "?"}</div>
                        )}
                    </div>
                </Link>

                <div className={styles.meta}>

                    <Link to={`/profile/${user.id}`} className={styles.avatarLink}>
                        <div className={styles.name}>{user.username}</div>
                    </Link>

                    <div className={styles.location}>{loc}</div>

                    <div className={styles.tagsRow}>
                        {user.skills.map((s) => (
                            <span key={s} className={`${styles.tag} ${styles.tagPrimary}`}>
                {s}
              </span>
                        ))}
                    </div>

                    <div className={styles.tagsRow}>
                        {user.genres.map((g) => (
                            <span key={g} className={styles.tag}>
                {g}
              </span>
                        ))}
                    </div>
                </div>
            </div>

            <div className={styles.right}>

                {!user.isMe && (
                    <button
                        className={styles.messageBtn}
                        onClick={() => void handleMessageClick()}
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
                    >
                        {
                            user.isFollowing ?
                                <><UserMinusIcon className={styles.icon}/>Following</>
                                :
                                <><UserPlusIcon className={styles.icon}/>Follow</>
                        }
                    </button>
                )}

                <div className={styles.followers}>{user.followersCount} followers</div>

            </div>
        </div>
    );
};

export default UserSearchCard;
