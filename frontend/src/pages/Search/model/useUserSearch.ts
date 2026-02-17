import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { ApiError, isApiError } from "shared/lib/api/base";
import { searchUsers } from "entities/user/api/usersSearch.api";
import type { PagedResult, UserSearchItem } from "entities/user/model/types";

type Filters = {
    q: string;
    location: string;
    skills: string[];
    genres: string[];
    page: number;
    pageSize: number;
};

const empty: PagedResult<UserSearchItem> = {
    items: [],
    page: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 1,
};

export function useUserSearch(initial?: Partial<Filters>) {
    const [filters, setFilters] = useState<Filters>({
        q: initial?.q ?? "",
        location: initial?.location ?? "",
        skills: initial?.skills ?? [],
        genres: initial?.genres ?? [],
        page: initial?.page ?? 1,
        pageSize: initial?.pageSize ?? 10,
    });

    const [data, setData] = useState<PagedResult<UserSearchItem>>({
        ...empty,
        pageSize: filters.pageSize,
    });

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const timer = useRef<number | null>(null);

    const normalized = useMemo(() => {
        return {
            ...filters,
            q: filters.q.trim(),
            location: filters.location.trim(),
        };
    }, [filters]);

    const refresh = useCallback(async (f: Filters) => {
        setLoading(true);
        setError(null);
        try {
            const res = await searchUsers({
                q: f.q,
                location: f.location,
                skills: f.skills,
                genres: f.genres,
                page: f.page,
                pageSize: f.pageSize,
            });
            setData(res);
        } catch (e) {
            const msg = isApiError(e) ? (e as ApiError).message : "Failed to search users";
            setError(msg);
            setData((prev) => ({ ...empty, pageSize: prev.pageSize }));
        } finally {
            setLoading(false);
        }
    }, []);


    useEffect(() => {
        let alive = true;

        if (timer.current) window.clearTimeout(timer.current);
        timer.current = window.setTimeout(() => {
            if (!alive) return;
            void refresh(normalized);
        }, 350);

        return () => {
            alive = false;
            if (timer.current) window.clearTimeout(timer.current);
        };
    }, [normalized, refresh]);

    const setQ = (q: string) => setFilters((p) => ({ ...p, q, page: 1 }));
    const setLocation = (location: string) => setFilters((p) => ({ ...p, location, page: 1 }));
    const setPage = (page: number) => setFilters((p) => ({ ...p, page }));

    const addSkill = (name: string) => {
        const v = name.trim();
        if (!v) return;
        setFilters((p) => {
            if (p.skills.some((x) => x.toLowerCase() === v.toLowerCase())) return p;
            return { ...p, skills: [...p.skills, v], page: 1 };
        });
    };

    const removeSkill = (name: string) =>
        setFilters((p) => ({ ...p, skills: p.skills.filter((x) => x !== name), page: 1 }));

    const addGenre = (name: string) => {
        const v = name.trim();
        if (!v) return;
        setFilters((p) => {
            if (p.genres.some((x) => x.toLowerCase() === v.toLowerCase())) return p;
            return { ...p, genres: [...p.genres, v], page: 1 };
        });
    };

    const removeGenre = (name: string) =>
        setFilters((p) => ({ ...p, genres: p.genres.filter((x) => x !== name), page: 1 }));

    const clearFilters = () =>
        setFilters((p) => ({ ...p, location: "", skills: [], genres: [], page: 1 }));

    return {
        filters,
        data,
        setData,
        loading,
        error,
        refresh,

        setQ,
        setLocation,
        setPage,

        addSkill,
        removeSkill,
        addGenre,
        removeGenre,
        clearFilters,
    };
}
