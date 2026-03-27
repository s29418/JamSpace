import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { getOrCreateDirectConversation } from "entities/chat/api/conversations.api";
import styles from "./SearchPage.module.css";

import { useUserSearch } from "../../../features/user/search-users/model/useUserSearch";
import { useSearchFollowActions } from "../../../features/user/search-users/model/useSearchFollowActions";

import SearchTopBar from "../../../widgets/search-filters/ui/SearchTopBar";
import FiltersDrawer from "../../../widgets/search-filters/ui/FiltersDrawer";
import UsersResults from "../../../widgets/search-results/ui/UsersResults";
import Pagination from "../../../widgets/search-results/ui/Pagination";

const SearchPage: React.FC = () => {
    const [filtersOpen, setFiltersOpen] = useState(true);
    const navigate = useNavigate();

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

    const handleMessage = async (userId: string) => {
        try {
            const result = await getOrCreateDirectConversation({
                otherUserId: userId,
            });

            navigate(`/chat?conversationId=${result.conversationId}`);
        } catch (error) {
            console.error("Failed to open direct conversation", error);
        }
    };

    return (
        <div className={styles.page}>
            <div className={styles.shell}>
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
                        <div className={styles.contentInner}>
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
                                onMessage={handleMessage}
                            />

                            <Pagination
                                page={data.page}
                                totalPages={data.totalPages}
                                onChange={setPage}
                            />
                        </div>
                    </section>
                </div>
            </div>
        </div>
    );
};

export default SearchPage;