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

export interface UserLite {
    id: string;
    username: string;
    displayName: string;
    profilePictureUrl?: string | null;
    isFollowing?: boolean;
}

export interface AuthContext {
    userId?: string | null;
}

export interface PagedResult<T> {
    items: T[];
    page: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
}

export interface UserSearchItem {
    id: string;
    username: string;
    displayName: string;
    profilePictureUrl?: string | null;
    location?: LocationDto | null;

    skills: string[];
    genres: string[];

    followersCount: number;

    isFollowing: boolean;
    isMe: boolean;
}