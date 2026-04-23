import React, { useEffect, useMemo, useState } from 'react';
import {
    ChevronLeftIcon,
    ChevronRightIcon,
    CogIcon as SettingsIcon,
    PlusIcon,
    XMarkIcon,
} from '@heroicons/react/24/outline';
import { useTeamEvents } from 'features/team/team-events/model/useTeamEvents';
import type { TeamEvent } from 'entities/team/model/types';
import { ApiError, isApiError } from 'shared/api/base';
import styles from './TeamCalendar.module.css';
import DateTimeRangePicker from './DateTimeRangePicker';

type Props = {
    teamId: string;
};

const dayLabels = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

const startOfDay = (date: Date) => {
    const next = new Date(date);
    next.setHours(0, 0, 0, 0);
    return next;
};

const addDays = (date: Date, amount: number) => {
    const next = new Date(date);
    next.setDate(next.getDate() + amount);
    return next;
};

const addMinutes = (date: Date, amount: number) =>
    new Date(date.getTime() + amount * 60 * 1000);

const startOfWeek = (date: Date) => {
    const day = date.getDay();
    const diff = day === 0 ? -6 : 1 - day;
    return startOfDay(addDays(date, diff));
};

const isSameDay = (a: Date, b: Date) =>
    a.getFullYear() === b.getFullYear() &&
    a.getMonth() === b.getMonth() &&
    a.getDate() === b.getDate();

const formatTime = (dateTime: string) =>
    new Intl.DateTimeFormat('en-GB', {
        hour: '2-digit',
        minute: '2-digit',
        hour12: false,
    }).format(new Date(dateTime));

const formatShortDate = (date: Date) =>
    new Intl.DateTimeFormat('en-GB', {
        day: '2-digit',
        month: 'short',
    }).format(date);

const formatEventTimeRange = (event: TeamEvent, selectedDay: Date | null) => {
    const start = new Date(event.startDateTime);
    const end = getEventEndDate(event);
    const startTime = formatTime(event.startDateTime);
    const endTime = formatTime(end.toISOString());

    if (selectedDay && startOfDay(start) < startOfDay(selectedDay) && end > startOfDay(selectedDay)) {
        return `${formatShortDate(start)} ${startTime} - ${endTime}`;
    }

    return `${startTime} - ${endTime}`;
};

const getEventEndDate = (event: TeamEvent) =>
    new Date(new Date(event.startDateTime).getTime() + event.durationMinutes * 60 * 1000);

const getDefaultEventStart = (selectedDay: Date | null, visibleWeekStart: Date) => {
    const base = selectedDay ? new Date(selectedDay) : new Date();
    if (!selectedDay && !isSameDay(startOfWeek(base), visibleWeekStart)) {
        base.setTime(visibleWeekStart.getTime());
        base.setHours(10, 0, 0, 0);
        return base;
    }

    if (selectedDay) {
        base.setHours(10, 0, 0, 0);
        return base;
    }

    const roundedMinutes = Math.ceil(base.getMinutes() / 15) * 15;
    base.setMinutes(roundedMinutes, 0, 0);
    return base;
};

const getErrorMessage = (error: unknown, fallback: string) =>
    isApiError(error) ? (error as ApiError).message : fallback;

