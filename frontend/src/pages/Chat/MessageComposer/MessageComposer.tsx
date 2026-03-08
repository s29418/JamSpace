import React, { FormEvent, useState } from "react";
import { chatHub } from "shared/lib/realtime/chatHub";
import styles from "./MessageComposer.module.css";

type Props = {
    conversationId: string;
};

const MessageComposer = ({ conversationId }: Props) => {
    const [value, setValue] = useState("");
    const [sending, setSending] = useState(false);

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();

        const trimmed = value.trim();
        if (!trimmed || sending) return;

        setSending(true);

        try {
            await chatHub.sendMessage({
                conversationId,
                content: trimmed,
                replyToMessageId: null,
            });

            setValue("");
        } catch (error) {
            console.error("Failed to send message", error);
        } finally {
            setSending(false);
        }
    };

    return (
        <form className={styles.form} onSubmit={handleSubmit}>
            <input
                className={styles.input}
                type="text"
                placeholder="Write a message..."
                value={value}
                onChange={(e) => setValue(e.target.value)}
                disabled={sending}
            />

            <button className={styles.button} type="submit" disabled={sending || !value.trim()}>
                Send
            </button>
        </form>
    );
};

export default MessageComposer;