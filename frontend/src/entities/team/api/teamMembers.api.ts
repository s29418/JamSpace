import { api } from '../../../shared/lib/api/base';
import {TeamRoleLabel} from "../model/types";

const ROOT = '/teams';

export const changeTeamMemberRole = async (
    teamId: string,
    userId: string,
    newRole: TeamRoleLabel
) => {
    const res = await api.patch(
        `${ROOT}/${teamId}/members/${userId}/role?newRole=${encodeURIComponent(newRole)}`,
        {}
    );
    return res.data;
};

export const kickTeamMember = async (teamId: string, userId: string) => {
    const res = await api.delete<{ message?: string }>(`${ROOT}/${teamId}/members/${userId}`);
    return res.data;
};

export const changeTeamMemberMusicalRole = async (teamId: string, userId: string, newMusicalRole: string) => {
    const res = await api.patch<{ message?: string }>(
        `${ROOT}/${teamId}/members/${userId}/musical-role?musicalRole=${encodeURIComponent(newMusicalRole)}`,
        {}
    );
    return res.data;
};

export const leaveTeam = async (teamId: string) => {
    const res = await api.delete<{ message?: string }>(`${ROOT}/${teamId}/members/leave`);
    return res.data;
};
