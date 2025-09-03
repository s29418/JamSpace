import * as React from 'react';

export type InlineMessageType = 'success' | 'error' | 'info';
export type MessageState = { text: string; color: string } | null;

type Options = {
    defaultTimeoutMs?: number;
    palette?: Partial<Record<InlineMessageType, string>>;
};

const DEFAULT_PALETTE: Record<InlineMessageType, string> = {
    success: '#26cdd4',
    error: '#ef4444',
    info: '#9ca3af',
};

export function useToast(opts: Options = {}) {
    const {
        defaultTimeoutMs = 5000,
        palette = {},
    } = opts;

    const colors = { ...DEFAULT_PALETTE, ...palette };
    const [message, setMessage] = React.useState<MessageState>(null);
    const timeoutRef = React.useRef<number | null>(null);

    const clear = React.useCallback(() => {
        if (timeoutRef.current) {
            window.clearTimeout(timeoutRef.current);
            timeoutRef.current = null;
        }
        setMessage(null);
    }, []);

    const show = React.useCallback(
        (text: string, type: InlineMessageType = 'info', timeoutMs = defaultTimeoutMs) => {
            clear();
            setMessage({ text, color: colors[type] });
            timeoutRef.current = window.setTimeout(() => {
                setMessage(null);
                timeoutRef.current = null;
            }, timeoutMs);
        },
        [clear, colors, defaultTimeoutMs],
    );

    const showSuccess = React.useCallback(
        (text: string, timeoutMs?: number) => show(text, 'success', timeoutMs),
        [show],
    );
    const showError = React.useCallback(
        (text: string, timeoutMs?: number) => show(text, 'error', timeoutMs),
        [show],
    );
    const showInfo = React.useCallback(
        (text: string, timeoutMs?: number) => show(text, 'info', timeoutMs),
        [show],
    );

    React.useEffect(() => clear, [clear]);

    return { message, show, showSuccess, showError, showInfo, clear };
}
