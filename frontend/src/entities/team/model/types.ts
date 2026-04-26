export type TeamRoleLabel = 'Leader' | 'Admin' | 'Member';


export interface TeamMember {
    userId: string;
    username: string;
    role: TeamRoleLabel;
    musicalRole?: string | null;
    userPictureUrl?: string | null;
}

export interface Team {
    id: string;
    name: string;
    teamPictureUrl?: string | null;
    currentUserRole: TeamRoleLabel;
    members: TeamMember[];
}

export interface TeamEvent {
    id: string;
    teamId: string;
    createdById: string;
    createdByDisplayName: string;
    createdByAvatarUrl?: string | null;
    title: string;
    description?: string | null;
    startDateTime: string;
    durationMinutes: number;
    createdAt: string;
}

export interface TeamProject {
    id: string;
    teamId: string;
    name: string;
    pictureUrl: string;
    createdAt: string;
}

export interface CreateTeamProjectRequest {
    name: string;
}

export interface CreateTeamEventRequest {
    title: string;
    description?: string | null;
    startDateTime: string;
    durationMinutes: number;
}

export interface EditTeamEventRequest {
    title?: string | null;
    description?: string | null;
    startDateTime?: string | null;
    durationMinutes?: number | null;
}

export type InviteStatus = 'Pending' | 'Accepted' | 'Rejected' | 'Cancelled';

export interface TeamInvite {
    id: string;
    teamId: string;
    invitedUserId?: string | null;
    invitedUserEmail?: string | null;
    status: InviteStatus;
    createdAt?: string;
    teamName: string;
    teamPictureUrl?: string | null;
}
