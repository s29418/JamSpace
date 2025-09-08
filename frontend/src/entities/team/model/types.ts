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