import React, { useEffect, useMemo, useRef } from 'react';
import ReactDOM from 'react-dom';
import styles from './TeamSettingsModal.module.css';

// HOOKS & TYPES
import { useTeam } from '../model/useTeam';
import { useTeamInvites } from '../../inviteCard/model/useTeamInvites';
import { useTeamActions } from '../model/useTeamActions';
import { useToast } from 'shared/lib/hooks/useToast';
import { TeamRoleLabel, TeamInvite, TeamMember } from 'entities/team/model/types';
import { emitTeamUpdated, emitTeamRemoved } from 'shared/lib/events/teamEvents';

// UI BLOCKS
import { TeamHeader } from './Header/TeamHeader';
import { MembersList } from './MembersList/MembersList';
import { InvitesList, InviteView } from './Invites/InvitesList';
import { InviteForm } from './Invites/InviteForm';
import MessageSlot from 'shared/ui/message/MessageSlot';
import { AvatarUploader } from './Header/AvatarUploader';

type Props = {
    isOpen: boolean;
    teamId: string;
    currentUserId: string;
    onClose: () => void;
    onTeamDeleted?: () => void;
    onLeftTeam?: () => void;
};

export default function TeamSettingsModal({
                                              isOpen,
                                              teamId,
                                              currentUserId,
                                              onClose,
                                              onTeamDeleted,
                                              onLeftTeam,
                                          }: Props) {
    // TEAM DATA
    const {
        team,
        setTeam,
        loading: teamLoading,
        error: teamError,
        refresh: refreshTeam,
    } = useTeam(teamId);

    // INVITES DATA
    const {
        invites,
        loading: invitesLoading,
        error: invitesError,
        inviteUserToTeam,
        cancelInvite,
    } = useTeamInvites({ teamId });

    // MESSAGES/ERRORS
    const {
        message: headerMsg, show: showHeaderMsg, showError: showHeaderErr,
    } = useToast();
    const {
        message: membersMsg, show: showMembersMsg, showError: showMembersErr,
    } = useToast();
    const {
        message: invitesMsg, show: showInvitesMsg, showError: showInvitesErr,
    } = useToast();

    // ACTIONS
    const actions = useTeamActions(teamId, { onUpdated: refreshTeam, setTeam });

    // HEADER ACTIONS
    const renameTeam = async (name: string) => {
        try {
            await actions.renameTeam(name);
            emitTeamUpdated(teamId, { name });
            showHeaderMsg('Team name updated.', 'success');
        } catch (e: any) {
            showHeaderErr(e?.message ?? 'Failed to update team name.');
        }
    };


    const changeTeamPicture = async (file: File) => {
        try {
            await actions.updateTeamPicture(file);
            emitTeamUpdated(teamId);
            showHeaderMsg('Team picture updated.', 'success');
        } catch (e: any) {
            showHeaderErr(e?.message ?? 'Failed to update team picture.');
        }
    };


    const deleteTeam = async () => {
        try {
            await actions.removeTeam();
            emitTeamRemoved(teamId, 'deleted');
            showHeaderMsg('Team deleted.', 'success');
            onTeamDeleted ? onTeamDeleted() : onClose();
        } catch (e: any) {
            showHeaderErr(e?.message ?? 'Failed to delete team.');
        }
    };

    const leaveTeam = async () => {
        try {
            await actions.leaveCurrentTeam();
            emitTeamRemoved(teamId, 'left');
            showHeaderMsg('You left the team.', 'success');
            onLeftTeam ? onLeftTeam() : onClose();
        } catch (e: any) {
            showHeaderErr(e?.message ?? 'Failed to leave team.');
        }
    };


    // Member actions
    const changeMemberRole = async (userId: string, role: TeamRoleLabel) => {
        try {
            await actions.setMemberRole(userId, role);
            showMembersMsg('Role updated.', 'success');
        } catch (e: any) {
            showMembersErr(e?.message ?? 'Failed to change role.');
        }
    };

    const changeMemberMusicalRole = async (userId: string, val: string) => {
        try {
            await actions.setMemberMusicalRole(userId, val);
            showMembersMsg('Musical role updated.', 'success');
        } catch (e: any) {
            showMembersErr(e?.message ?? 'Failed to change musical role.');
        }
    };

    const kickMember = async (userId: string) => {
        try {
            await actions.removeMember(userId);
            showMembersMsg('Member removed.', 'success');
        } catch (e: any) {
            showMembersErr(e?.message ?? 'Failed to remove member.');
        }
    };

    // MAPPING INVITES TO VIEW
    const inviteViews: InviteView[] = useMemo(() => {
        const toView = (inv: TeamInvite & any): InviteView => ({
            id: inv.id,
            invitedUserName:
                inv.invitedUserName ??
                inv.invitedUserEmail ??
                inv.userName ??
                'User',
            invitedUserPictureUrl:
                inv.invitedUserPictureUrl ?? inv.userPictureUrl ?? null,
            invitedUserId:
                inv.invitedUserId ?? inv.userId ?? null,
            invitedByUserName:
                inv.invitedByUserName ?? inv.createdByUserName ?? 'You',
        });
        return invites.map(toView);
    }, [invites]);

    // INVITE ACTIONS
    async function handleInvite(username: string) {
        try {
            await inviteUserToTeam(username, teamId);
            showInvitesMsg('Invite sent.', 'success');
        } catch (e: any) {
            showInvitesErr(e?.message ?? 'Failed to send invite.');
        }
    }

    async function handleCancelInvite(inviteId: string) {
        try {
            await cancelInvite(inviteId);
            showInvitesMsg('Invite cancelled.', 'success');
        } catch (e: any) {
            showInvitesErr(e?.message ?? 'Failed to cancel invite.');
        }
    }

    // PERMISSIONS
    const canRename = team?.currentUserRole === 'Leader' || team?.currentUserRole === 'Admin';
    const canChangePicture = canRename;
    const canDeleteTeam = team?.currentUserRole === 'Leader';
    const canLeaveTeam = true;

    const canChangeRole = (): boolean =>
        team?.currentUserRole === 'Leader';

    const canEditMusicalRole = (_m: TeamMember): boolean =>
        team?.currentUserRole === 'Leader' || team?.currentUserRole === 'Admin';

    const canKick = (m: TeamMember): boolean =>
        team?.currentUserRole === 'Leader' && m.role !== 'Leader';

    // CLOSE ON ESCAPE
    useEffect(() => {
        if (!isOpen) return;
        const onKey = (e: KeyboardEvent) => {
            if (e.key === 'Escape') onClose();
        };
        window.addEventListener('keydown', onKey);
        return () => window.removeEventListener('keydown', onKey);
    }, [isOpen, onClose]);

    if (!isOpen) return null;

    const modalRoot = document.getElementById('modal-root');
    const body = (
        <ModalBody
            teamId={teamId}
            onClose={onClose}
            teamLoading={teamLoading}
            teamError={teamError}
            invitesLoading={invitesLoading}
            invitesError={invitesError}

            // Data
            teamName={team?.name ?? ''}
            teamPictureUrl={team?.teamPictureUrl ?? null}
            members={team?.members ?? []}

            // Header
            canRename={canRename}
            canChangePicture={canChangePicture}
            canDeleteTeam={canDeleteTeam}
            canLeaveTeam={canLeaveTeam}
            onRename={renameTeam}
            onChangePicture={changeTeamPicture}
            onDeleteTeam={deleteTeam}
            onLeaveTeam={leaveTeam}
            headerMsg={headerMsg}

            // members
            currentUserId={currentUserId}
            onChangeRole={changeMemberRole}
            onChangeMusicalRole={changeMemberMusicalRole}
            onKick={kickMember}
            canChangeRoleFn={canChangeRole}
            canEditMusicalRoleFn={canEditMusicalRole}
            canKickFn={canKick}
            membersMsg={membersMsg}

            // invites
            invites={inviteViews}
            onInvite={handleInvite}
            onCancelInvite={handleCancelInvite}
            invitesMsg={invitesMsg}
        />
    );

    if (!modalRoot) return body;
    return ReactDOM.createPortal(body, modalRoot);
}

