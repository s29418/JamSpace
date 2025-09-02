import { useCallback } from 'react';
import {
    Team,
    TeamRoleCode,
    roleCodeToLabel,
    TeamRoleLabel,
} from '../types/team';
import {
    changeTeamName,
    deleteTeam,
    changeTeamPicture,
} from '../services/teams.service';
import {
    leaveTeam,
    changeTeamMemberRole,
    changeTeamMemberMusicalRole,
    kickTeamMember,
} from '../services/teamMembers.service';

type ToastFn = (text: string, type?: 'success' | 'error' | 'info') => void;

function getErrorMessage(e: any, fallback: string) {
    if (!e) return fallback;
    if (typeof e === 'string') return e;
    if (e?.message) return e.message;
    if (e?.response?.data?.message) return e.response.data.message;
    return fallback;
}

export function useTeamActions(params: {
    teamId: string;
    team: Team | null;
    setTeam: React.Dispatch<React.SetStateAction<Team | null>>;
    refreshTeam?: () => Promise<void>;
    refreshInvites?: () => Promise<void>;
    toast?: ToastFn;
}) {
    const { teamId, team, setTeam, refreshTeam, toast } = params;

    const renameTeamAction = useCallback(
        async (newName: string) => {
            const trimmed = newName.trim();
            if (!trimmed) {
                toast?.('Name cannot be empty.', 'error');
                return;
            }
            const prev = team;
            setTeam((t) => (t ? { ...t, name: trimmed } : t));
            try {
                await changeTeamName(teamId, trimmed);
                toast?.('Team name updated.', 'success');
                window.dispatchEvent(new CustomEvent('team:updated', {
                    detail: { teamId, patch: { name: newName } }
                }));
            } catch (e) {
                setTeam(prev); // rollback
                toast?.(getErrorMessage(e, 'Failed to change team name.'), 'error');
            }
        },
        [teamId, team, setTeam, toast],
    );

    const changeMemberRoleAction = useCallback(
        async (userId: string, roleCode: TeamRoleCode) => {
            const label = roleCodeToLabel[roleCode] as TeamRoleLabel;
            const prev = team;
            setTeam((t) =>
                t
                    ? {
                        ...t,
                        members: t.members.map((m) =>
                            m.userId === userId ? { ...m, role: label } : m,
                        ),
                    }
                    : t,
            );
            try {
                await changeTeamMemberRole(teamId, userId, String(roleCode));
                toast?.('Member role updated.', 'success');
            } catch (e) {
                setTeam(prev); // rollback
                toast?.(getErrorMessage(e, 'Failed to change member role.'), 'error');
            }
        },
        [teamId, team, setTeam, toast],
    );

    const changeMemberMusicalRoleAction = useCallback(
        async (userId: string, musicalRole: string) => {
            const trimmed = musicalRole.trim();
            const prev = team;
            setTeam((t) =>
                t
                    ? {
                        ...t,
                        members: t.members.map((m) =>
                            m.userId === userId ? { ...m, musicalRole: trimmed } : m,
                        ),
                    }
                    : t,
            );
            try {
                await changeTeamMemberMusicalRole(teamId, userId, trimmed);
                toast?.('Musical role updated.', 'success');
            } catch (e) {
                setTeam(prev);
                toast?.(
                    getErrorMessage(e, 'Failed to change musical role.'),
                    'error',
                );
            }
        },
        [teamId, team, setTeam, toast],
    );

    const changeTeamPictureAction = useCallback(
        async (file: File) => {
            if (!file) return;
            const prev = team;
            try {
                const url = await changeTeamPicture(teamId, file);
                const bust = url + (url.includes('?') ? '&' : '?') + 'v=' + Date.now();
                setTeam((t) => (t ? { ...t, teamPictureUrl: bust } : t));
                window.dispatchEvent(new CustomEvent('team:updated', {
                    detail: { teamId, patch: { teamPictureUrl: bust } }
                }));
            } catch (e) {
                setTeam(prev);
                toast?.(getErrorMessage(e, 'Failed to change team picture.'), 'error');
            }
        },
        [teamId, team, setTeam, toast],
    );

    const deleteTeamAction = useCallback(async () => {
        try {
            await deleteTeam(teamId);
            toast?.('Team deleted.', 'success');
            return true;
        } catch (e) {
            toast?.(getErrorMessage(e, 'Failed to delete team.'), 'error');
            return false;
        }
    }, [teamId, toast]);

    const leaveTeamAction = useCallback(async () => {
        try {
            await leaveTeam(teamId);
            toast?.('You left the team.', 'success');
            return true;
        } catch (e) {
            toast?.(getErrorMessage(e, 'Failed to leave the team.'), 'error');
            return false;
        }
    }, [teamId, toast]);

    const kickMemberAction = useCallback(
        async (userId: string) => {
            const prev = team;
            setTeam((t) =>
                t ? { ...t, members: t.members.filter((m) => m.userId !== userId) } : t,
            );
            try {
                await kickTeamMember(teamId, userId);
                toast?.('Member removed.', 'success');
            } catch (e) {
                setTeam(prev);
                toast?.(getErrorMessage(e, 'Failed to remove member.'), 'error');
            }
        },
        [teamId, team, setTeam, toast],
    );

    return {
        renameTeam: renameTeamAction,
        deleteTeam: deleteTeamAction,
        leaveTeam: leaveTeamAction,
        changeTeamPicture: changeTeamPictureAction,
        changeMemberRole: changeMemberRoleAction,
        changeMemberMusicalRole: changeMemberMusicalRoleAction,
        kickMember: kickMemberAction,
    };
}

