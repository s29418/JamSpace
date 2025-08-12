import {
    changeTeamMemberRole as apiChangeTeamMemberRole,
    kickTeamMember as apiKickTeamMember,
    changeTeamMemberMusicalRole as apiChangeTeamMemberMusicalRole,
    leaveTeam as apiLeaveTeam
} from '../api/teamMembers.api';

export async function changeTeamMemberRole(teamId: string, userId: string, newRole: string): Promise<string> {
    if (!newRole.trim()) {
        throw new Error('Role cannot be empty.');
    }
    const res = await apiChangeTeamMemberRole(teamId, userId, newRole);
    return res.data.message || `Member role updated.`;
}

export async function kickTeamMember(teamId: string, userId: string): Promise<string> {
    const res = await apiKickTeamMember(teamId, userId);
    return res.data.message || 'Member has been removed from the team.';
}

export async function changeTeamMemberMusicalRole(teamId: string, userId: string, newMusicalRole: string): Promise<string> {
    if (!newMusicalRole.trim()) {
        throw new Error('Musical role cannot be empty.');
    }
    const res = await apiChangeTeamMemberMusicalRole(teamId, userId, newMusicalRole);
    return res.data.message || `Musical role updated to "${newMusicalRole}".`;
}

export async function leaveTeam(teamId: string): Promise<string> {
    const res = await apiLeaveTeam(teamId);
    return res.data.message || 'You have left the team.';
}
