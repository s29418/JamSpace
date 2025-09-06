import { changeTeamName, changeTeamPicture, deleteTeam } from 'entities/team/api/teams.api';
import {
    changeTeamMemberRole,
    changeTeamMemberMusicalRole,
    kickTeamMember,
    leaveTeam,
} from 'entities/team/api/teamMembers.api';
import type {Team, TeamRoleLabel} from 'entities/team/model/types';

type Options = {
    onUpdated?: () => void | Promise<void>;
    setTeam?: React.Dispatch<React.SetStateAction<Team | null>>;
};

export function useTeamActions(teamId: string, opts: Options = {}) {
    const { onUpdated, setTeam } = opts;

    const afterChange = async () => {
        if (onUpdated) await onUpdated();
    };

    async function renameTeam(newName: string) {
        if (setTeam) {
            setTeam((prev) => (prev ? { ...prev, name: newName } as Team : prev));
        }
        await changeTeamName(teamId, newName);
        await afterChange();
    }

    async function updateTeamPicture(file: File) {
        await changeTeamPicture(teamId, file);
        await afterChange();
    }

    async function setMemberRole(userId: string, role: TeamRoleLabel) {
        await changeTeamMemberRole(teamId, userId, role);
        await afterChange();
    }

    async function setMemberMusicalRole(userId: string, musicalRole: string) {
        await changeTeamMemberMusicalRole(teamId, userId, musicalRole);
        await afterChange();
    }

    async function removeMember(userId: string) {
        await kickTeamMember(teamId, userId);
        await afterChange();
    }

    async function leaveCurrentTeam() {
        await leaveTeam(teamId);
        await afterChange();
    }

    async function removeTeam() {
        await deleteTeam(teamId);
        await afterChange();
    }

    return {
        renameTeam,
        updateTeamPicture,
        setMemberRole,
        setMemberMusicalRole,
        removeMember,
        leaveCurrentTeam,
        removeTeam,
    };
}
