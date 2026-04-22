import { useCallback, useEffect, useMemo, useState } from 'react';
import {
    createTeamEvent,
    deleteTeamEvent,
    editTeamEvent,
    getTeamEvents,
    type TeamEventsRange,
} from 'entities/team/api/teamEvents.api';
import type {
    CreateTeamEventRequest,
    EditTeamEventRequest,
    TeamEvent,
} from 'entities/team/model/types';
import { ApiError, isApiError } from 'shared/api/base';

const byStartDateTime = (a: TeamEvent, b: TeamEvent) =>
    new Date(a.startDateTime).getTime() - new Date(b.startDateTime).getTime();

const getErrorMessage = (error: unknown, fallback: string) =>
    isApiError(error) ? (error as ApiError).message : fallback;

export function useTeamEvents(teamId?: string, initialRange?: TeamEventsRange) {
    const [events, setEvents] = useState<TeamEvent[]>([]);
    const [range, setRange] = useState<TeamEventsRange | undefined>(initialRange);
    const [loading, setLoading] = useState<boolean>(!!teamId && !!initialRange);
    const [error, setError] = useState<string | null>(null);

    const canLoad = useMemo(() => !!teamId && !!range, [teamId, range]);

    const refresh = useCallback(async (nextRange?: TeamEventsRange) => {
        const rangeToLoad = nextRange ?? range;
        if (!teamId || !rangeToLoad) return [];

        setLoading(true);
        setError(null);
        try {
            const data = await getTeamEvents(teamId, rangeToLoad);
            setEvents(data);
            if (nextRange) setRange(nextRange);
            return data;
        } catch (e) {
            const message = getErrorMessage(e, 'Failed to load team events');
            setError(message);
            throw e;
        } finally {
            setLoading(false);
        }
    }, [range, teamId]);

    useEffect(() => {
        let alive = true;
        if (!canLoad || !teamId || !range) return;

        (async () => {
            setLoading(true);
            setError(null);
            try {
                const data = await getTeamEvents(teamId, range);
                if (alive) setEvents(data);
            } catch (e) {
                if (alive) setError(getErrorMessage(e, 'Failed to load team events'));
            } finally {
                if (alive) setLoading(false);
            }
        })();

        return () => { alive = false; };
    }, [canLoad, range, teamId]);

    const createEvent = useCallback(async (payload: CreateTeamEventRequest) => {
        if (!teamId) throw new ApiError(400, 'Team is not selected');

        const created = await createTeamEvent(teamId, payload);
        setEvents(prev => [...prev, created].sort(byStartDateTime));
        return created;
    }, [teamId]);

    const editEvent = useCallback(async (eventId: string, payload: EditTeamEventRequest) => {
        if (!teamId) throw new ApiError(400, 'Team is not selected');

        const updated = await editTeamEvent(teamId, eventId, payload);
        setEvents(prev => prev.map(event => event.id === eventId ? updated : event).sort(byStartDateTime));
        return updated;
    }, [teamId]);

    const removeEvent = useCallback(async (eventId: string) => {
        if (!teamId) throw new ApiError(400, 'Team is not selected');

        await deleteTeamEvent(teamId, eventId);
        setEvents(prev => prev.filter(event => event.id !== eventId));
    }, [teamId]);

    return {
        events,
        setEvents,
        range,
        setRange,
        loading,
        error,
        refresh,
        createEvent,
        editEvent,
        removeEvent,
    };
}
