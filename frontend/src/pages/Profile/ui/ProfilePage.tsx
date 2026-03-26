import React, {FC, useEffect, useState} from 'react';
import { useParams } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode';

import styles from './ProfilePage.module.css';
import LoginForm from '../../../features/auth/ui/LoginForm';
import RegisterForm from '../../../features/auth/ui/RegisterForm';

import { useProfile } from '../../../features/user/profile/model/useProfile';

import { ProfileHeaderCard } from '../../../widgets/profile-header/ui/ProfileHeaderCard';
import { EditProfilePanel } from '../../../features/user/edit-profile/ui/EditProfilePanel';
import {ProfileHeaderCardSkeleton} from "../../../widgets/profile-header/ui/ProfileHeaderCardSkeleton";
import { getToken, clearToken } from '../../../shared/lib/auth/auth';
import { chatHub } from '../../../shared/lib/realtime/chatHub';

type JwtPayload = { sub: string; username: string; email: string };

const ProfilePage: FC = () => {
    const params = useParams<{ id?: string }>();

    const [isLoginView, setIsLoginView] = useState(true);
    const [myId, setMyId] = useState<string | null>(null);

    useEffect(() => {
        const token = getToken();
        if (!token) { setMyId(null); return; }
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
            } catch {  }
        };

    const targetUserId = params.id ?? myId ?? undefined;

    const { profile, loading, error, toggleFollow, followLoading, updateProfile,
    addGenre, removeGenre, addSkill, removeSkill, updateEmail, changePassword, deleteAccount, logoutAll
    } = useProfile(targetUserId);

    const [editOpen, setEditOpen] = useState(false);

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
        const token = localStorage.getItem("accessToken");
        try {
            await fetch(`http://localhost:5072/api/auth/logout`, {
                method: "POST",
                credentials: "include",
                headers: token ? { Authorization: `Bearer ${token}` } : undefined,
            });
            await chatHub.stop();
        } catch (e) {
            console.error("logout error", e);
        }

        clearToken();
        setMyId(null);
    }

    async function handleLogoutAll() {
        try{
            await logoutAll();
        } catch {}
        clearToken();
        setMyId(null);
    }

    return (
        <div>
            {error && <p className={styles.error}>{error}</p>}
            {loading && <ProfileHeaderCardSkeleton />}


            <div className={styles.wrapper}>

                {profile && (
                    <>
                        <ProfileHeaderCard
                            profile={profile}
                            isOwner={isOwner}
                            onEdit={() => setEditOpen(true)}
                            onLogout={isOwner ? handleLogout : undefined}
                            onToggleFollow={!isOwner ? toggleFollow : undefined}
                            followLoading={!isOwner ? followLoading : undefined}
                            onOpenFollowers={() => {
                                    window.location.href = `/profile/${profile.id}/followers`;
                            }}
                            onOpenFollowing={() => {
                                    window.location.href = `/profile/${profile.id}/following`;
                            }}
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
                            onDeleteAccount={deleteAccount}
                            onLogoutAll={handleLogoutAll}
                        />
                    </>
                )}
            </div>
        </div>
    );
};

export default ProfilePage;
