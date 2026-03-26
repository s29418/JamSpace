import React from 'react';
import styles from './ProfileHeaderCard.module.css';
import Skeleton from "../../../shared/ui/skeleton/Skeleton";

export const ProfileHeaderCardSkeleton: React.FC = () => {
    return (
        <div className={styles.profileCard} style={{marginTop: 142}}>
            <div className={styles.profileCardAvatar}>
                <Skeleton rounded="full" width={160} height={160} />
            </div>

            <div style={{ width: '100%', marginTop: 64 }}>
                <Skeleton width="40%" height={42} />
                <Skeleton width="15%" height={12} style={{ marginTop: 6 }} />
                <Skeleton width="30%" height={16} style={{ marginTop: 8 }} />

                <div className={styles.profileCardMeta}>
                    <Skeleton width={90} height={12} />
                    <Skeleton width={90} height={12} />
                </div>

                <div className={styles.actionsInline}>
                    <Skeleton width={100} height={32} rounded="lg" />
                    <Skeleton width={100} height={32} rounded="lg" />
                </div>

                <div className={styles.profileCardChipsRow}>
                    {Array.from({ length: 5 }).map((_, i) => (
                        <Skeleton key={i} width={70} height={22} rounded="lg" />
                    ))}
                </div>

                <div className={styles.profileCardChipsRow}>
                    {Array.from({ length: 4 }).map((_, i) => (
                        <Skeleton key={i} width={70} height={22} rounded="lg" />
                    ))}
                </div>

                <Skeleton width="100%" height={90} style={{ marginTop: 32 }} />
            </div>
        </div>
    );
};
