export function formatPostTimestamp(createdAt: string) {
    const createdDate = new Date(createdAt);
    const now = new Date();
    let diffMs = now.getTime() - createdDate.getTime();

    if (Number.isNaN(createdDate.getTime())) {
        return '';
    }

    // Tolerate minor client/server clock skew so fresh items do not fall back to a date.
    if (diffMs < 0 && Math.abs(diffMs) <= 24 * 60 * 60 * 1000) {
        diffMs = Math.abs(diffMs);
    }

    if (diffMs < 0) {
        return '';
    }

    const diffMinutes = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMinutes < 1) return 'now';
    if (diffMinutes < 60) return `${diffMinutes} min`;
    if (diffHours < 24) return `${diffHours}h`;
    if (diffDays <= 7) return `${diffDays}d`;

    const day = String(createdDate.getDate()).padStart(2, '0');
    const month = String(createdDate.getMonth() + 1).padStart(2, '0');
    const currentYear = now.getFullYear();
    const year = String(createdDate.getFullYear()).slice(-2);

    return createdDate.getFullYear() === currentYear
        ? `${day}.${month}`
        : `${day}.${month}.${year}`;
}

export function formatPostTimestampSafe(createdAt?: string | null) {
    if (!createdAt) {
        return '';
    }

    const formatted = formatPostTimestamp(createdAt);

    if (formatted) {
        return formatted;
    }

    const createdDate = new Date(createdAt);

    if (Number.isNaN(createdDate.getTime())) {
        return '';
    }

    const day = String(createdDate.getDate()).padStart(2, '0');
    const month = String(createdDate.getMonth() + 1).padStart(2, '0');
    const currentYear = new Date().getFullYear();
    const year = String(createdDate.getFullYear()).slice(-2);

    return createdDate.getFullYear() === currentYear
        ? `${day}.${month}`
        : `${day}.${month}.${year}`;
}

export function inferMediaKind(mediaType?: string | null, mediaUrl?: string | null) {
    const mediaTypeValue = String(mediaType ?? '').trim().toLowerCase();
    const mediaUrlValue = String(mediaUrl ?? '').trim().toLowerCase();
    const source = `${mediaTypeValue} ${mediaUrlValue}`;

    const imageExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.webp', '.bmp', '.svg', '.avif'];
    const videoExtensions = ['.mp4', '.webm', '.mov', '.m4v', '.ogg', '.ogv', '.mkv'];
    const audioExtensions = ['.mp3', '.wav', '.aac', '.m4a', '.flac', '.oga', '.ogg'];

    if (mediaTypeValue === '0') return 'image';
    if (mediaTypeValue === '1') return 'video';
    if (mediaTypeValue === '2') return 'audio';

    if (source.includes('image')) return 'image';
    if (source.includes('video')) return 'video';
    if (source.includes('audio')) return 'audio';

    if (videoExtensions.some((extension) => mediaUrlValue.includes(extension))) return 'video';
    if (audioExtensions.some((extension) => mediaUrlValue.includes(extension))) return 'audio';
    if (imageExtensions.some((extension) => mediaUrlValue.includes(extension))) return 'image';

    if (mediaUrlValue.includes('/postvideo/')) return 'video';
    if (mediaUrlValue.includes('/postaudio/')) return 'audio';
    if (mediaUrlValue.includes('/postimage/')) return 'image';

    return 'file';
}
