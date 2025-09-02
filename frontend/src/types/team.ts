export enum TeamRoleCode {
    Leader = 0,
    Admin = 1,
    Member = 2,
}

export type TeamRoleLabel = 'Leader' | 'Admin' | 'Member';

export const roleCodeToLabel: Record<TeamRoleCode, TeamRoleLabel> = {
    [TeamRoleCode.Leader]: 'Leader',
    [TeamRoleCode.Admin]: 'Admin',
    [TeamRoleCode.Member]: 'Member',
};

export const roleLabelToCode: Record<TeamRoleLabel, TeamRoleCode> = {
    Leader: TeamRoleCode.Leader,
    Admin: TeamRoleCode.Admin,
    Member: TeamRoleCode.Member,
};

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

export type InviteStatus = 'Pending' | 'Accepted' | 'Rejected' | 'Cancelled';

export interface TeamInvite {
    id: string;
    teamId: string;
    invitedUserId?: string | null;
    invitedUserEmail?: string | null;
    status: InviteStatus;
    createdAt?: string;
}

export type ApiError = { message: string };

export type Nullable<T> = T | null;

export function isTeamRoleCode(v: unknown): v is TeamRoleCode {
    return typeof v === 'number' && v in roleCodeToLabel;
}

export function isTeamRoleLabel(v: unknown): v is TeamRoleLabel {
    return v === 'Leader' || v === 'Admin' || v === 'Member';
}