import React from "react";
import styles from "./SearchTopBar.module.css";

import { MagnifyingGlassIcon, FunnelIcon } from "@heroicons/react/24/outline";
import { FunnelIcon as FunnelActive } from "@heroicons/react/24/solid";

type Props = {
    value: string;
    onChange: (v: string) => void;
    onOpenFilters: () => void;
    isActive: boolean;
};

const SearchTopBar: React.FC<Props> = ({ value, onChange, onOpenFilters, isActive }) => {
    return (
        <div className={styles.wrap}>
            <div className={styles.searchBar}>
                <MagnifyingGlassIcon className={styles.searchIcon} />

                <input
                    className={styles.input}
                    placeholder="Search by username..."
                    value={value}
                    onChange={(e) => onChange(e.target.value)}
                />

                <button
                    className={styles.filterBtn}
                    onClick={onOpenFilters}
                    aria-label="Open filters"
                    aria-pressed={isActive}
                >
                    {isActive ? (
                        <FunnelActive className={styles.filterIcon} />
                    ) : (
                        <FunnelIcon className={styles.filterIcon} />
                    )}
                </button>
            </div>
        </div>
    );
};

export default SearchTopBar;