const TeamCalendar = ({ teamId }: Props) => {
    const today = useMemo(() => new Date(), []);
    const [visibleWeekStart, setVisibleWeekStart] = useState(() => startOfWeek(today));
    const [selectedDay, setSelectedDay] = useState<Date | null>(() => startOfDay(today));
    const [isCreateOpen, setIsCreateOpen] = useState(false);
    const [eventName, setEventName] = useState('');
    const [description, setDescription] = useState('');
    const [startsAt, setStartsAt] = useState<Date | null>(null);
    const [endsAt, setEndsAt] = useState<Date | null>(null);
    const [formError, setFormError] = useState<string | null>(null);
    const [creating, setCreating] = useState(false);
    const {
        events,
        loading: eventsLoading,
        error: eventsError,
        setRange: setEventsRange,
        refresh,
        createEvent,
    } = useTeamEvents(teamId);

    const weekDays = useMemo(
        () => Array.from({ length: 7 }, (_, index) => addDays(visibleWeekStart, index)),
        [visibleWeekStart]
    );

    const isCurrentWeek = useMemo(
        () => isSameDay(visibleWeekStart, startOfWeek(today)),
        [today, visibleWeekStart]
    );
    const calendarRange = useMemo(() => {
        if (selectedDay) {
            const from = startOfDay(selectedDay);
            return {
                from,
                to: addDays(from, 1),
            };
        }

        return {
            from: isCurrentWeek ? new Date() : visibleWeekStart,
            to: addDays(visibleWeekStart, 7),
        };
    }, [isCurrentWeek, selectedDay, visibleWeekStart]);

    useEffect(() => {
        setEventsRange(calendarRange);
    }, [calendarRange, setEventsRange]);

    const handleWeekChange = (direction: -1 | 1) => {
        setVisibleWeekStart(prev => addDays(prev, direction * 7));
        setSelectedDay(null);
    };

    const handleDayClick = (day: Date) => {
        if (selectedDay && isSameDay(selectedDay, day) && isCurrentWeek) {
            setSelectedDay(null);
            return;
        }

        setSelectedDay(startOfDay(day));
    };

    const openCreateForm = () => {
        const defaultStart = getDefaultEventStart(selectedDay, visibleWeekStart);
        setEventName('');
        setDescription('');
        setStartsAt(defaultStart);
        setEndsAt(addMinutes(defaultStart, 60));
        setFormError(null);
        setIsCreateOpen(true);
    };

    const closeCreateForm = () => {
        if (creating) return;
        setIsCreateOpen(false);
        setFormError(null);
    };

    const handleCreateSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        setFormError(null);

        const trimmedName = eventName.trim();
        const trimmedDescription = description.trim();

        if (!trimmedName) {
            setFormError('Event name is required.');
            return;
        }

        if (!startsAt || !endsAt || Number.isNaN(startsAt.getTime()) || Number.isNaN(endsAt.getTime())) {
            setFormError('Choose start and end date.');
            return;
        }

        const durationMinutes = Math.round((endsAt.getTime() - startsAt.getTime()) / 60000);

        if (durationMinutes < 5) {
            setFormError('Event must last at least 5 minutes.');
            return;
        }

        if (durationMinutes > 1440) {
            setFormError('Event cannot last longer than 24 hours.');
            return;
        }

        try {
            setCreating(true);
            await createEvent({
                title: trimmedName,
                description: trimmedDescription || null,
                startDateTime: startsAt.toISOString(),
                durationMinutes,
            });
            await refresh();
            setIsCreateOpen(false);
        } catch (e) {
            setFormError(getErrorMessage(e, 'Failed to create event.'));
        } finally {
            setCreating(false);
        }
    };

    return (
        <section className={styles.calendarPanel}>
            <div className={styles.calendarTopBar}>
                <h2 className={styles.calendarTitle}>Calendar</h2>
                <button type="button" className={styles.addEventButton} onClick={openCreateForm}>
                    <PlusIcon className={styles.buttonIcon} />
                    Add Event
                </button>
            </div>

            {isCreateOpen && (
                <div className={styles.formOverlay} role="presentation">
                    <form className={styles.eventForm} onSubmit={handleCreateSubmit}>
                        <div className={styles.formHeader}>
                            <h3 className={styles.formTitle}>Add Event</h3>
                            <button
                                type="button"
                                className={styles.closeButton}
                                onClick={closeCreateForm}
                                aria-label="Close create event form"
                            >
                                <XMarkIcon className={styles.closeIcon} />
                            </button>
                        </div>

                        <label className={styles.field}>
                            <span className={styles.fieldLabel}>Event name</span>
                            <input
                                className={styles.input}
                                value={eventName}
                                onChange={(e) => setEventName(e.target.value)}
                                maxLength={50}
                                required
                            />
                        </label>

                        {startsAt && endsAt && (
                            <DateTimeRangePicker
                                start={startsAt}
                                end={endsAt}
                                onChange={({ start, end }) => {
                                    setStartsAt(start);
                                    setEndsAt(end);
                                }}
                            />
                        )}

                        <label className={styles.field}>
                            <span className={styles.fieldLabel}>Description</span>
                            <textarea
                                className={styles.textarea}
                                value={description}
                                onChange={(e) => setDescription(e.target.value)}
                                maxLength={150}
                                rows={4}
                            />
                        </label>

                        {formError && <p className={styles.formError}>{formError}</p>}

                        <div className={styles.formActions}>
                            <button
                                type="button"
                                className={styles.secondaryButton}
                                onClick={closeCreateForm}
                                disabled={creating}
                            >
                                Cancel
                            </button>
                            <button type="submit" className={styles.primaryButton} disabled={creating}>
                                {creating ? 'Saving...' : 'Save Event'}
                            </button>
                        </div>
                    </form>
                </div>
            )}

            <div className={styles.weekHeader}>
                <button
                    type="button"
                    className={styles.weekNavButton}
                    onClick={() => handleWeekChange(-1)}
                    aria-label="Previous week"
                >
                    <ChevronLeftIcon className={styles.weekNavIcon} />
                </button>

                <h3 className={styles.weekTitle}>Week</h3>

                <button
                    type="button"
                    className={styles.weekNavButton}
                    onClick={() => handleWeekChange(1)}
                    aria-label="Next week"
                >
                    <ChevronRightIcon className={styles.weekNavIcon} />
                </button>
            </div>

            <div className={styles.daysGrid}>
                {weekDays.map((day, index) => {
                    const active = selectedDay ? isSameDay(selectedDay, day) : false;
                    const current = isSameDay(today, day);

                    return (
                        <button
                            type="button"
                            key={day.toISOString()}
                            className={[
                                styles.dayButton,
                                active ? styles.dayButtonActive : '',
                                current ? styles.dayButtonToday : '',
                            ].filter(Boolean).join(' ')}
                            onClick={() => handleDayClick(day)}
                        >
                            <span className={styles.dayName}>{dayLabels[index]}</span>
                            <span className={styles.dayNumber}>{day.getDate()}</span>
                        </button>
                    );
                })}
            </div>

            <div className={styles.eventsList}>
                {eventsLoading && <p className={styles.calendarNote}>Loading events...</p>}
                {!eventsLoading && eventsError && <p className={styles.calendarError}>{eventsError}</p>}
                {!eventsLoading && !eventsError && events.length === 0 && (
                    <p className={styles.calendarNote}>No events in this range.</p>
                )}
                {!eventsLoading && !eventsError && events.map(event => (
                    <article className={styles.eventCard} key={event.id}>
                        <div className={styles.eventDetails}>
                            <h4 className={styles.eventTitle}>{event.title}</h4>
                            <p className={styles.eventTime}>
                                {formatEventTimeRange(event, selectedDay)}
                            </p>
                        </div>

                        <div className={styles.eventActions}>
                            <button type="button" className={styles.eventActionButton}>
                                <SettingsIcon className={styles.eventActionIcon} />
                                Edit
                            </button>
                            <button type="button" className={styles.eventActionButton}>
                                <XMarkIcon className={styles.eventActionIcon} />
                                Delete
                            </button>
                        </div>
                    </article>
                ))}
            </div>
        </section>
    );
};

export default TeamCalendar;
