import {
    getMyTeamsApi,
    createTeamApi,
    getTeamByIdApi,
    deleteTeamApi,
    changeTeamNameApi,
    changeTeamPictureApi
} from '../api/teams.api';

export async function getMyTeams() {
    const res = await getMyTeamsApi();
    return res.data;
}

export async function createTeam(teamData: { name: string; teamPictureUrl?: string | null }) {
    const res = await createTeamApi(teamData);
    return res.data;
}

export async function getTeamById(id: string) {
    const res = await getTeamByIdApi(id);
    return res.data;
}

export async function deleteTeam(teamId: string): Promise<string> {
    const res = await deleteTeamApi(teamId);
    return res.data.message || 'Team deleted successfully.';
}

export async function changeTeamName(teamId: string, newName: string) {
    if (!newName.trim()) {
        throw new Error('Team name cannot be empty.');
    }

    await changeTeamNameApi(teamId, newName);

    const res = await getTeamByIdApi(teamId);
    return res.data;
}

export async function changeTeamPicture(teamId: string, file: File): Promise<string> {
    await changeTeamPictureApi(teamId, file);
    return "Team picture updated successfully.";
}
