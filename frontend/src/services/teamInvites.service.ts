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
    await acceptTeamInvite(inviteId);
    return 'Invite accepted successfully.';
}

export async function rejectInvite(inviteId: string): Promise<string> {
    await rejectTeamInvite(inviteId);
    return 'Invite rejected successfully.';
}

export async function fetchTeamInvitesByTeamId(teamId: string) {
    const res = await getTeamInvitesByTeamId(teamId);
    return res.data;
}

export async function cancelInvite(inviteId: string): Promise<string> {
    await cancelTeamInvite(inviteId);
    return 'Invite cancelled successfully.';
}

export async function inviteUserToTeam(username: string, teamId: string): Promise<string> {
    if (!username.trim()) {
        throw new Error('Username cannot be empty.');
    }

    await postInviteUser(username, teamId);

    return `User "${username}" has been invited.`;
}
