import React from "react";
import styles from "./Chips.module.css";

type Props = {
    items: string[];
    onRemove: (v: string) => void;
};

const Chips: React.FC<Props> = ({ items, onRemove }) => {
    if (!items.length) return null;

    return (
        <div className={styles.wrap}>
            {items.map((x) => (
                <span key={x} className={styles.chip}>
          {x}
                    <button className={styles.x} onClick={() => onRemove(x)} aria-label={`Remove ${x}`}>
            ✕
          </button>
        </span>
            ))}
        </div>
    );
};

export default Chips;