// ==========
// MODAL BODY
// ==========

type BodyProps = {
    teamId: string;
    onClose: () => void;

    teamLoading: boolean;
    teamError: string | null;
    invitesLoading: boolean;
    invitesError: string | null;

    teamName: string;
    teamPictureUrl: string | null;
    members: TeamMember[];

    // Header
    canRename: boolean;
    canChangePicture: boolean;
    canDeleteTeam: boolean;
    canLeaveTeam: boolean;
    onRename: (n: string) => void | Promise<void>;
    onChangePicture: (f: File) => void | Promise<void>;
    onDeleteTeam: () => Promise<void>;
    onLeaveTeam: () => Promise<void>;
    headerMsg: { text: string; color: string } | null;

    // Members
    currentUserId: string;
    onChangeRole: (userId: string, role: TeamRoleLabel) => void | Promise<void>;
    onChangeMusicalRole: (userId: string, val: string) => void | Promise<void>;
    onKick: (userId: string) => void | Promise<void>;
    canChangeRoleFn: (m: TeamMember) => boolean;
    canEditMusicalRoleFn: (m: TeamMember) => boolean;
    canKickFn: (m: TeamMember) => boolean;
    membersMsg: { text: string; color: string } | null;

    // Invites
    invites: InviteView[];
    onInvite: (username: string) => void | Promise<void>;
    onCancelInvite: (inviteId: string) => void | Promise<void>;
    invitesMsg: { text: string; color: string } | null;
};

