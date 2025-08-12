import {
    getTeamInvites,
    acceptTeamInvite,
    rejectTeamInvite,
    getTeamInvitesByTeamId,
    cancelTeamInvite,
    postInviteUser
} from '../api/teamInvites.api';

export async function fetchUserInvites() {
    const res = await getTeamInvites();
    return res.data;
}

export async function acceptInvite(inviteId: string): Promise<string> {
    const res = await acceptTeamInvite(inviteId);
    return res.data.message || 'Invite accepted successfully.';
}

export async function rejectInvite(inviteId: string): Promise<string> {
    const res = await rejectTeamInvite(inviteId);
    return res.data.message || 'Invite rejected successfully.';
}

export async function fetchTeamInvitesByTeamId(teamId: string) {
    const res = await getTeamInvitesByTeamId(teamId);
    return res.data;
}

export async function cancelInvite(inviteId: string): Promise<string> {
    const res = await cancelTeamInvite(inviteId);
    return res.data.message || 'Invite cancelled successfully.';
}

export async function inviteUserToTeam(username: string, teamId: string): Promise<string> {
    if (!username.trim()) {
        throw new Error('Username cannot be empty.');
    }

    const res = await postInviteUser(username, teamId);

    return res.data.message || `User "${username}" has been invited.`;
}
