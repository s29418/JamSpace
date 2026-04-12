import React from 'react';
import { Link } from 'react-router-dom';
import type { PostComment } from '../model/types';
import { formatPostTimestamp } from './postCard.utils';
import styles from './PostCard.module.css';

type Props = {
    comments: PostComment[];
};

export const PostComments: React.FC<Props> = ({ comments }) => {
    if (!comments.length) {
        return null;
    }

    return (
        <div className={styles.comments}>
            {comments.map((comment) => {
                const commentAuthorName = comment.authorDisplayName?.trim() || 'Unknown user';

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
                                <span className={styles.commentTimestamp}>
                                    {formatPostTimestamp(comment.createdAt)}
                                </span>
                            </div>

                            <p className={styles.commentContent}>{comment.content}</p>
                        </div>
                    </div>
                );
            })}
        </div>
    );
};
