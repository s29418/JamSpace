import type { Post } from '../../../entities/post/model/types';

export function updatePostsById(
    posts: Post[],
    postId: string,
    updater: (post: Post) => Post,
): Post[] {
    return posts.map((post) => updatePostById(post, postId, updater));
}

function updatePostById(
    post: Post,
    postId: string,
    updater: (post: Post) => Post,
): Post {
    const nextPost = post.id === postId ? updater(post) : post;

    if (!nextPost.originalPost) {
        return nextPost;
    }

    const nextOriginalPost = updatePostById(nextPost.originalPost, postId, updater);

    if (nextOriginalPost === nextPost.originalPost) {
        return nextPost;
    }

    return {
        ...nextPost,
        originalPost: nextOriginalPost,
    };
}

export function removeOwnRepostsForOriginal(
    posts: Post[],
    originalPostId: string,
    currentUserId?: string | null,
): Post[] {
    if (!currentUserId) {
        return posts;
    }

    return posts.filter(
        (post) => !(post.authorId === currentUserId && post.originalPost?.id === originalPostId),
    );
}
