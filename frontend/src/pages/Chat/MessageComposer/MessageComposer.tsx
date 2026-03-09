import React, { FormEvent, KeyboardEvent, useRef, useState } from "react";
import { chatHub } from "shared/lib/realtime/chatHub";
import styles from "./MessageComposer.module.css";

type Props = {
    conversationId: string;
    onMessageSent?: (content: string) => void;
};

const MessageComposer = ({ conversationId, onMessageSent }: Props) => {
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
            await chatHub.sendMessage({
                conversationId,
                content: trimmed,
                replyToMessageId: null,
            });

            onMessageSent?.(trimmed);
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
                onChange={(e) => setValue(e.target.value)}
                onKeyDown={handleKeyDown}
                rows={1}
            />

            <button
                className={styles.button}
                type="submit"
                disabled={sending || !value.trim()}
            >
                Send
            </button>
        </form>
    );
};

export default MessageComposer;