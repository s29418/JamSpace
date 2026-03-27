import React from 'react';
import styles from './FollowsListPage.module.css';
import Skeleton from "../../../shared/ui/skeleton/Skeleton";

export const FollowsListPageSkeleton: React.FC = () => {
    return (
        <div className={styles.wrapper}>
            <Skeleton width={220} height={28} style={{ marginBottom: 24 }} />

            <ul className={styles.list}>
                {Array.from({ length: 6 }).map((_, i) => (
                    <li key={i} className={styles.item}>
                        <Skeleton rounded="full" width={56} height={56} />
                        <div className={styles.meta}>
                            <Skeleton width="40%" height={18} />
                            <Skeleton width="20%" height={14} style={{ marginTop: 4 }} />
                        </div>
                        <div className={styles.action}>
                            <Skeleton width={70} height={20} rounded="lg" />
                        </div>
                    </li>
                ))}
            </ul>
        </div>
    );
};
