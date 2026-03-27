import React, {useEffect, useState} from 'react';
import { getToken } from '../../../shared/lib/auth/token';
import { Link, useParams } from 'react-router-dom';
import styles from './FollowsListPage.module.css';
import {FollowsMode, useUserFollows} from "../../../features/user/follows/model/useUserFollows";
import {jwtDecode} from "jwt-decode";
import {FollowsListPageSkeleton} from "./FollowsListPageSkeleton";

type Props = {
    mode: FollowsMode
};

type JwtPayload = {
    sub: string;
}

const FollowsListPage: React.FC<Props> = ({ mode }) => {
    const { id } = useParams<{ id: string }>();
    const [myId, setMyId] = useState<string | null>(null);

    const { items, loading, busy, ownerName, toggleFollow } = useUserFollows(id, mode);

    useEffect(() => {
        const token = getToken();
        if (!token) { setMyId(null); return; }
        try {
            const { sub } = jwtDecode<JwtPayload>(token);
            setMyId(sub);
        } catch {
            setMyId(null);
        }
    }, []);

    function possessive(name: string) {
        if (!name) return '';
        const trimmed = name.trim();
        const lastChar = trimmed.slice(-1).toLowerCase();
        return lastChar === 's' ? `${trimmed}'` : `${trimmed}'s`;
    }

    return (
        <div className={styles.wrapper}>

            {loading && <FollowsListPageSkeleton />}

            <h2 className={styles.title}>
                {ownerName ? `${possessive(ownerName)} ` : ''}{mode === 'followers' ? 'followers' : 'following'}
            </h2>

            <ul className={styles.list}>
                {items.map(u => (
                    <li key={u.id} className={styles.item}>
                        <Link to={`/profile/${u.id}`} className={styles.avatarLink}>
                            {u.profilePictureUrl
                                ? <img src={u.profilePictureUrl} alt={`${u.username} avatar`}/>
                                : <div
                                    className={styles.avatarFallback}>{u.displayName?.[0].toUpperCase() ?? u.username?.[0].toUpperCase() ?? '?'}</div>}
                        </Link>

                        <div className={styles.meta}>
                            <Link to={`/profile/${u.id}`} className={styles.displayName}>{u.displayName}</Link>
                            <div className={styles.username}>@{u.username}</div>
                        </div>

                        {u.id !== myId && (
                            <div className={styles.action}>
                                <button
                                    type="button"
                                    className={`${styles.btn} ${u.isFollowing ? styles.btnPrimary : styles.btnGhost}`}
                                    onClick={() => void toggleFollow(u.id)}
                                    disabled={busy[u.id]}
                                >
                                    {u.isFollowing ? 'Following' : 'Follow'}
                                </button>
                            </div>
                        )}
                    </li>
                ))}
                {!loading && !items.length && <p className={styles.info}>No users.</p>}
            </ul>
        </div>
    );
};

export default FollowsListPage;