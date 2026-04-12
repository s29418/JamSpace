import React, { FC, useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode';

import styles from './ProfilePage.module.css';
import LoginForm from '../../../features/auth/ui/LoginForm';
import RegisterForm from '../../../features/auth/ui/RegisterForm';

import { useProfile } from '../../../features/user/profile/model/useProfile';
import { logout } from '../../../entities/user/api/auth.api';
import { getOrCreateDirectConversation } from '../../../entities/chat/api/conversations.api';

import { ProfileHeaderCard } from '../../../widgets/profile-header/ui/ProfileHeaderCard';
import { EditProfilePanel } from '../../../features/user/edit-profile/ui/EditProfilePanel';
import { ProfileHeaderCardSkeleton } from "../../../widgets/profile-header/ui/ProfileHeaderCardSkeleton";
import { getToken, clearToken } from '../../../shared/lib/auth/token';
import { chatHub } from '../../../shared/lib/realtime/chatHub';
import { useUserPosts } from '../../../features/post/model/useUserPosts';
import { PostFeed } from '../../../widgets/post-feed/ui/PostFeed';
import { useToast } from '../../../shared/lib/hooks/useToast';
import { isApiError } from '../../../shared/api/base';

type JwtPayload = { sub: string; username: string; email: string };

const ProfilePage: FC = () => {
    const params = useParams<{ id?: string }>();
    const navigate = useNavigate();

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
    const {
        posts,
        loading: postsLoading,
        error: postsError,
        removePost,
        toggleLike,
        toggleRepost,
    } = useUserPosts(targetUserId);
    const { message, showSuccess, showError } = useToast();

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
                            onEdit={() => setEditOpen(true)}
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
                            initialTab="profile"
                            profile={profile}
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

                        <PostFeed
                            posts={posts}
                            loading={postsLoading}
                            error={postsError}
                            emptyText="This user hasn't published any posts yet."
                            canDelete={isOwner}
                            onDeletePost={isOwner ? handleDeletePost : undefined}
                            onToggleLike={handleToggleLike}
                            onToggleRepost={handleToggleRepost}
                        />
                    </div>
                )}
            </div>
        </div>
    );
};

export default ProfilePage;
