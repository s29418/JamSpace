import React, { useMemo } from 'react';
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
    const navigate = useNavigate();
    const mediaKind = useMemo(
        () => inferMediaKind(post.mediaType, post.mediaUrl),
        [post.mediaType, post.mediaUrl],
    );
    const detailsHref = `/posts/${post.id}`;
    const isCardClickable = enableDetailsNavigation && !isNested;

    return (
        <article
            className={`${styles.card} ${isNested ? styles.repostCard : ''} ${
                isCardClickable ? styles.cardClickable : ''
            }`}
            onClick={() => {
                if (isCardClickable) {
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
        </article>
    );
};
