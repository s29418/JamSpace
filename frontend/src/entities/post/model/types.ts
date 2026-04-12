export interface PostComment {
    id: string;
    content: string;
    createdAt: string;
    authorId: string;
    authorDisplayName?: string | null;
    authorAvatarUrl?: string | null;
}

export interface Post {
    id: string;
    content?: string | null;
    createdAt: string;
    mediaUrl?: string | null;
    mediaType?: string | null;
    authorId: string;
    authorDisplayName?: string | null;
    authorAvatarUrl?: string | null;
    likeCount: number;
    commentCount: number;
    repostCount: number;
    isLikedByCurrentUser: boolean;
    isRepostedByCurrentUser: boolean;
    originalPost?: Post | null;
    comments: PostComment[];
}

export interface CursorResult<T> {
    items: T[];
    nextCursor?: string | null;
    hasMore?: boolean;
}
