import React from 'react';
import { useParams } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode';

import styles from './ProfilePage.module.css';
import LoginForm from '../../features/auth/ui/LoginForm';
import RegisterForm from '../../features/auth/ui/RegisterForm';

import { useProfile } from '../../features/user/profileHeader/model/useProfile';

import { ProfileHeaderCard } from '../../features/user/profileHeader/ui/ProfileHeaderCard';
import { EditProfilePanel } from '../../features/user/profileHeader/ui/EditProfilePanel/EditProfilePanel';

type JwtPayload = { sub: string; username: string; email: string };

const ProfilePage: React.FC = () => {
    const params = useParams<{ id?: string }>();

    const [isLoginView, setIsLoginView] = React.useState(true);
    const [myId, setMyId] = React.useState<string | null>(null);

    React.useEffect(() => {
        const token = localStorage.getItem('token');
        if (!token) { setMyId(null); return; }
        try {
            const { sub } = jwtDecode<JwtPayload>(token);
            setMyId(sub);
        } catch {
            setMyId(null);
        }
    }, []);


    const targetUserId = params.id ?? myId ?? undefined;

    const { profile, setProfile, loading, error, refresh, toggleFollow, followLoading, updateProfile,
    addGenre, removeGenre, addSkill, removeSkill} = useProfile(targetUserId);

    const [editOpen, setEditOpen] = React.useState(false);

    if (!params.id && !myId) {
        return (
            <div className={styles.wrapper}>
                <div className={styles.loginForm}>
                    {isLoginView ? (
                        <>
                            <LoginForm onLogin={() => window.location.reload()} />
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


    function handleLogout() {
        localStorage.removeItem('token');
        window.location.reload();
    }

    return (
        <div>
            {error && <p className={styles.error}>{error}</p>}
            {loading && <p className={styles.info}>Loading profile…</p>}


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
                        />
                    </>
                )}
            </div>
        </div>
    );
};

export default ProfilePage;
