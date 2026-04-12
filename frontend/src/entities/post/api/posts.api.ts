import { api } from '../../../shared/api/base';
import type { CursorResult, Post } from '../model/types';

const ROOT = '/posts';

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
            ? dto.comments.map((comment: any) => ({
                id: String(comment.id),
                content: comment.content ?? '',
                createdAt: comment.createdAt,
                authorId: String(comment.authorId),
                authorDisplayName: comment.authorDisplayName ?? null,
                authorAvatarUrl: comment.authorAvatarUrl ?? null,
            }))
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

export async function deletePost(postId: string) {
    await api.delete(`${ROOT}/${postId}`);
}
