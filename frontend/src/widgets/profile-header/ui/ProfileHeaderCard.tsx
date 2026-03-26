import React, {useMemo} from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getOrCreateDirectConversation } from "entities/chat/api/conversations.api";

import { UserProfile } from "../../../entities/user/model/types";
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

function useRegionName(locale = "en") {
    const dn = useMemo(() => {
        try { return new Intl.DisplayNames([locale], { type: "region" }); }
        catch { return null; }
    }, [locale]);

    return (code?: string | null) =>
        !code ? undefined : (dn?.of(code.toUpperCase()) ?? code.toUpperCase());
}

export const ProfileHeaderCard: React.FC<Props> = (props) => {
    const { profile, isOwner, onEdit, onLogout, onToggleFollow, followLoading, onOpenFollowers, onOpenFollowing } = props;

    const regionName = useRegionName("en");

    const city = profile.location?.city?.trim();
    const countryCode = (profile.location as any)?.country ?? (profile.location as any)?.countryCode;
    const country = regionName(countryCode);

    const locationText =
        city && country ? `${city}, ${country}` :
            city ? city :
                country ? country :
                    null;

    const navigate = useNavigate();
    const { id } = useParams();

    const handleMessageClick = async () => {
        if (!id) return;

        try {
            const result = await getOrCreateDirectConversation({
                otherUserId: id,
            });

            navigate(`/chat?conversationId=${result.conversationId}`);
        } catch (error) {
            console.error("Failed to open direct conversation", error);
        }
    };

    return (
        <div className={styles.profileCard}>

            {/* Avatar */}
            <div className={styles.profileCardAvatar}>
                {profile.profilePictureUrl ? (
                    <img src={profile.profilePictureUrl} alt={`${profile.username} avatar`} />
                ) : (
                    <div className={styles.avatarFallback}>{profile.username?.[0].toUpperCase() ?? "?"}</div>
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

                {locationText && (
                    <p className={styles.profileCardLocation}>{locationText}</p>
                )}

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

                        <button
                            className={`${styles.button} ${styles.buttonGhost}`}
                            type="button"
                            onClick={() => void handleMessageClick()}
                        >
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
