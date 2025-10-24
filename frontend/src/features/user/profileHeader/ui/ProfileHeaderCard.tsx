import React from "react";
import { UserProfile } from "../../../../entities/user/model/types";
import styles from "./ProfileHeaderCard.module.css";
import {
    CogIcon as SettingsIcon,
    UserPlusIcon as FollowIcon,
    UserMinusIcon as UnfollowIcon,
    ChatBubbleOvalLeftIcon as MessageIcon,
    ArrowRightStartOnRectangleIcon as LogoutIcon
} from '@heroicons/react/24/outline';

type Props = {
    profile: UserProfile;
    isOwner: boolean;
    onEdit: () => void;
    onLogout?: () => void;
    onToggleFollow?: () => void;
    followLoading?: boolean;
    onOpenFollowers?: () => void;
    onOpenFollowing?: () => void;
};

export const ProfileHeaderCard: React.FC<Props> = ({
                                                       profile,
                                                       isOwner,
                                                       onEdit ,
                                                       onLogout,
                                                       onToggleFollow,
                                                       followLoading,
                                                       onOpenFollowers,
                                                       onOpenFollowing
}) => {
    return (
        <div className={styles.profileCard}>

            {/* Avatar */}
            <div className={styles.profileCardAvatar}>
                {profile.profilePictureUrl ? (
                    <img src={profile.profilePictureUrl} alt={`${profile.username} avatar`} />
                ) : (
                    <div className={styles.avatarFallback}>{profile.username?.[0] ?? "?"}</div>
                )}
            </div>

            {/* EDIT & LOGOUT*/}
            {isOwner && (
                <div className={styles.editTopRight}>
                    <button
                        className={`${styles.button} ${styles.buttonGhost}`}
                        onClick={onLogout}
                        type="button"
                    >
                        <LogoutIcon className={styles.icon} />
                        Logout
                    </button>

                    <button
                        className={`${styles.button} ${styles.buttonGhost}`}
                        onClick={onEdit}
                        type="button"
                    >
                        <SettingsIcon className={styles.icon} />
                        Edit profile
                    </button>
                </div>
            )}

            <div>
                <h1 className={styles.profileCardName}>{profile.displayName}</h1>
                <p className={styles.profileUserName}>@{profile.username}</p>
                {profile.location && <p className={styles.profileCardLocation}>
                    {profile.location.city}, {profile.location.country}
                </p>}

                <div className={styles.profileCardMeta}>
                    <button type="button" className={styles.metaLink} onClick={onOpenFollowers}>
                        {profile.followersCount ?? 0} followers
                    </button>

                    <button type="button" className={styles.metaLink} onClick={onOpenFollowing}>
                        {profile.followingCount ?? 0} following
                    </button>
                </div>

                {/* FOLLOW / MESSAGE */}
                {!isOwner && (
                    <div className={styles.actionsInline}>
                        <button
                            className={`${styles.button} ${styles.buttonGhost}`}
                            onClick={onToggleFollow}
                            disabled={!!followLoading}
                            type="button"
                        >

                            {profile.isFollowing ? (
                                <>
                                    <UnfollowIcon className={styles.icon}/>
                                    Unfollow
                                </>
                            ) : (
                                <>
                                    <FollowIcon className={styles.icon}/>
                                    Follow
                                </>
                            )}
                        </button>

                        <button className={`${styles.button} ${styles.buttonGhost}`} type="button">
                            <MessageIcon className={styles.icon}/>
                            Message
                        </button>
                    </div>
                )}


                <div className={styles.profileCardChipsRow}>
                    {(profile.skills ?? []).slice(0, 12).map(s => (
                        <span key={s.id} className={styles.chip}>{s.name}</span>
                    ))}
                </div>


                <div className={styles.profileCardChipsRow}>
                    {(profile.genres ?? []).slice(0, 12).map(g => (
                        <span key={g.id} className={`${styles.chip} ${styles.chipHollow}`}>{g.name}</span>
                    ))}
                </div>

                {profile.bio && <p className={styles.profileCardBio}>{profile.bio}</p>}
            </div>
        </div>
    );
};
