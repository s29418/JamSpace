import React, { useEffect, useMemo, useRef, useState } from 'react';
import {
    EllipsisHorizontalIcon,
    TrashIcon
} from '@heroicons/react/24/outline';
import { Link, useLocation } from 'react-router-dom';
import { getCurrentUserId } from '../../../shared/lib/auth/token';
import { saveScrollPosition } from '../../../shared/lib/scroll/postDetailsScroll';
import type { Post, PostComment } from '../model/types';
import { formatPostTimestampSafe } from './postCard.utils';
import styles from './PostCard.module.css';

type Props = {
    post: Post;
    comments: PostComment[];
    onDeleteComment?: (post: Post, commentId: string) => void | Promise<void>;
    maxVisibleComments?: number;
    viewAllHref?: string;
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
                onClick={(event) => {
                    event.stopPropagation();
                    setMenuOpen((current) => !current);
                }}
            >
                <EllipsisHorizontalIcon width={18} height={18} />
            </button>

            {menuOpen && (
                <div className={`${styles.menu} ${styles.commentMenu}`}>
                    <button
                        type="button"
                        className={styles.menuItem}
                        onClick={(event) => {
                            event.stopPropagation();
                            setMenuOpen(false);
                            void onDeleteComment(post, comment.id);
                        }}
                    >
                        <TrashIcon className={styles.trashIcon}/>
                        Delete comment
                    </button>
                </div>
            )}
        </div>
    );
};

export const PostComments: React.FC<Props> = ({
    post,
    comments,
    onDeleteComment,
    maxVisibleComments,
    viewAllHref,
}) => {
    const currentUserId = useMemo(() => getCurrentUserId(), []);
    const location = useLocation();
    const visibleComments = typeof maxVisibleComments === 'number'
        ? comments.slice(0, maxVisibleComments)
        : comments;
    const hasHiddenComments = Boolean(viewAllHref) && post.commentCount > visibleComments.length;

    if (!visibleComments.length && !hasHiddenComments) {
        return null;
    }

    return (
        <div className={styles.comments}>
            {visibleComments.map((comment) => {
                const commentAuthorName = comment.authorDisplayName?.trim() || 'Unknown user';
                const commentTimestamp = formatPostTimestampSafe(comment.createdAt);
                const canDeleteComment = currentUserId === comment.authorId;

                return (
                    <div key={comment.id} className={styles.comment}>
                        <Link
                            to={`/profile/${comment.authorId}`}
                            className={styles.commentAvatarLink}
                            onClick={(event) => event.stopPropagation()}
                        >
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
                                <Link
                                    to={`/profile/${comment.authorId}`}
                                    className={styles.commentAuthor}
                                    onClick={(event) => event.stopPropagation()}
                                >
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

            {hasHiddenComments && viewAllHref && (
                <Link
                    to={viewAllHref}
                    className={styles.viewAllCommentsLink}
                    onClick={(event) => {
                        event.stopPropagation();
                        saveScrollPosition(`${location.pathname}${location.search}`);
                    }}
                >
                    Display all comments
                </Link>
            )}
        </div>
    );
};
