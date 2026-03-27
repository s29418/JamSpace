import React from "react";
import styles from "./UsersResults.module.css";
import type { UserSearchItem } from "entities/user/model/types";
import UserSearchCard from "../../../entities/user/ui/UserSearchCard";

type Props = {
    items: UserSearchItem[];
    loading: boolean;
    busy: Record<string, boolean>;
    onToggleFollow: (id: string) => void;
    onMessage: (userId: string) => void | Promise<void>;
};

const UsersResults: React.FC<Props> = ({
                                           items,
                                           loading,
                                           busy,
                                           onToggleFollow,
                                           onMessage,
                                       }) => {
    if (loading) return <p className={styles.info}>Loading...</p>;
    if (!items.length) return <p className={styles.info}>No results.</p>;

    return (
        <div className={styles.list}>
            {items.map((u) => (
                <UserSearchCard
                    key={u.id}
                    user={u}
                    busy={busy[u.id]}
                    onToggleFollow={() => onToggleFollow(u.id)}
                    onMessage={onMessage}
                />
            ))}
        </div>
    );
};

export default UsersResults;