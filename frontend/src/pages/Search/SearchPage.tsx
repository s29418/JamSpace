import React, { useState } from "react";
import styles from "./SearchPage.module.css";

import { useUserSearch } from "./model/useUserSearch";
import { useSearchFollowActions } from "./model/useSearchFollowActions";

import SearchTopBar from "./ui/SearchTopBar/SearchTopBar";
import FiltersDrawer from "./ui/FiltersDrawer/FiltersDrawer";
import UsersResults from "./ui/UsersResults/UsersResults";
import Pagination from "./ui/Pagination/Pagination";

const SearchPage: React.FC = () => {
    const [filtersOpen, setFiltersOpen] = useState(true);

    const {
        filters,
        data,
        setData,
        loading,
        error,

        setQ,
        setLocation,
        setPage,

        addSkill,
        removeSkill,
        addGenre,
        removeGenre,

        clearFilters,
    } = useUserSearch({ pageSize: 10 });

    const { toggleFollow, busy } = useSearchFollowActions(
        () => data.items,
        (updater) => setData((prev) => ({ ...prev, items: updater(prev.items) }))
    );

    const hasAppliedFilters = Boolean(
        filters.location ||
        (filters.skills && filters.skills.length > 0) ||
        (filters.genres && filters.genres.length > 0)
    );

    return (
        <div className={styles.page}>
            <div className={styles.layout}>
                <aside
                    className={`${styles.sidebar} ${
                        filtersOpen ? styles.sidebarOpen : styles.sidebarClosed
                    }`}
                >
                    <div className={styles.sidebarInner}>
                        <FiltersDrawer
                            location={filters.location}
                            onLocationChange={setLocation}
                            skills={filters.skills}
                            onAddSkill={addSkill}
                            onRemoveSkill={removeSkill}
                            genres={filters.genres}
                            onAddGenre={addGenre}
                            onRemoveGenre={removeGenre}
                            onClear={clearFilters}
                        />
                    </div>
                </aside>

                <section className={styles.content}>
                    <SearchTopBar
                        value={filters.q}
                        onChange={setQ}
                        onToggleFilters={() => setFiltersOpen((prev) => !prev)}
                        isActive={filtersOpen || hasAppliedFilters}
                        isOpen={filtersOpen}
                    />

                    {error && <p className={styles.message}>{error}</p>}

                    <UsersResults
                        items={data.items}
                        loading={loading}
                        busy={busy}
                        onToggleFollow={toggleFollow}
                    />

                    <Pagination page={data.page} totalPages={data.totalPages} onChange={setPage} />
                </section>
            </div>
        </div>
    );
};

export default SearchPage;
