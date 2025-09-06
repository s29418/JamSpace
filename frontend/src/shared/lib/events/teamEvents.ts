import type { Team } from 'entities/team/model/types';

export type TeamUpdatedDetail = { teamId: string; patch?: Partial<Team> };
export type TeamRemovedDetail = { teamId: string; reason: 'deleted' | 'left' };

const EVT_UPDATED = 'team:updated';
const EVT_REMOVED = 'team:removed';

export function emitTeamUpdated(teamId: string, patch?: Partial<Team>) {
    window.dispatchEvent(new CustomEvent<TeamUpdatedDetail>(EVT_UPDATED, { detail: { teamId, patch } }));
}
export function emitTeamRemoved(teamId: string, reason: 'deleted' | 'left') {
    window.dispatchEvent(new CustomEvent<TeamRemovedDetail>(EVT_REMOVED, { detail: { teamId, reason } }));
}

export function onTeamUpdated(handler: (d: TeamUpdatedDetail) => void) {
    const listener = (e: Event) => handler((e as CustomEvent<TeamUpdatedDetail>).detail);
    window.addEventListener(EVT_UPDATED, listener);
    return () => window.removeEventListener(EVT_UPDATED, listener);
}
export function onTeamRemoved(handler: (d: TeamRemovedDetail) => void) {
    const listener = (e: Event) => handler((e as CustomEvent<TeamRemovedDetail>).detail);
    window.addEventListener(EVT_REMOVED, listener);
    return () => window.removeEventListener(EVT_REMOVED, listener);
}
