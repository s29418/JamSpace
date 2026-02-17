import { api } from "shared/lib/api/base";
import type { PagedResult, UserSearchItem } from "../model/types";

const ROOT = "/users";

export type SearchUsersParams = {
    q?: string | null;
    location?: string | null;
    skills?: string[];
    genres?: string[];
    page?: number;
    pageSize?: number;
};

const addMulti = (sp: URLSearchParams, key: string, values?: string[]) => {
    (values ?? [])
        .map((x) => x?.trim())
        .filter(Boolean)
        .forEach((v) => sp.append(key, v!));
};

export const searchUsers = async (params: SearchUsersParams): Promise<PagedResult<UserSearchItem>> => {
    const sp = new URLSearchParams();

    if (params.q?.trim()) sp.set("q", params.q.trim());
    if (params.location?.trim()) sp.set("location", params.location.trim());

    addMulti(sp, "skills", params.skills);
    addMulti(sp, "genres", params.genres);

    sp.set("page", String(params.page ?? 1));
    sp.set("pageSize", String(params.pageSize ?? 10));

    const res = await api.get<any>(`${ROOT}/search?${sp.toString()}`);
    const raw = res.data ?? {};

    return {
        items: (raw.items ?? []).map((u: any) => ({
            id: u.id,
            username: u.username ?? "",
            profilePictureUrl: u.profilePictureUrl ?? null,
            location: {
                city: u.city ?? null,
                country: u.countryCode ?? null,
            },
            skills: (u.skills ?? []) as string[],
            genres: (u.genres ?? []) as string[],
            followersCount: u.followersCount ?? 0,
            isFollowing: !!u.isFollowedByMe,
            isMe: !!u.isMe,
        })),
        page: raw.page ?? 1,
        pageSize: raw.pageSize ?? 10,
        totalItems: raw.totalItems ?? 0,
        totalPages: raw.totalPages ?? 1,
    };
};
