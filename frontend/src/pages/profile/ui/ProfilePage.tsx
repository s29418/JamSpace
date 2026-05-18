import React, { FC, useEffect, useState } from 'react';
import { useLocation, useNavigate, useNavigationType, useParams } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode';

import styles from './ProfilePage.module.css';
import LoginForm from '../../../features/auth/ui/LoginForm';
import RegisterForm from '../../../features/auth/ui/RegisterForm';

import { useProfile } from '../../../features/user/profile/model/useProfile';
import { logout } from '../../../entities/user/api/auth.api';
import { getOrCreateDirectConversation } from '../../../entities/chat/api/conversations.api';

import { ProfileHeaderCard } from '../../../widgets/profile-header/ui/ProfileHeaderCard';
import { EditProfilePanel } from '../../../features/user/edit-profile/ui/EditProfilePanel';
import type { EditProfilePanelTab } from '../../../features/user/edit-profile/ui/EditProfilePanel';
import { ProfileHeaderCardSkeleton } from "../../../widgets/profile-header/ui/ProfileHeaderCardSkeleton";
import { getToken, clearToken } from '../../../shared/lib/auth/token';
import { chatHub } from '../../../shared/lib/realtime/chatHub';
import { useUserPosts } from '../../../features/post/model/useUserPosts';
import { PostFeed } from '../../../widgets/post-feed/ui/PostFeed';
import { useToast } from '../../../shared/lib/hooks/useToast';
import { isApiError } from '../../../shared/api/base';
import { PostComposer } from '../../../features/post/ui/PostComposer';
import { restoreScrollPosition } from '../../../shared/lib/scroll/postDetailsScroll';
import {
    getUserExternalAccounts,
    PublicUserExternalAccount
} from '../../../entities/user/api/externalAccounts.api';
import { PortfolioTracksSection } from '../../../widgets/portfolio-tracks/ui/PortfolioTracksSection';
import {
    addExternalPortfolioTrack,
    AddExternalPortfolioTrackRequest,
    deletePortfolioTrack,
    getUserPortfolioTracks,
    uploadPortfolioTrack,
    UploadPortfolioTrackRequest
} from '../../../entities/portfolio-track/api/portfolioTracks.api';
import type { PortfolioTrack } from '../../../entities/portfolio-track/model/types';

type JwtPayload = { sub: string; username: string; email: string };
type ProfileContentTab = 'posts' | 'portfolio';

