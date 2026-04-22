import { api } from '../../../shared/api/base';
import type {
    CreateTeamEventRequest,
    EditTeamEventRequest,
    TeamEvent,
} from '../model/types';

const eventsRoot = (teamId: string) => `/teams/${teamId}/events`;

export type TeamEventsRange = {
    from: string | Date;
    to: string | Date;
};

const toDateTimeOffsetParam = (value: string | Date) =>
    value instanceof Date ? value.toISOString() : value;

export const getTeamEvents = async (teamId: string, range: TeamEventsRange) => {
    const res = await api.get<TeamEvent[]>(eventsRoot(teamId), {
        params: {
            from: toDateTimeOffsetParam(range.from),
            to: toDateTimeOffsetParam(range.to),
        },
    });
    return res.data;
};

export const createTeamEvent = async (teamId: string, payload: CreateTeamEventRequest) => {
    const res = await api.post<TeamEvent>(eventsRoot(teamId), payload);
    return res.data;
};

export const editTeamEvent = async (
    teamId: string,
    eventId: string,
    payload: EditTeamEventRequest
) => {
    const res = await api.put<TeamEvent>(`${eventsRoot(teamId)}/${eventId}`, payload);
    return res.data;
};

export const deleteTeamEvent = async (teamId: string, eventId: string) => {
    await api.delete(`${eventsRoot(teamId)}/${eventId}`);
};
