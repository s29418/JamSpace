import React from 'react';
import { usePostsFeed } from '../../../features/post/model/usePostsFeed';
import { PostFeed } from '../../../widgets/post-feed/ui/PostFeed';
import styles from './HomePage.module.css';

const HomePage = () => {
    const { posts, loading, error } = usePostsFeed({ mode: 'auto' });

    return (
        <main className={styles.page}>
            <div className={styles.content}>
                <PostFeed
                    posts={posts}
                    loading={loading}
                    error={error}
                    emptyText="No posts to show yet."
                />
            </div>
        </main>
    );
};

export default HomePage;
