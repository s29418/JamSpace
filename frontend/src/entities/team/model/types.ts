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
    description?: string | null;
    pictureUrl?: string | null;
    createdAt: string;
    updatedAt: string;
}

export interface CreateTeamProjectRequest {
    name: string;
    description?: string | null;
}

export interface EditTeamProjectRequest {
    name: string;
    description?: string | null;
}

export interface ProjectAudioVersion {
    id: string;
    projectId: string;
    createdById: string;
    createdByDisplayName: string;
    createdByAvatarUrl?: string | null;
    name: string;
    fileUrl: string;
    originalFileName: string;
    contentType: string;
    length: number;
    durationSeconds?: number | null;
    createdAt: string;
}

export interface UploadProjectAudioVersionRequest {
    name: string;
    durationSeconds?: number | null;
    file: File;
}

export type ProjectNoteStatus = 'Active' | 'Completed';

export interface ProjectNote {
    id: string;
    projectId: string;
    audioVersionId?: string | null;
    audioVersionName?: string | null;
    isAudioVersionDeleted: boolean;
    createdById: string;
    createdByDisplayName: string;
    createdByAvatarUrl?: string | null;
    createdByMusicalRole?: string | null;
    completedById?: string | null;
    completedByDisplayName?: string | null;
    completedByAvatarUrl?: string | null;
    completedByMusicalRole?: string | null;
    content: string;
    startTimeSeconds?: number | null;
    endTimeSeconds?: number | null;
    status: ProjectNoteStatus;
    createdAt: string;
    updatedAt: string;
    completedAt?: string | null;
}

export interface CreateProjectNoteRequest {
    content: string;
    audioVersionId?: string | null;
    startTimeSeconds?: number | null;
    endTimeSeconds?: number | null;
}

export interface EditProjectNoteRequest {
    content: string;
    audioVersionId?: string | null;
    startTimeSeconds?: number | null;
    endTimeSeconds?: number | null;
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
