import React from 'react';
import { Link, useParams } from 'react-router-dom';
import styles from './FollowsListPage.module.css';
import {FollowsMode, useUserFollows} from "../../../features/user/profileHeader/model/useUserFollows";

type Props = {
    mode: FollowsMode
};


const FollowsListPage: React.FC<Props> = ({ mode }) => {
    const { id } = useParams<{ id: string }>();
    const { items, loading, error } = useUserFollows(id, mode);

    return (
        <div className={styles.wrapper}>
            <h2 className={styles.title}>{mode === 'followers' ? 'Followers' : 'Following'}</h2>
            {loading && <p className={styles.info}>Loading…</p>}
            {error && <p className={styles.error}>{error}</p>}

            <ul className={styles.list}>
                {items.map(u => (

                    <li key={u.id} className={styles.item}>

                        <Link to={`/profile/${u.id}`} className={styles.avatarLink}>
                            {u.profilePictureUrl
                                ? <img src={u.profilePictureUrl} alt={`${u.username} avatar`} />
                                : <div className={styles.avatarFallback}>{u.displayName?.[0] ?? u.username?.[0] ?? '?'}</div>}
                        </Link>

                        <div className={styles.meta}>
                            <Link to={`/profile/${u.id}`} className={styles.displayName}>{u.displayName}</Link>
                            <div className={styles.username}>@{u.username}</div>
                        </div>

                    </li>
                ))}

                {!loading && !items.length && <p className={styles.info}>No users.</p>}
            </ul>
        </div>
    );
};

export default FollowsListPage;