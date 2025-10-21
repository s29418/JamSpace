export interface UserProfile {
    id: string;
    displayName: string;
    bio?: string | null;
    location?: string | null;
    profilePictureUrl?: string | null;
    followersCount?: number;
    followingCount?: number;
    skills?: { id: string; name: string }[];
    genres?: { id: string; name: string }[];
}

export interface AuthContext {
    userId?: string | null;
}