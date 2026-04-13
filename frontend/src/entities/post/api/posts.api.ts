import { api } from '../../../shared/api/base';
import type { CursorResult, Post, PostComment } from '../model/types';

const ROOT = '/posts';

function normalizeComment(comment: any): PostComment {
    return {
        id: String(comment.id ?? comment.Id),
        content: comment.content ?? '',
        createdAt: comment.createdAt ?? comment.CreatedAt,
        authorId: String(comment.authorId ?? comment.AuthorId ?? comment.userId ?? comment.UserId),
        authorDisplayName:
            comment.authorDisplayName ??
            comment.AuthorDisplayName ??
            comment.userDisplayName ??
            comment.UserDisplayName ??
            null,
        authorAvatarUrl:
            comment.authorAvatarUrl ??
            comment.AuthorAvatarUrl ??
            comment.userProfilePictureUrl ??
            comment.UserProfilePictureUrl ??
            null,
    };
}

function normalizePost(dto: any): Post {
    return {
        id: String(dto.id),
        content: dto.content ?? null,
        createdAt: dto.createdAt,
        mediaUrl: dto.mediaUrl ?? null,
        mediaType: dto.mediaType ?? null,
        authorId: String(dto.authorId),
        authorDisplayName: dto.authorDisplayName ?? null,
        authorAvatarUrl: dto.authorAvatarUrl ?? null,
        likeCount: Number(dto.likeCount ?? 0),
        commentCount: Number(dto.commentCount ?? 0),
        repostCount: Number(dto.repostCount ?? 0),
        isLikedByCurrentUser: Boolean(dto.isLikedByCurrentUser),
        isRepostedByCurrentUser: Boolean(dto.isRepostedByCurrentUser),
        originalPost: dto.originalPost ? normalizePost(dto.originalPost) : null,
        comments: Array.isArray(dto.comments)
            ? dto.comments.map(normalizeComment)
            : [],
    };
}

function normalizeCursorResult(dto: any): CursorResult<Post> {
    return {
        items: Array.isArray(dto.items) ? dto.items.map(normalizePost) : [],
        nextCursor: dto.nextCursor ?? dto.next ?? dto.cursor ?? null,
        hasMore: Boolean(dto.hasMore ?? dto.nextCursor ?? dto.next ?? dto.cursor),
    };
}

export async function getFollowedFeed(before?: string) {
    const res = await api.get<CursorResult<Post>>(ROOT + '/feed', {
        params: {
            ...(before ? { before } : {}),
        },
    });

    return normalizeCursorResult(res.data);
}

export async function getExploreFeed(before?: string) {
    const res = await api.get<CursorResult<Post>>(ROOT + '/explore', {
        params: {
            ...(before ? { before } : {}),
        },
    });

    return normalizeCursorResult(res.data);
}

export async function getUserPosts(userId: string, before?: string) {
    const res = await api.get<CursorResult<Post>>(`/users/${userId}/posts`, {
        params: {
            ...(before ? { before } : {}),
        },
    });

    return normalizeCursorResult(res.data);
}

export async function getPost(postId: string) {
    const res = await api.get<Post>(`${ROOT}/${postId}`);
    return normalizePost(res.data);
}

export async function createPost(content: string, file?: File | null) {
    const formData = new FormData();

    if (content.trim()) {
        formData.append('content', content.trim());
    }

    if (file) {
        formData.append('file', file);
    }

    const res = await api.post<Post>(ROOT, formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
    });

    return normalizePost(res.data);
}

export async function deletePost(postId: string) {
    await api.delete(`${ROOT}/${postId}`);
}

export async function likePost(postId: string) {
    await api.post(`${ROOT}/${postId}/likes`);
}

export async function unlikePost(postId: string) {
    await api.delete(`${ROOT}/${postId}/likes`);
}

export async function repostPost(postId: string) {
    const res = await api.post<Post>(`${ROOT}/${postId}/repost`);
    return normalizePost(res.data);
}

export async function deleteRepost(postId: string) {
    await api.delete(`${ROOT}/${postId}/repost`);
}

export async function createComment(postId: string, content: string) {
    const res = await api.post<PostComment>(`${ROOT}/${postId}/comments`, JSON.stringify(content), {
        headers: { 'Content-Type': 'application/json' },
    });

    return normalizeComment(res.data);
}

export async function deleteComment(postId: string, commentId: string) {
    await api.delete(`${ROOT}/${postId}/comments/${commentId}`);
}
