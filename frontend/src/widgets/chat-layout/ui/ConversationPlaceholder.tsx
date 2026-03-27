import React from "react";
import styles from "./ConversationPlaceholder.module.css";

const ConversationPlaceholder = () => {
    return (
        <section className={styles.wrapper}>
            <div className={styles.content}>
                Select a conversation to start chatting
            </div>
        </section>
    );
};

export default ConversationPlaceholder;