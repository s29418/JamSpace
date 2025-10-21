import React from "react";
import { UserProfile } from "../../../../../entities/user/model/types";
import styles from "./ProfileHeaderCard.module.css";
import {
    CogIcon as SettingsIcon,
    UserPlusIcon as FollowIcon,
    ChatBubbleOvalLeftIcon as MessageIcon
} from '@heroicons/react/24/outline';

type Props = {
    profile: UserProfile;
    isOwner: boolean;
    onEdit: () => void;
};

export const ProfileHeaderCard: React.FC<Props> = ({ profile, isOwner, onEdit }) => {
    return (
        <div className={styles.profileCard}>

            {/* Avatar */}
            <div className={styles.profileCardAvatar}>
                {profile.profilePictureUrl ? (
                    <img src={profile.profilePictureUrl} alt={`${profile.displayName} avatar`} />
                ) : (
                    <div className={styles.avatarFallback}>{profile.displayName?.[0] ?? "?"}</div>
                )}
            </div>

            {/* EDIT */}
            {isOwner && (
                <button
                    className={`${styles.button} ${styles.buttonEdit} ${styles.editTopRight}`}
                    onClick={onEdit}
                >
                    <SettingsIcon className={styles.icon} />
                    Edit profile
                </button>
            )}

            <div>
                <h1 className={styles.profileCardName}>{profile.displayName}</h1>
                {profile.location && <p className={styles.profileCardLocation}>{profile.location}</p>}

                <div className={styles.profileCardMeta}>
                    <span>{profile.followersCount ?? 0} followers</span>
                    <span>{profile.followingCount ?? 0} following</span>
                </div>

                {/* FOLLOW / MESSAGE */}
                {/*{!isOwner && (*/}
                    <div className={styles.actionsInline}>
                        <button className={`${styles.button} ${styles.buttonGhost}`}>
                            <FollowIcon className={styles.icon} />
                            Follow
                        </button>
                        <button className={`${styles.button} ${styles.buttonGhost}`}>
                            <MessageIcon className={styles.icon} />
                            Message
                        </button>
                    </div>
                {/*)}*/}


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
