import React, { useLayoutEffect, useMemo, useRef, useState } from "react";
import styles from "../UserSearchCard/UserSearchCard.module.css";

type TagRowProps = {
    tags: string[];
    variant?: "default" | "primary";
};

const TagRow: React.FC<TagRowProps> = ({ tags, variant = "default" }) => {
    const containerRef = useRef<HTMLDivElement | null>(null);
    const moreRef = useRef<HTMLSpanElement | null>(null);
    const tagRefs = useRef<(HTMLSpanElement | null)[]>([]);

    const [visibleCount, setVisibleCount] = useState(tags.length);

    const tagClassName =
        variant === "primary"
            ? `${styles.tag} ${styles.tagPrimary}`
            : styles.tag;

    useLayoutEffect(() => {
        const calculateVisibleTags = () => {
            const container = containerRef.current;
            if (!container) return;

            const containerWidth = container.clientWidth;
            if (containerWidth <= 0) return;

            let usedWidth = 0;
            let count = 0;
            const gap = 10;

            for (let i = 0; i < tags.length; i++) {
                const el = tagRefs.current[i];
                if (!el) continue;

                const tagWidth = el.offsetWidth;
                const nextTagWidth = count === 0 ? tagWidth : usedWidth + gap + tagWidth;

                const remainingAfterThis = tags.length - (i + 1);

                let moreWidth = 0;
                if (remainingAfterThis > 0 && moreRef.current) {
                    moreRef.current.textContent = `+${remainingAfterThis}`;
                    moreWidth = gap + moreRef.current.offsetWidth;
                }

                if (nextTagWidth + moreWidth <= containerWidth + 1) {
                    usedWidth = nextTagWidth;
                    count++;
                } else {
                    break;
                }
            }

            setVisibleCount(count);
        };

        calculateVisibleTags();

        const observer = new ResizeObserver(() => {
            calculateVisibleTags();
        });

        if (containerRef.current) {
            observer.observe(containerRef.current);
        }

        window.addEventListener("resize", calculateVisibleTags);

        return () => {
            observer.disconnect();
            window.removeEventListener("resize", calculateVisibleTags);
        };
    }, [tags]);

    const visibleTags = useMemo(() => tags.slice(0, visibleCount), [tags, visibleCount]);
    const hiddenCount = tags.length - visibleTags.length;

    return (
        <div className={styles.tagsRow} ref={containerRef}>
            {tags.map((tag, index) => (
                <span
                    key={`${tag}-${index}`}
                    ref={(el) => {
                        tagRefs.current[index] = el;
                    }}
                    className={`${tagClassName} ${index < visibleCount ? "" : styles.tagMeasureHidden}`}
                >
                    {tag}
                </span>
            ))}

            {hiddenCount > 0 && (
                <span className={`${styles.tag} ${styles.tagMore}`}>
                    +{hiddenCount}
                </span>
            )}

            <span
                ref={moreRef}
                className={`${styles.tag} ${styles.tagMore} ${styles.tagMeasureHidden}`}
            >
                +999
            </span>
        </div>
    );
};

export default TagRow;