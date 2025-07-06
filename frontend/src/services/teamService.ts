import axios from 'axios';

const API_URL = 'http://localhost:5072/api/teams';

export const getMyTeams = async () => {
    const token = localStorage.getItem('token');
    const response = await axios.get(`${API_URL}/my`, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    return response.data;
};

export const uploadTeamPicture = async (file: File): Promise<string> => {
    const token = localStorage.getItem("token");
    const formData = new FormData();
    formData.append("file", file);

    const response = await axios.post(
        'http://localhost:5072/api/uploads/team-picture',
        formData,
        {
            headers: {
                Authorization: `Bearer ${token}`,
                'Content-Type': 'multipart/form-data',
            },
        }
    );

    return response.data.url;
};

export const createTeam = async (teamData: { name: string; teamPictureUrl?: string | null}) => {
    const token = localStorage.getItem('token');
    const response = await axios.post(API_URL, teamData, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    return response.data;
}

export const getTeamInvites = async () => {
    const token = localStorage.getItem('token');
    const response = await axios.get(`${API_URL}/invite`, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    return response.data;
};

export const acceptTeamInvite = async (inviteId: string) => {
    const token = localStorage.getItem('token');
    const response = await axios.post(`${API_URL}/invite/${inviteId}/accept`, {}, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    return response.data;
};

export const rejectTeamInvite = async (inviteId: string) => {
    const token = localStorage.getItem('token');
    const response = await axios.post(`${API_URL}/invite/${inviteId}/reject`, {}, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    return response.data;
};

export const getTeamById = async (id: string) => {
    const token = localStorage.getItem('token');
    const response = await axios.get(`${API_URL}/${id}`, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    return response.data;
};

export const inviteUserToTeam = async (username: string, teamId: string) => {
    const token = localStorage.getItem('token');
    const response = await axios.post(`${API_URL}/invite/${username}`, `"${teamId}"`, {
        headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
        },
    });
    return response.data;
};
