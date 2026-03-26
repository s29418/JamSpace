import React from "react";
import styles from "./Pagination.module.css";

type Props = {
    page: number;
    totalPages: number;
    onChange: (p: number) => void;
};

const Pagination: React.FC<Props> = ({ page, totalPages, onChange }) => {
    if (totalPages <= 1) return null;

    return (
        <div className={styles.wrap}>
            <button className={styles.btn} disabled={page <= 1} onClick={() => onChange(page - 1)}>
                Previous
            </button>

            <div className={styles.info}>
                {page} / {totalPages}
            </div>

            <button className={styles.btn} disabled={page >= totalPages} onClick={() => onChange(page + 1)}>
                Next
            </button>
        </div>
    );
};

export default Pagination;
