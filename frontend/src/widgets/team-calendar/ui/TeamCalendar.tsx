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
import styles from './TeamCalendar.module.css';

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

const getEventEndDate = (event: TeamEvent) =>
    new Date(new Date(event.startDateTime).getTime() + event.durationMinutes * 60 * 1000);

const TeamCalendar = ({ teamId }: Props) => {
    const today = useMemo(() => new Date(), []);
    const [visibleWeekStart, setVisibleWeekStart] = useState(() => startOfWeek(today));
    const [selectedDay, setSelectedDay] = useState<Date | null>(() => startOfDay(today));
    const {
        events,
        loading: eventsLoading,
        error: eventsError,
        setRange: setEventsRange,
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

    return (
        <section className={styles.calendarPanel}>
            <div className={styles.calendarTopBar}>
                <h2 className={styles.calendarTitle}>Calendar</h2>
                <button type="button" className={styles.addEventButton}>
                    <PlusIcon className={styles.buttonIcon} />
                    Add Event
                </button>
            </div>

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
                                {formatTime(event.startDateTime)} - {formatTime(getEventEndDate(event).toISOString())}
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
