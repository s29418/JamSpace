import React, { useEffect, useMemo, useRef, useState } from 'react';
import { Link } from 'react-router-dom';
import {
    ArrowPathRoundedSquareIcon,
    ChatBubbleOvalLeftIcon,
    EllipsisHorizontalIcon,
    HeartIcon,
} from '@heroicons/react/24/outline';
import type { Post } from '../model/types';
import styles from './PostCard.module.css';

type Props = {
    post: Post;
    canDelete?: boolean;
    onDelete?: (postId: string) => void | Promise<void>;
    isNested?: boolean;
};

function formatPostTimestamp(createdAt: string) {
    const createdDate = new Date(createdAt);
    const now = new Date();
    const diffMs = now.getTime() - createdDate.getTime();

    if (Number.isNaN(createdDate.getTime()) || diffMs < 0) {
        return '';
    }

    const diffMinutes = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMinutes < 1) return 'now';
    if (diffMinutes < 60) return `${diffMinutes} min`;
    if (diffHours < 24) return `${diffHours}h`;
    if (diffDays <= 7) return `${diffDays}d`;

    const day = String(createdDate.getDate()).padStart(2, '0');
    const month = String(createdDate.getMonth() + 1).padStart(2, '0');
    const currentYear = now.getFullYear();
    const year = String(createdDate.getFullYear()).slice(-2);

    return createdDate.getFullYear() === currentYear
        ? `${day}.${month}`
        : `${day}.${month}.${year}`;
}

function inferMediaKind(mediaType?: string | null, mediaUrl?: string | null) {
    const source = `${mediaType ?? ''} ${mediaUrl ?? ''}`.toLowerCase();

    if (source.includes('image')) return 'image';
    if (source.includes('video')) return 'video';
    if (source.includes('audio')) return 'audio';
    return 'file';
}

export const PostCard: React.FC<Props> = ({
    post,
    canDelete = false,
    onDelete,
    isNested = false,
}) => {
    const [menuOpen, setMenuOpen] = useState(false);
    const menuRef = useRef<HTMLDivElement | null>(null);
    const timestamp = useMemo(() => formatPostTimestamp(post.createdAt), [post.createdAt]);
    const mediaKind = useMemo(
        () => inferMediaKind(post.mediaType, post.mediaUrl),
        [post.mediaType, post.mediaUrl],
    );

    useEffect(() => {
        function handleClickOutside(event: MouseEvent) {
            if (!menuRef.current?.contains(event.target as Node)) {
                setMenuOpen(false);
            }
        }

        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    const authorName = post.authorDisplayName?.trim() || 'Unknown user';
    const authorSlug = `/profile/${post.authorId}`;

    return (
        <article className={`${styles.card} ${isNested ? styles.repostCard : ''}`}>
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

            {post.content && <div className={styles.content}>{post.content}</div>}

            {post.mediaUrl && (
                <div className={styles.mediaWrap}>
                    {mediaKind === 'image' && (
                        <img className={styles.image} src={post.mediaUrl} alt="Post attachment" />
                    )}
                    {mediaKind === 'video' && (
                        <video className={styles.video} src={post.mediaUrl} controls preload="metadata" />
                    )}
                    {mediaKind === 'audio' && (
                        <audio className={styles.audio} src={post.mediaUrl} controls preload="metadata" />
                    )}
                    {mediaKind === 'file' && (
                        <a
                            className={styles.fileLink}
                            href={post.mediaUrl}
                            target="_blank"
                            rel="noreferrer"
                        >
                            Open attachment
                        </a>
                    )}
                </div>
            )}

            {post.originalPost && (
                <PostCard
                    post={post.originalPost}
                    isNested
                />
            )}

            {!isNested && (
                <div className={styles.actions}>
                    <button type="button" className={styles.actionButton} aria-label="Likes">
                        <HeartIcon />
                        <span className={styles.actionValue}>{post.likeCount}</span>
                    </button>

                    <button type="button" className={styles.actionButton} aria-label="Comments">
                        <ChatBubbleOvalLeftIcon />
                        <span className={styles.actionValue}>{post.commentCount}</span>
                    </button>

                    <button type="button" className={styles.actionButton} aria-label="Reposts">
                        <ArrowPathRoundedSquareIcon />
                        <span className={styles.actionValue}>{post.repostCount}</span>
                    </button>
                </div>
            )}
        </article>
    );
};
