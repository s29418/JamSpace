import { useAutoAnimate } from "@formkit/auto-animate/react";

export type MessageState = { text: string; color: string } | null;

type Props = {
    message: MessageState;
    className?: string;
};

export default function MessageSlot({ message, className }: Props) {
    const [parent] = useAutoAnimate({ duration: 350, easing: "cubic-bezier(0.22,1,0.36,1)" });

    return (
        <div ref={parent} className={className} style={{ overflow: "hidden" }}>
            {message && (
                <p role="status" aria-live="polite" style={{ color: message.color }}>
                    {message.text}
                </p>
            )}
        </div>
    );
}