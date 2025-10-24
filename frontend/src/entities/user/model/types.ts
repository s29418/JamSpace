export interface UserProfile {
    id: string;
    username: string;
    displayName: string;
    email?: string | null;
    bio?: string | null;
    location?: LocationDto | null;
    profilePictureUrl?: string | null;

    followersCount?: number;
    followingCount?: number;
    isFollowing?: boolean;

    skills?: UserTag[];
    genres?: UserTag[];
}

export interface LocationDto {
    city?: string | null;
    country?: string | null;
}

export interface UserTag {
    id: string;
    name: string;
}

export interface AuthContext {
    userId?: string | null;
}