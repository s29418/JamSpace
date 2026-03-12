import React from "react";
import styles from "./SeenAvatars.module.css";

type SeenUser = {
    userId: string;
    displayName: string;
    avatarUrl: string | null;
};

type Props = {
    users: SeenUser[];
    isOwn: boolean;
};

const SeenAvatars = ({ users, isOwn }: Props) => {
    if (users.length === 0) return null;

    return (
        <div className={`${styles.wrapper} ${isOwn ? styles.own : styles.other}`}>
            {users.map((user) =>
                user.avatarUrl ? (
                    <img
                        key={user.userId}
                        src={user.avatarUrl}
                        alt={user.displayName}
                        title={"seen by: " + user.displayName}
                        className={styles.avatar}
                    />
                ) : (
                    <div
                        key={user.userId}
                        title={"seen by: " + user.displayName}
                        className={styles.fallback}
                    >
                        {user.displayName.charAt(0).toUpperCase()}
                    </div>
                )
            )}
        </div>
    );
};

export default SeenAvatars;