const ProfilePage: FC = () => {
    const params = useParams<{ id?: string }>();
    const navigate = useNavigate();
    const location = useLocation();
    const navigationType = useNavigationType();

    const [isLoginView, setIsLoginView] = useState(true);
    const [myId, setMyId] = useState<string | null>(null);

    useEffect(() => {
        const token = getToken();
        if (!token) {
            setMyId(null);
            return;
        }

        try {
            const { sub } = jwtDecode<JwtPayload>(token);
            setMyId(sub);
        } catch {
            setMyId(null);
        }
    }, []);

    const handleLoggedIn = () => {
        const token = getToken();
        if (!token) return;

        try {
            const { sub } = jwtDecode<JwtPayload>(token);
            setMyId(sub);
        } catch {}
    };

    const targetUserId = params.id ?? myId ?? undefined;

    const {
        profile,
        loading,
        error,
        toggleFollow,
        followLoading,
        updateProfile,
        addGenre,
        removeGenre,
        addSkill,
        removeSkill,
        updateEmail,
        changePassword,
        deleteAccount,
        logoutAll,
    } = useProfile(targetUserId);

    const [editOpen, setEditOpen] = useState(false);
    const [editInitialTab, setEditInitialTab] = useState<EditProfilePanelTab>('profile');
    const [platformsRefreshKey, setPlatformsRefreshKey] = useState(0);
    const [externalAccounts, setExternalAccounts] = useState<PublicUserExternalAccount[]>([]);
    const [portfolioTracks, setPortfolioTracks] = useState<PortfolioTrack[]>([]);
    const [portfolioLoading, setPortfolioLoading] = useState(false);
    const [portfolioError, setPortfolioError] = useState<string | null>(null);
    const [contentTab, setContentTab] = useState<ProfileContentTab>('posts');
    const {
        posts,
        loading: postsLoading,
        error: postsError,
        addPost,
        removePost,
        toggleLike,
        toggleRepost,
        addComment,
        removeComment,
    } = useUserPosts(targetUserId);
    const { message, showSuccess, showError } = useToast();

    useEffect(() => {
        if (!profile?.id) {
            setExternalAccounts([]);
            return;
        }

        let alive = true;

        (async () => {
            try {
                const accounts = await getUserExternalAccounts(profile.id);
                if (alive) setExternalAccounts(accounts);
            } catch {
                if (alive) setExternalAccounts([]);
            }
        })();

        return () => {
            alive = false;
        };
    }, [profile?.id, platformsRefreshKey]);

    useEffect(() => {
        if (!profile?.id) {
            setPortfolioTracks([]);
            return;
        }

        let alive = true;
        setPortfolioLoading(true);
        setPortfolioError(null);

        (async () => {
            try {
                const tracks = await getUserPortfolioTracks(profile.id);
                if (alive) setPortfolioTracks(tracks);
            } catch (e) {
                if (alive) {
                    setPortfolioError(isApiError(e) ? e.message : 'Failed to load portfolio.');
                    setPortfolioTracks([]);
                }
            } finally {
                if (alive) setPortfolioLoading(false);
            }
        })();

        return () => {
            alive = false;
        };
    }, [profile?.id]);

    useEffect(() => {
        const params = new URLSearchParams(location.search);
        const settingsTab = params.get('settingsTab');
        const externalAccountConnected = params.get('externalAccountConnected');
        const externalAccountProvider = params.get('externalAccountProvider');

        if (settingsTab !== 'platforms' && externalAccountConnected === null) return;

        setEditInitialTab('platforms');
        setEditOpen(true);

        if (externalAccountConnected === 'true') {
            setPlatformsRefreshKey((value) => value + 1);
            showSuccess(`${externalAccountProvider ?? 'Platform'} connected.`);
        } else if (externalAccountConnected === 'false') {
            showError(`Could not connect ${externalAccountProvider ?? 'platform'}.`);
        }

        params.delete('settingsTab');
        params.delete('externalAccountConnected');
        params.delete('externalAccountProvider');

        const nextSearch = params.toString();
        navigate(
            {
                pathname: location.pathname,
                search: nextSearch ? `?${nextSearch}` : '',
            },
            { replace: true },
        );
    }, [location.pathname, location.search, navigate, showError, showSuccess]);

    useEffect(() => {
        if (navigationType === 'POP' && !loading && !postsLoading) {
            restoreScrollPosition(`${location.pathname}${location.search}`);
        }
    }, [loading, postsLoading, location.pathname, location.search, navigationType]);

    if (!params.id && !myId) {
        return (
            <div className={styles.wrapper}>
                <div className={styles.loginForm}>
                    {isLoginView ? (
                        <>
                            <LoginForm onLogin={handleLoggedIn} />
                            <p>
                                Don’t have an account?{' '}
                                <span onClick={() => setIsLoginView(false)}>Sign up!</span>
                            </p>
                        </>
                    ) : (
                        <>
                            <RegisterForm />
                            <p>
                                Already have an account?{' '}
                                <span onClick={() => setIsLoginView(true)}>Log in!</span>
                            </p>
                        </>
                    )}
                </div>
            </div>
        );
    }

    const isOwner = !!profile && !!myId && profile.id === myId;

    async function handleLogout() {
        try {
            await logout();
            await chatHub.stop();
        } catch (e) {
            console.error("logout error", e);
        }

        clearToken();
        setMyId(null);
        navigate('/profile');
    }

    async function handleLogoutAll() {
        try {
            await logoutAll();
            await chatHub.stop();
        } catch {}

        clearToken();
        setMyId(null);
        navigate('/profile');
    }

    async function handleDeleteAccount() {
        try {
            await deleteAccount();
            await chatHub.stop();
        } catch {}

        clearToken();
        setMyId(null);
        navigate('/profile');
    }

    async function handleMessage() {
        if (!profile) return;

        try {
            const result = await getOrCreateDirectConversation({
                otherUserId: profile.id,
            });

            navigate(`/chat?conversationId=${result.conversationId}`);
        } catch (error) {
            console.error("Failed to open direct conversation", error);
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

    async function handleCreatePost(content: string, file?: File | null) {
        try {
            await addPost(content, file);
            showSuccess('Post published.');
        } catch (e) {
            throw new Error(isApiError(e) ? e.message : 'Failed to publish post.');
        }
    }

    async function handleAddExternalTrack(request: AddExternalPortfolioTrackRequest) {
        try {
            const track = await addExternalPortfolioTrack(request);
            setPortfolioTracks((current) => [track, ...current]);
            showSuccess('Track added to portfolio.');
        } catch (e) {
            throw new Error(isApiError(e) ? e.message : 'Failed to add track.');
        }
    }

    async function handleDeletePortfolioTrack(trackId: string) {
        try {
            await deletePortfolioTrack(trackId);
            setPortfolioTracks((current) => current.filter((track) => track.id !== trackId));
            showSuccess('Track removed from portfolio.');
        } catch (e) {
            showError(isApiError(e) ? e.message : 'Failed to remove track.');
        }
    }

    async function handleUploadPortfolioTrack(request: UploadPortfolioTrackRequest) {
        try {
            const track = await uploadPortfolioTrack(request);
            setPortfolioTracks((current) => [track, ...current]);
            showSuccess('Track uploaded to portfolio.');
        } catch (e) {
            throw new Error(isApiError(e) ? e.message : 'Failed to upload track.');
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
            showSuccess('Comment deleted.');
        } catch (e) {
            showError(isApiError(e) ? e.message : 'Failed to delete comment.');
        }
    }

    return (
        <div>
            {error && <p className={styles.error}>{error}</p>}
            {loading && <ProfileHeaderCardSkeleton />}

            <div className={styles.wrapper}>
                {profile && (
                    <div className={styles.profileContent}>
                        <ProfileHeaderCard
                            profile={profile}
                            isOwner={isOwner}
                            externalAccounts={externalAccounts}
                            onEdit={() => {
                                setEditInitialTab('profile');
                                setEditOpen(true);
                            }}
                            onLogout={isOwner ? handleLogout : undefined}
                            onToggleFollow={!isOwner ? toggleFollow : undefined}
                            onMessage={!isOwner ? handleMessage : undefined}
                            followLoading={!isOwner ? followLoading : undefined}
                            onOpenFollowers={() => navigate(`/profile/${profile.id}/followers`)}
                            onOpenFollowing={() => navigate(`/profile/${profile.id}/following`)}
                        />

                        <EditProfilePanel
                            isOpen={editOpen}
                            onClose={() => setEditOpen(false)}
                            initialTab={editInitialTab}
                            profile={profile}
                            platformsRefreshKey={platformsRefreshKey}
                            onSaveProfile={updateProfile}
                            addGenre={addGenre}
                            removeGenre={removeGenre}
                            addSkill={addSkill}
                            removeSkill={removeSkill}
                            onUpdateEmail={updateEmail}
                            onChangePassword={async (oldPassword, newPassword) => {
                                await changePassword(oldPassword, newPassword);
                                await handleLogoutAll();
                            }}
                            onDeleteAccount={handleDeleteAccount}
                            onLogoutAll={handleLogoutAll}
                        />

                        {message && (
                            <p style={{ color: message.color }}>{message.text}</p>
                        )}

                        <div className={styles.contentTabs} role="tablist" aria-label="Profile content">
                            <button
                                type="button"
                                role="tab"
                                aria-selected={contentTab === 'posts'}
                                className={contentTab === 'posts' ? styles.contentTabActive : ''}
                                onClick={() => setContentTab('posts')}
                            >
                                Posts
                            </button>

                            <button
                                type="button"
                                role="tab"
                                aria-selected={contentTab === 'portfolio'}
                                className={contentTab === 'portfolio' ? styles.contentTabActive : ''}
                                onClick={() => setContentTab('portfolio')}
                            >
                                Portfolio
                            </button>
                        </div>

                        {contentTab === 'posts' ? (
                            <>
                                {isOwner && <PostComposer onSubmit={handleCreatePost} />}

                                <PostFeed
                                    posts={posts}
                                    loading={postsLoading}
                                    error={postsError}
                                    emptyText="This user hasn't published any posts yet."
                                    currentUserId={myId}
                                    onDeletePost={isOwner ? handleDeletePost : undefined}
                                    onToggleLike={handleToggleLike}
                                    onToggleRepost={handleToggleRepost}
                                    onAddComment={handleAddComment}
                                    onDeleteComment={handleDeleteComment}
                                />
                            </>
                        ) : (
                            <PortfolioTracksSection
                                tracks={portfolioTracks}
                                loading={portfolioLoading}
                                error={portfolioError}
                                canAdd={isOwner}
                                onAddExternalTrack={isOwner ? handleAddExternalTrack : undefined}
                                onUploadTrack={isOwner ? handleUploadPortfolioTrack : undefined}
                                canDelete={isOwner}
                                onDeleteTrack={isOwner ? handleDeletePortfolioTrack : undefined}
                            />
                        )}
                    </div>
                )}
            </div>
        </div>
    );
};

export default ProfilePage;
