import React from 'react';
import styles from './ExpandableMessage.module.css';

type MessageState = { text: string; color: string } | null;

interface Props {
    message: MessageState;
}

const ExpandableMessage: React.FC<Props> = ({ message }) => {
    return (
        <div
            className={
                styles.expandable + (message ? ' ' + styles.expanded : '')
            }
        >
            {message && (
                <p style={{ color: message.color }}>
                    {message.text}
                </p>
            )}
        </div>
    );
};

export default ExpandableMessage;