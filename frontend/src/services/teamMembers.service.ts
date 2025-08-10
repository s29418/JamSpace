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
    await apiChangeTeamMemberRole(teamId, userId, newRole);
    return `Member role updated.`;
}

export async function kickTeamMember(teamId: string, userId: string): Promise<string> {
    await apiKickTeamMember(teamId, userId);
    return 'Member has been removed from the team.';
}

export async function changeTeamMemberMusicalRole(teamId: string, userId: string, newMusicalRole: string): Promise<string> {
    if (!newMusicalRole.trim()) {
        throw new Error('Musical role cannot be empty.');
    }
    await apiChangeTeamMemberMusicalRole(teamId, userId, newMusicalRole);
    return `Musical role updated to "${newMusicalRole}".`;
}

export async function leaveTeam(teamId: string): Promise<string> {
    await apiLeaveTeam(teamId);
    return 'You have left the team.';
}
