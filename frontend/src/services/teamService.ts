import {
    getMyTeamsApi,
    uploadTeamPictureApi,
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

export async function uploadTeamPicture(file: File): Promise<string> {
    const res = await uploadTeamPictureApi(file);
    return res.data.url;
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
    await deleteTeamApi(teamId);
    return 'Team deleted successfully.';
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
    const res = await changeTeamPictureApi(teamId, file);
    return res.data.url;
}