const ModalBody: React.FC<BodyProps> = (p) => {
    const {
        onClose,
        teamLoading, teamError,
        invitesLoading, invitesError,
        teamName, teamPictureUrl, members,
        canRename, canChangePicture, canDeleteTeam, canLeaveTeam,
        onRename, onChangePicture, onDeleteTeam, onLeaveTeam, headerMsg,
        currentUserId, onChangeRole, onChangeMusicalRole, onKick,
        canChangeRoleFn, canEditMusicalRoleFn, canKickFn, membersMsg,
        invites, onInvite, onCancelInvite, invitesMsg,
    } = p;

    const modalRef = useRef<HTMLDivElement | null>(null);
    const scrollRef = useRef<HTMLDivElement | null>(null);



    return (
        <div
            className={styles.modalBackdrop}
            role="dialog"
            aria-modal="true"
            onClick={onClose}
        >
            <div
                ref={modalRef}
                className={styles.modalBody}
                onClick={(e) => e.stopPropagation()}
            >
                <button
                    type="button"
                    className={styles.modalClose}
                    aria-label="Close settings"
                    onClick={onClose}
                >
                    ×
                </button>

                <div ref={scrollRef} className={styles.contentScrollable}>
                    <div className={styles.heroAvatarDock}>
                        <AvatarUploader
                            imageUrl={teamPictureUrl ?? null}
                            canEdit={canChangePicture}
                            onUpload={onChangePicture}
                            className={styles.avatar}
                        />
                    </div>

                    <div id="avatar-controls-slot" className={styles.avatarControlsSlot} />

                    {/* HEADER */}
                    <TeamHeader
                        teamName={teamName}
                        teamPictureUrl={teamPictureUrl}
                        canRename={canRename}
                        canChangePicture={canChangePicture}
                        canDeleteTeam={canDeleteTeam}
                        canLeaveTeam={canLeaveTeam}
                        onRename={onRename}
                        onChangePicture={onChangePicture}
                        onDeleteTeam={onDeleteTeam}
                        onLeaveTeam={onLeaveTeam}
                    />
                    <MessageSlot message={headerMsg} className={styles.teamInfoMsg} />

                    {/* LOADING/ERRORS */}
                    {(teamLoading || invitesLoading) && (
                        <p className={styles.role}>Loading…</p>
                    )}
                    {teamError && <p className={styles.role}>{teamError}</p>}
                    {invitesError && <p className={styles.role}>{invitesError}</p>}

                    <hr className={styles.lineBreak} />

                    {/* MEMBERS */}
                    <h3 className={styles.subtitle}>Members</h3>
                    <MembersList
                        members={members}
                        currentUserId={currentUserId}
                        onChangeRole={onChangeRole}
                        onChangeMusicalRole={onChangeMusicalRole}
                        onKick={onKick}
                        canChangeRole={canChangeRoleFn}
                        canEditMusicalRole={canEditMusicalRoleFn}
                        canKick={canKickFn}
                    />
                    <MessageSlot message={membersMsg} className={styles.teamInfoMsg} />

                    <hr className={styles.lineBreak} />

                    {/* INVITES */}
                    <h3 className={styles.subtitle}>Invites</h3>
                    <InvitesList invites={invites} onCancel={onCancelInvite} />
                    <h3 className={styles.subtitle}>Invite User</h3>
                    <InviteForm onInvite={onInvite} />
                    <MessageSlot message={invitesMsg} className={styles.teamInfoMsg} />
                </div>
            </div>
        </div>
    );
};
