import React from 'react';
import styles from './Skeleton.module.css';

type Props = React.HTMLAttributes<HTMLDivElement> & {
    rounded?: 'sm' | 'md' | 'lg' | 'full';
    height?: number | string;
    width?: number | string;
};

export default function Skeleton({ rounded='md', height=16, width='100%', style, className, ...rest }: Props) {
    const r = rounded === 'full' ? 999 : rounded === 'lg' ? 16 : rounded === 'md' ? 8 : 4;
    return (
        <div
            className={`${styles.skeleton} ${className ?? ''}`}
            style={{ height, width, borderRadius: r, ...style }}
            {...rest}
        >
            <div className={styles.shimmer} />
        </div>
    );
}
