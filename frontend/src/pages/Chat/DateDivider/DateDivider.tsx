import styles from "./DateDivider.module.css";

type Props = {
    date: string;
};

function formatDate(value: string) {
    const date = new Date(value);

    const today = new Date();
    const yesterday = new Date();
    yesterday.setDate(today.getDate() - 1);

    const sameDay = (a: Date, b: Date) =>
        a.getFullYear() === b.getFullYear() &&
        a.getMonth() === b.getMonth() &&
        a.getDate() === b.getDate();

    if (sameDay(date, today)) return "Today";
    if (sameDay(date, yesterday)) return "Yesterday";

    return date.toLocaleDateString([], {
        year: "numeric",
        month: "long",
        day: "numeric",
    });
}

const DateDivider = ({ date }: Props) => {
    return (
        <div className={styles.wrapper}>
            <span className={styles.line} />
            <span className={styles.label}>{formatDate(date)}</span>
            <span className={styles.line} />
        </div>
    );
};

export default DateDivider;