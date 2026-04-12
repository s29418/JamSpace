import React, { useMemo } from 'react';
import type { Post } from '../model/types';
import styles from './PostCard.module.css';
import { PostHeader } from './PostHeader';
import { PostActions } from './PostActions';
import { PostComments } from './PostComments';
import { inferMediaKind } from './postCard.utils';

type Props = {
    post: Post;
    canDelete?: boolean;
    onDelete?: (postId: string) => void | Promise<void>;
    onToggleLike?: (post: Post) => void | Promise<void>;
    onToggleRepost?: (post: Post) => void | Promise<void>;
    isNested?: boolean;
};

export const PostCard: React.FC<Props> = ({
    post,
    canDelete = false,
    onDelete,
    onToggleLike,
    onToggleRepost,
    isNested = false,
}) => {
    const mediaKind = useMemo(
        () => inferMediaKind(post.mediaType, post.mediaUrl),
        [post.mediaType, post.mediaUrl],
    );
    const actionPost = post.originalPost ?? post;

    return (
        <article className={`${styles.card} ${isNested ? styles.repostCard : ''}`}>
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
                        post={actionPost}
                        showRepostAction={!post.originalPost}
                        onToggleLike={onToggleLike}
                        onToggleRepost={onToggleRepost}
                    />
                    <PostComments comments={actionPost.comments} />
                </>
            )}
        </article>
    );
};
