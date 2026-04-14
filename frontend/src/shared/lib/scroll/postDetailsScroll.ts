const SCROLL_KEY_PREFIX = 'jamspace:scroll:';

function getScrollKey(path: string) {
    return `${SCROLL_KEY_PREFIX}${path}`;
}

export function saveScrollPosition(path: string) {
    sessionStorage.setItem(getScrollKey(path), String(window.scrollY));
}

export function restoreScrollPosition(path: string) {
    const key = getScrollKey(path);
    const rawValue = sessionStorage.getItem(key);

    if (!rawValue) {
        return;
    }

    const targetScroll = Number(rawValue);

    if (!Number.isFinite(targetScroll)) {
        sessionStorage.removeItem(key);
        return;
    }

    let attempts = 0;

    const restore = () => {
        window.scrollTo({ top: targetScroll, left: 0, behavior: 'auto' });

        const maxScrollableTop = document.documentElement.scrollHeight - window.innerHeight;
        const isReachable = maxScrollableTop >= targetScroll - 4;

        if (isReachable || attempts >= 20) {
            sessionStorage.removeItem(key);
            return;
        }

        attempts += 1;
        window.requestAnimationFrame(restore);
    };

    window.requestAnimationFrame(restore);
}
