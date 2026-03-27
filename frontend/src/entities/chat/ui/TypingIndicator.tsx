import React from "react";
import styles from "./TypingIndicator.module.css";

type Props = {
    names: string[];
};

const TypingIndicator = ({ names }: Props) => {
    if (names.length === 0) return null;

    const text =
        names.length === 1
            ? `${names[0]} is typing...`
            : `${names.slice(0, 2).join(", ")} are typing...`;

    return (
        <div className={styles.wrapper}>
            <div className={styles.dots}>
                <span />
                <span />
                <span />
            </div>
            <span className={styles.text}>{text}</span>
        </div>
    );
};

export default TypingIndicator;