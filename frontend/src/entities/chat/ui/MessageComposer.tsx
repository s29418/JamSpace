import React, { FormEvent, KeyboardEvent, useRef, useState } from "react";
import styles from "./MessageComposer.module.css";
import {
        PaperAirplaneIcon as SendIcon
} from '@heroicons/react/24/outline';

type Props = {
    conversationId: string;
    onMessageSent?: (content: string) => void;
    onTyping?: () => void;
    onStopTyping?: () => Promise<void>;
    onSendMessage: (content: string) => Promise<void>;
};

const MessageComposer = ({
                             onMessageSent,
                             onTyping,
                             onStopTyping,
                             onSendMessage,
                         }: Props) => {
    const [value, setValue] = useState("");
    const [sending, setSending] = useState(false);

    const inputRef = useRef<HTMLTextAreaElement | null>(null);

    const send = async () => {
        const trimmed = value.trim();
        if (!trimmed || sending) return;

        setValue("");
        inputRef.current?.focus();

        setSending(true);

        try {
            await onSendMessage(trimmed);
            onMessageSent?.(trimmed);
            await onStopTyping?.();
        } catch (error) {
            console.error("Failed to send message", error);
        } finally {
            setSending(false);
        }
    };

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();
        await send();
    };

    const handleKeyDown = async (e: KeyboardEvent<HTMLTextAreaElement>) => {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault();
            await send();
        }
    };

    return (
        <form className={styles.form} onSubmit={handleSubmit}>
            <textarea
                ref={inputRef}
                className={styles.input}
                placeholder="Write a message..."
                value={value}
                onChange={(e) => {
                    setValue(e.target.value);
                    onTyping?.();
                }}
                onKeyDown={handleKeyDown}
                rows={1}
            />

            <button
                className={styles.button}
                type="submit"
                disabled={sending || !value.trim()}
            >
                Send
                <SendIcon className={styles.icon}/>
            </button>
        </form>
    );
};

export default MessageComposer;