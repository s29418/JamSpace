import { useCallback, useEffect, useState } from 'react';
import { getTeamById } from '../services/teams.service';
import {Team, TeamMember, isTeamRoleCode, isTeamRoleLabel, roleCodeToLabel, TeamRoleLabel} from "../types/team";

function normalizeMembers(raw: any[]): TeamMember[] {
    return (raw ?? []).map((m: any) => {
        const userId =
            m.userId ?? m.id ?? m.user?.id ?? m.user?.userId ?? '';

        const username =
            m.username ??
            m.userName ??
            m.user?.username ??
            m.user?.userName ??
            m.name ??
            m.user?.name ??
            '—';

        const avatarUrl =
            m.avatarUrl ??
            m.userPictureUrl ??
            m.pictureUrl ??
            m.profilePictureUrl ??
            m.user?.avatarUrl ??
            m.user?.pictureUrl ??
            null;

        const rawRole = m.role ?? m.user?.role;


        const maybeNum = typeof rawRole === 'string' && /^\d+$/.test(rawRole)
            ? Number(rawRole)
            : rawRole;

        const role: TeamRoleLabel =
            isTeamRoleCode(maybeNum)
                ? roleCodeToLabel[maybeNum]
                : isTeamRoleLabel(rawRole)
                    ? rawRole
                    : 'Member';

        const musicalRole =
            m.musicalRole ?? m.userMusicalRole ?? m.user?.musicalRole ?? null;

        // ZWRACAMY 'username' (wymagane przez TeamMember) + alias 'userName' dla zgodności
        return {
            userId,
            username,            // ← wymagane przez Twój TeamMember
            userName: username,  // ← alias dla komponentów używających userName
            avatarUrl,
            role,
            musicalRole,
            userPictureUrl: avatarUrl,
        } as unknown as TeamMember; // (opcjonalny as, ale zwykle niepotrzebny)
    });
}

export function useTeam(teamId?: string) {
    const [team, setTeam] = useState<Team | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const refreshTeam = useCallback(async () => {
        if (!teamId) return;
        setLoading(true);
        setError(null);
        try {
            const data = await getTeamById(teamId);
            const normalized: Team = {
                ...data,
                members: normalizeMembers(data.members ?? []),
            };
            setTeam(normalized);
        } catch (e) {
            setError('Failed to fetch team details');
        } finally {
            setLoading(false);
        }
    }, [teamId]);

    // pierwszy fetch
    useEffect(() => {
        setTeam(null);
        setLoading(true);
        setError(null);
        refreshTeam();
    }, [refreshTeam]);

    // subskrypcja patchy z modala
    useEffect(() => {
        function onTeamUpdated(e: any) {
            if (!e?.detail || e.detail.teamId !== teamId) return;
            setTeam(prev => (prev ? { ...prev, ...e.detail.patch } : prev));
        }
        window.addEventListener('team:updated', onTeamUpdated);
        return () => window.removeEventListener('team:updated', onTeamUpdated);
    }, [teamId]);

    return { team, setTeam, loading, error, refreshTeam };
}
