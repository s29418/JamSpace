import { postInviteUser} from '../api/teamInvites.api';

export async function inviteUserToTeam(username: string, teamId: string): Promise<string> {
    if (!username.trim()) {
        throw new Error('Username cannot be empty.');
    }

    await postInviteUser(username, teamId);

    return `User "${username}" has been invited.`;
}
