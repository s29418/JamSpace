import React, { useEffect, useState } from 'react';
import { useLocation, useNavigationType } from 'react-router-dom';
import { usePostsFeed } from '../../../features/post/model/usePostsFeed';
import { PostFeed } from '../../../widgets/post-feed/ui/PostFeed';
import styles from './HomePage.module.css';
import { useToast } from '../../../shared/lib/hooks/useToast';
import { isApiError } from '../../../shared/api/base';
import { useAuthState } from '../../../shared/lib/hooks/useAuthState';
import { PostComposer } from '../../../features/post/ui/PostComposer';
import { restoreScrollPosition } from '../../../shared/lib/scroll/postDetailsScroll';
import { getUserPortfolioTracks } from '../../../entities/portfolio-track/api/portfolioTracks.api';
import type { PortfolioTrack } from '../../../entities/portfolio-track/model/types';
import type { CreatePostSpotifyPlaylist } from '../../../entities/post/api/posts.api';
import {
    getMySpotifyPlaylists,
    SpotifyPlaylist
} from '../../../entities/user/api/externalAccounts.api';

const HomePage = () => {
    const { posts, loading, error, addPost, removePost, toggleLike, toggleRepost, addComment, removeComment } = usePostsFeed({ mode: 'auto' });
    const { isAuthenticated, currentUserId } = useAuthState();
    const { message, showError, showSuccess } = useToast();
    const location = useLocation();
    const navigationType = useNavigationType();
    const [portfolioTracks, setPortfolioTracks] = useState<PortfolioTrack[]>([]);
    const [spotifyPlaylists, setSpotifyPlaylists] = useState<SpotifyPlaylist[]>([]);
    const [spotifyPlaylistsLoading, setSpotifyPlaylistsLoading] = useState(false);
    const [spotifyPlaylistsError, setSpotifyPlaylistsError] = useState<string | null>(null);

    useEffect(() => {
        if (navigationType === 'POP' && !loading) {
            restoreScrollPosition(`${location.pathname}${location.search}`);
        }
    }, [loading, location.pathname, location.search, navigationType]);

    useEffect(() => {
        if (!isAuthenticated || !currentUserId) {
            setPortfolioTracks([]);
            setSpotifyPlaylists([]);
            return;
        }

        let alive = true;

        (async () => {
            try {
                const tracks = await getUserPortfolioTracks(currentUserId);
                if (alive) setPortfolioTracks(tracks);
            } catch {
                if (alive) setPortfolioTracks([]);
            }
        })();

        return () => {
            alive = false;
        };
    }, [isAuthenticated, currentUserId]);

    async function loadSpotifyPlaylists() {
        if (!isAuthenticated || !currentUserId || spotifyPlaylistsLoading) return;

        setSpotifyPlaylistsLoading(true);
        setSpotifyPlaylistsError(null);

        try {
            const playlists = await getMySpotifyPlaylists();
            setSpotifyPlaylists(playlists);
        } catch (e) {
            setSpotifyPlaylistsError(isApiError(e) ? e.message : 'Could not load Spotify playlists.');
            setSpotifyPlaylists([]);
        } finally {
            setSpotifyPlaylistsLoading(false);
        }
    }

    async function handleCreatePost(
        content: string,
        file?: File | null,
        portfolioTrackId?: string | null,
        spotifyPlaylist?: CreatePostSpotifyPlaylist | null,
    ) {
        try {
            await addPost(content, file, portfolioTrackId, spotifyPlaylist);
            showSuccess('Post published.');
        } catch (e) {
            throw new Error(isApiError(e) ? e.message : 'Failed to publish post.');
        }
    }

    async function handleToggleLike(post: Parameters<typeof toggleLike>[0]) {
        try {
            await toggleLike(post);
        } catch (e) {
            showError(isApiError(e) ? e.message : 'Failed to update like.');
        }
    }

    async function handleToggleRepost(post: Parameters<typeof toggleRepost>[0]) {
        try {
            await toggleRepost(post);
        } catch (e) {
            showError(isApiError(e) ? e.message : 'Failed to update repost.');
        }
    }

    async function handleAddComment(post: Parameters<typeof addComment>[0], content: string) {
        try {
            await addComment(post, content);
        } catch (e) {
            throw new Error(isApiError(e) ? e.message : 'Failed to add comment.');
        }
    }

    async function handleDeleteComment(post: Parameters<typeof removeComment>[0], commentId: string) {
        try {
            await removeComment(post, commentId);
        } catch (e) {
            showError(isApiError(e) ? e.message : 'Failed to delete comment.');
        }
    }

    async function handleDeletePost(postId: string) {
        try {
            await removePost(postId);
            showSuccess('Post deleted.');
        } catch (e) {
            showError(isApiError(e) ? e.message : 'Failed to delete post.');
        }
    }

    return (
        <main className={styles.page}>
            <div className={styles.content}>
                {message && (
                    <p style={{ color: message.color, marginBottom: 16, justifySelf: "center" }}>{message.text}</p>
                )}
                {isAuthenticated && (
                    <PostComposer
                        onSubmit={handleCreatePost}
                        portfolioTracks={portfolioTracks}
                        spotifyPlaylists={spotifyPlaylists}
                        spotifyPlaylistsLoading={spotifyPlaylistsLoading}
                        spotifyPlaylistsError={spotifyPlaylistsError}
                        onRefreshSpotifyPlaylists={loadSpotifyPlaylists}
                    />
                )}
                <PostFeed
                    posts={posts}
                    loading={loading}
                    error={error}
                    emptyText="No posts to show yet."
                    currentUserId={currentUserId}
                    onDeletePost={handleDeletePost}
                    onToggleLike={handleToggleLike}
                    onToggleRepost={handleToggleRepost}
                    onAddComment={handleAddComment}
                    onDeleteComment={handleDeleteComment}
                />
            </div>
        </main>
    );
};

export default HomePage;
