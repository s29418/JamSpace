import React from "react";
import styles from "./SeenAvatars.module.css";

type SeenUser = {
    userId: string;
    displayName: string;
    avatarUrl: string | null;
};

type Props = {
    users: SeenUser[];
};

const SeenAvatars = ({ users }: Props) => {
    if (users.length === 0) return null;

    return (
        <div className={styles.wrapper}>
            {users.map((user) =>
                user.avatarUrl ? (
                    <img
                        key={user.userId}
                        src={user.avatarUrl}
                        alt={user.displayName}
                        title={user.displayName}
                        className={styles.avatar}
                    />
                ) : (
                    <div
                        key={user.userId}
                        title={user.displayName}
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