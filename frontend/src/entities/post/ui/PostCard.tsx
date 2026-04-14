import React, { useEffect, useMemo } from 'react';
import { XMarkIcon } from '@heroicons/react/24/outline';
import { useNavigate } from 'react-router-dom';
import type { Post } from '../model/types';
import styles from './PostCard.module.css';
import { PostHeader } from './PostHeader';
import { PostActions } from './PostActions';
import { PostComments } from './PostComments';
import { PostCommentComposer } from './PostCommentComposer';
import { inferMediaKind } from './postCard.utils';

type Props = {
    post: Post;
    canDelete?: boolean;
    onDelete?: (postId: string) => void | Promise<void>;
    onToggleLike?: (post: Post) => void | Promise<void>;
    onToggleRepost?: (post: Post) => void | Promise<void>;
    onAddComment?: (post: Post, content: string) => void | Promise<void>;
    onDeleteComment?: (post: Post, commentId: string) => void | Promise<void>;
    enableDetailsNavigation?: boolean;
    maxVisibleComments?: number;
    isNested?: boolean;
};

export const PostCard: React.FC<Props> = ({
    post,
    canDelete = false,
    onDelete,
    onToggleLike,
    onToggleRepost,
    onAddComment,
    onDeleteComment,
    enableDetailsNavigation = false,
    maxVisibleComments,
    isNested = false,
}) => {
    const [composerOpen, setComposerOpen] = React.useState(false);
    const [mediaViewerOpen, setMediaViewerOpen] = React.useState(false);
    const navigate = useNavigate();
    const mediaKind = useMemo(
        () => inferMediaKind(post.mediaType, post.mediaUrl),
        [post.mediaType, post.mediaUrl],
    );
    const detailsHref = `/posts/${post.id}`;
    const isCardClickable = enableDetailsNavigation;
    const canPreviewMedia = mediaKind === 'image' || mediaKind === 'video';

    useEffect(() => {
        if (!mediaViewerOpen) {
            return;
        }

        function handleKeyDown(event: KeyboardEvent) {
            if (event.key === 'Escape') {
                setMediaViewerOpen(false);
            }
        }

        document.addEventListener('keydown', handleKeyDown);
        return () => document.removeEventListener('keydown', handleKeyDown);
    }, [mediaViewerOpen]);

    return (
        <article
            className={`${styles.card} ${isNested ? styles.repostCard : ''} ${
                isCardClickable ? styles.cardClickable : ''
            }`}
            onClick={(event) => {
                if (isCardClickable) {
                    event.stopPropagation();
                    navigate(detailsHref);
                }
            }}
        >
            <PostHeader
                post={post}
                canDelete={canDelete}
                onDelete={onDelete}
            />

            {post.content && <div className={styles.content}>{post.content}</div>}

            {post.mediaUrl && (
                <div className={styles.mediaWrap}>
                    {mediaKind === 'image' && (
                        <img
                            className={`${styles.image} ${canPreviewMedia ? styles.previewableMedia : ''}`}
                            src={post.mediaUrl}
                            alt="Post attachment"
                            onClick={(event) => {
                                event.stopPropagation();
                                setMediaViewerOpen(true);
                            }}
                        />
                    )}
                    {mediaKind === 'video' && (
                        <video
                            className={`${styles.video} ${canPreviewMedia ? styles.previewableMedia : ''}`}
                            src={post.mediaUrl}
                            controls
                            preload="metadata"
                            onClick={(event) => {
                                event.stopPropagation();
                                setMediaViewerOpen(true);
                            }}
                        />
                    )}
                    {mediaKind === 'audio' && (
                        <audio
                            className={styles.audio}
                            src={post.mediaUrl}
                            controls
                            preload="metadata"
                            onClick={(event) => event.stopPropagation()}
                        />
                    )}
                    {mediaKind === 'file' && (
                        <a
                            className={styles.fileLink}
                            href={post.mediaUrl}
                            target="_blank"
                            rel="noreferrer"
                            onClick={(event) => event.stopPropagation()}
                        >
                            Open attachment
                        </a>
                    )}
                </div>
            )}

            {post.originalPost && (
                <PostCard
                    post={post.originalPost}
                    enableDetailsNavigation={enableDetailsNavigation}
                    isNested
                />
            )}

            {!isNested && (
                <>
                    <PostActions
                        post={post}
                        showRepostAction={!post.originalPost}
                        onToggleComments={() => setComposerOpen((current) => !current)}
                        onToggleLike={onToggleLike}
                        onToggleRepost={onToggleRepost}
                    />
                    <div className={styles.commentsSection}>
                        {onAddComment && (
                            <div
                                className={`${styles.commentComposerShell} ${
                                    composerOpen ? styles.commentComposerShellOpen : ''
                                }`}
                            >
                                <PostCommentComposer
                                    onSubmit={async (content) => {
                                        await onAddComment(post, content);
                                    }}
                                />
                            </div>
                        )}
                        <PostComments
                            post={post}
                            comments={post.comments}
                            onDeleteComment={onDeleteComment}
                            maxVisibleComments={maxVisibleComments}
                            viewAllHref={isCardClickable ? detailsHref : undefined}
                        />
                    </div>
                </>
            )}

            {mediaViewerOpen && post.mediaUrl && canPreviewMedia && (
                <div
                    className={styles.mediaViewerBackdrop}
                    role="dialog"
                    aria-modal="true"
                    aria-label="Media viewer"
                    onClick={(event) => {
                        event.stopPropagation();
                        setMediaViewerOpen(false);
                    }}
                >
                    <button
                        type="button"
                        className={styles.mediaViewerClose}
                        aria-label="Close media viewer"
                        onClick={(event) => {
                            event.stopPropagation();
                            setMediaViewerOpen(false);
                        }}
                    >
                        <XMarkIcon width={24} height={24} />
                    </button>

                    <div
                        className={styles.mediaViewerContent}
                        onClick={(event) => event.stopPropagation()}
                    >
                        {mediaKind === 'image' && (
                            <img
                                className={styles.mediaViewerImage}
                                src={post.mediaUrl}
                                alt="Post attachment preview"
                            />
                        )}
                        {mediaKind === 'video' && (
                            <video
                                className={styles.mediaViewerVideo}
                                src={post.mediaUrl}
                                controls
                                autoPlay
                                preload="metadata"
                            />
                        )}
                    </div>
                </div>
            )}
        </article>
    );
};
