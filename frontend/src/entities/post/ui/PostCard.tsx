import React, { useEffect, useMemo } from 'react';
import { ArrowsPointingOutIcon, XMarkIcon } from '@heroicons/react/24/outline';
import { useLocation, useNavigate } from 'react-router-dom';
import type { Post } from '../model/types';
import styles from './PostCard.module.css';
import { PostHeader } from './PostHeader';
import { PostActions } from './PostActions';
import { PostComments } from './PostComments';
import { PostCommentComposer } from './PostCommentComposer';
import { PostAudioPlayer } from './PostAudioPlayer';
import { PostVideoPlayer } from './PostVideoPlayer';
import { inferMediaKind } from './postCard.utils';
import { saveScrollPosition } from '../../../shared/lib/scroll/postDetailsScroll';
import { PlatformLogo } from '../../../shared/ui/platform-logo/PlatformLogo';
import { toMediaProxyUrl } from '../../../shared/api/media';
import { applyExternalTrackEmbedTheme } from '../../portfolio-track/lib/externalTrackEmbed';

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
    const location = useLocation();
    const mediaKind = useMemo(
        () => inferMediaKind(post.mediaType, post.mediaUrl),
        [post.mediaType, post.mediaUrl],
    );
    const detailsHref = `/posts/${post.id}`;
    const isCardClickable = enableDetailsNavigation;
    const canPreviewMedia = mediaKind === 'image' || mediaKind === 'video';
    const portfolioTrack = post.portfolioTrack;
    const portfolioTrackArtworkUrl = portfolioTrack?.source === 'Upload'
        ? toMediaProxyUrl(portfolioTrack.artworkUrl)
        : portfolioTrack?.artworkUrl;
    const portfolioTrackFileUrl = portfolioTrack?.source === 'Upload'
        ? toMediaProxyUrl(portfolioTrack.fileUrl)
        : portfolioTrack?.fileUrl;
    const portfolioTrackEmbedUrl = portfolioTrack?.embedUrl && portfolioTrack.source !== 'Upload'
        ? applyExternalTrackEmbedTheme(portfolioTrack.source, portfolioTrack.embedUrl)
        : portfolioTrack?.embedUrl;
    const spotifyPlaylist = post.spotifyPlaylist;

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
                    saveScrollPosition(`${location.pathname}${location.search}`);
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
                        <div
                            className={styles.imageShell}
                            onClick={(event) => event.stopPropagation()}
                        >
                            <img
                                className={styles.image}
                                src={post.mediaUrl}
                                alt="Post attachment"
                            />

                            <button
                                type="button"
                                className={styles.imagePreviewButton}
                                aria-label="Open image preview"
                                onClick={(event) => {
                                    event.stopPropagation();
                                    setMediaViewerOpen(true);
                                }}
                            >
                                <ArrowsPointingOutIcon width={18} height={18} />
                            </button>
                        </div>
                    )}
                    {mediaKind === 'video' && (
                        <PostVideoPlayer
                            src={post.mediaUrl}
                            onOpenPreview={() => setMediaViewerOpen(true)}
                        />
                    )}
                    {mediaKind === 'audio' && (
                        <PostAudioPlayer
                            src={post.mediaUrl}
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

            {portfolioTrack && (
                <div className={styles.portfolioTrack}>
                    <div className={styles.portfolioTrackHeader}>
                        {(portfolioTrack.source === 'Spotify' || portfolioTrack.source === 'SoundCloud') ? (
                            <PlatformLogo provider={portfolioTrack.source} size={22} />
                        ) : (
                            <span className={styles.portfolioTrackUploadIcon}>♪</span>
                        )}
                        <span>{portfolioTrack.source}</span>
                    </div>

                    {portfolioTrackEmbedUrl && (
                        <iframe
                            className={styles.portfolioTrackEmbed}
                            src={portfolioTrackEmbedUrl}
                            title={`${portfolioTrack.source} player`}
                            allow="autoplay; clipboard-write; encrypted-media; fullscreen; picture-in-picture"
                            loading="lazy"
                            onClick={(event) => event.stopPropagation()}
                        />
                    )}

                    {!portfolioTrack.embedUrl && portfolioTrackFileUrl && (
                        <PostAudioPlayer
                            src={portfolioTrackFileUrl}
                            title={portfolioTrack.title}
                            artworkUrl={portfolioTrackArtworkUrl}
                        />
                    )}
                </div>
            )}

            {spotifyPlaylist && (
                <div className={styles.portfolioTrack}>
                    <div className={styles.portfolioTrackHeader}>
                        <PlatformLogo provider="Spotify" size={22} />
                        <span>Spotify playlist</span>
                        <strong>{spotifyPlaylist.title}</strong>
                    </div>

                    <iframe
                        className={styles.portfolioTrackEmbed}
                        src={spotifyPlaylist.embedUrl}
                        title={`${spotifyPlaylist.title} Spotify playlist`}
                        allow="autoplay; clipboard-write; encrypted-media; fullscreen; picture-in-picture"
                        loading="lazy"
                        onClick={(event) => event.stopPropagation()}
                    />
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
                            <PostVideoPlayer
                                src={post.mediaUrl}
                                autoPlay
                                variant="viewer"
                            />
                        )}
                    </div>
                </div>
            )}
        </article>
    );
};
