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
    const response = await axios.get(`${API_URL}/invites`, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    return response.data;
};

export const acceptTeamInvite = async (inviteId: string) => {
    const token = localStorage.getItem('token');
    const response = await axios.post(`${API_URL}/invites/${inviteId}/accept`, {}, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    return response.data;
};

export const rejectTeamInvite = async (inviteId: string) => {
    const token = localStorage.getItem('token');
    const response = await axios.post(`${API_URL}/invites/${inviteId}/reject`, {}, {
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
    const response = await axios.post(`${API_URL}/invites/${username}`, `"${teamId}"`, {
        headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
        },
    });
    return response.data;
};

export const deleteTeam = async (teamId: string) => {
    const token = localStorage.getItem('token');
    const response = await axios.delete(`${API_URL}/${teamId}`, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    return response.data;
};

export const changeTeamName = async (teamId: string, newName: string) => {
    const token = localStorage.getItem('token');
    const response = await axios.patch(`${API_URL}/${teamId}/teamName?teamName=${newName}`,{ },  {
        headers: {
            Authorization: `Bearer ${token}`
        },
    });
    return response.data;
}

export const changeTeamPicture = async (teamId: string, pictureUrl: string) => {
    const token = localStorage.getItem('token');
    const response = await axios.patch(`${API_URL}/${teamId}/team-picture`, { teamPictureUrl: pictureUrl }, {
        headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
        },
    });
    return response.data;
};

export const changeTeamMemberRole = async (teamId: string, userId: string, newRole: string) => {
    const token = localStorage.getItem('token');
    const response = await axios.patch(`${API_URL}/${teamId}/members/${userId}/role`, { role: newRole }, {
        headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
        },
    });
    return response.data;
}

export const kickTeamMember = async (teamId: string, userId: string) => {
    const token = localStorage.getItem('token');
    const response = await axios.delete(`${API_URL}/${teamId}/members/${userId}`, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    return response.data;
};

export const changeTeamMemberMusicalRole = async (teamId: string, userId: string, newMusicalRole: string) => {
    const token = localStorage.getItem('token');
    const response = await axios.patch(`${API_URL}/${teamId}/members/${userId}/musical-role`, { musicalRole: newMusicalRole }, {
        headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
        },
    });
    return response.data;
}

export const getTeamInvitesByTeamId = async (teamId: string) => {
    const token = localStorage.getItem('token');
    const response = await axios.get(`${API_URL}/${teamId}/invites`, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    return response.data;
}

export const cancelTeamInvite = async (inviteId: string) => {
    const token = localStorage.getItem('token');
    const response = await axios.patch(`${API_URL}/invites/${inviteId}/cancel`, null, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    return response.data;
}