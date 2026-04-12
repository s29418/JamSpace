import React, { useEffect, useMemo, useRef, useState } from 'react';
import { Link } from 'react-router-dom';
import { ArrowPathRoundedSquareIcon, EllipsisHorizontalIcon } from '@heroicons/react/24/outline';
import type { Post } from '../model/types';
import { formatPostTimestamp } from './postCard.utils';
import styles from './PostCard.module.css';

type Props = {
    post: Post;
    canDelete?: boolean;
    onDelete?: (postId: string) => void | Promise<void>;
};

export const PostHeader: React.FC<Props> = ({
    post,
    canDelete = false,
    onDelete,
}) => {
    const [menuOpen, setMenuOpen] = useState(false);
    const menuRef = useRef<HTMLDivElement | null>(null);
    const timestamp = useMemo(() => formatPostTimestamp(post.createdAt), [post.createdAt]);
    const authorName = post.authorDisplayName?.trim() || 'Unknown user';
    const authorSlug = `/profile/${post.authorId}`;

    useEffect(() => {
        function handleClickOutside(event: MouseEvent) {
            if (!menuRef.current?.contains(event.target as Node)) {
                setMenuOpen(false);
            }
        }

        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    return (
        <>
            {post.originalPost && (
                <div className={styles.repostLabel}>
                    <ArrowPathRoundedSquareIcon width={16} height={16} />
                    Repost
                </div>
            )}

            <div className={styles.header}>
                <div className={styles.authorBlock}>
                    <Link to={authorSlug} className={styles.authorLink}>
                        {post.authorAvatarUrl ? (
                            <img
                                className={styles.avatar}
                                src={post.authorAvatarUrl}
                                alt={`${authorName} avatar`}
                            />
                        ) : (
                            <div className={styles.avatarFallback}>
                                {authorName[0]?.toUpperCase() ?? '?'}
                            </div>
                        )}
                    </Link>

                    <div className={styles.authorMeta}>
                        <Link to={authorSlug} className={styles.authorLink}>
                            <div className={styles.displayNameRow}>
                                <span className={styles.displayName}>{authorName}</span>
                                {timestamp && <span className={styles.timestamp}>{timestamp}</span>}
                            </div>
                        </Link>
                    </div>
                </div>

                {canDelete && onDelete && (
                    <div className={styles.menuWrap} ref={menuRef}>
                        <button
                            type="button"
                            className={styles.menuButton}
                            aria-label="More options"
                            onClick={() => setMenuOpen((current) => !current)}
                        >
                            <EllipsisHorizontalIcon width={20} height={20} />
                        </button>

                        {menuOpen && (
                            <div className={styles.menu}>
                                <button
                                    type="button"
                                    className={styles.menuItem}
                                    onClick={() => {
                                        setMenuOpen(false);
                                        void onDelete(post.id);
                                    }}
                                >
                                    Delete post
                                </button>
                            </div>
                        )}
                    </div>
                )}
            </div>
        </>
    );
};
