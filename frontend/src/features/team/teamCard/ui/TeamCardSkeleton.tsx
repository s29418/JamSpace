import React from 'react';
import Skeleton from 'shared/ui/skeleton/Skeleton';
import styles from './TeamCard.module.css';

export default function TeamCardSkeleton() {
    return (
        <div className={styles.card}>
            <Skeleton rounded="full" height={96} width={96} />
            <div style={{ flex: 1, marginLeft: 16 }}>
                <Skeleton height={24} width="45%" style={{ marginBottom: 24 }} />
                <Skeleton height={12} width="70%" />
            </div>
        </div>
    );
}
