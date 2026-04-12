import React, { useEffect, useMemo, useRef, useState } from 'react';
import { EllipsisHorizontalIcon } from '@heroicons/react/24/outline';
import { Link } from 'react-router-dom';
import { getCurrentUserId } from '../../../shared/lib/auth/token';
import type { Post, PostComment } from '../model/types';
import { formatPostTimestampSafe } from './postCard.utils';
import styles from './PostCard.module.css';

type Props = {
    post: Post;
    comments: PostComment[];
    onDeleteComment?: (post: Post, commentId: string) => void | Promise<void>;
};

type CommentMenuProps = {
    post: Post;
    comment: PostComment;
    onDeleteComment?: (post: Post, commentId: string) => void | Promise<void>;
};

const CommentMenu: React.FC<CommentMenuProps> = ({ post, comment, onDeleteComment }) => {
    const [menuOpen, setMenuOpen] = useState(false);
    const menuRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        function handleClickOutside(event: MouseEvent) {
            if (!menuRef.current?.contains(event.target as Node)) {
                setMenuOpen(false);
            }
        }

        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    if (!onDeleteComment) {
        return null;
    }

    return (
        <div className={`${styles.menuWrap} ${styles.commentMenuWrap}`} ref={menuRef}>
            <button
                type="button"
                className={`${styles.menuButton} ${styles.commentMenuButton}`}
                aria-label="More comment options"
                onClick={() => setMenuOpen((current) => !current)}
            >
                <EllipsisHorizontalIcon width={18} height={18} />
            </button>

            {menuOpen && (
                <div className={`${styles.menu} ${styles.commentMenu}`}>
                    <button
                        type="button"
                        className={styles.menuItem}
                        onClick={() => {
                            setMenuOpen(false);
                            void onDeleteComment(post, comment.id);
                        }}
                    >
                        Delete comment
                    </button>
                </div>
            )}
        </div>
    );
};

export const PostComments: React.FC<Props> = ({ post, comments, onDeleteComment }) => {
    const currentUserId = useMemo(() => getCurrentUserId(), []);

    if (!comments.length) {
        return null;
    }

    return (
        <div className={styles.comments}>
            {comments.map((comment) => {
                const commentAuthorName = comment.authorDisplayName?.trim() || 'Unknown user';
                const commentTimestamp = formatPostTimestampSafe(comment.createdAt);
                const canDeleteComment = currentUserId === comment.authorId;

                return (
                    <div key={comment.id} className={styles.comment}>
                        <Link to={`/profile/${comment.authorId}`} className={styles.commentAvatarLink}>
                            {comment.authorAvatarUrl ? (
                                <img
                                    className={styles.commentAvatar}
                                    src={comment.authorAvatarUrl}
                                    alt={`${commentAuthorName} avatar`}
                                />
                            ) : (
                                <div className={styles.commentAvatarFallback}>
                                    {commentAuthorName[0]?.toUpperCase() ?? '?'}
                                </div>
                            )}
                        </Link>

                        <div className={styles.commentBody}>
                            <div className={styles.commentMeta}>
                                <Link to={`/profile/${comment.authorId}`} className={styles.commentAuthor}>
                                    {commentAuthorName}
                                </Link>

                                <div className={styles.commentMetaRight}>
                                    {commentTimestamp && (
                                        <span className={styles.commentTimestamp}>
                                            {commentTimestamp}
                                        </span>
                                    )}
                                    {canDeleteComment && (
                                        <CommentMenu
                                            post={post}
                                            comment={comment}
                                            onDeleteComment={onDeleteComment}
                                        />
                                    )}
                                </div>
                            </div>

                            <p className={styles.commentContent}>{comment.content}</p>
                        </div>
                    </div>
                );
            })}
        </div>
    );
};
