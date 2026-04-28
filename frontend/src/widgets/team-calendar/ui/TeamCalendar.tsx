import React, { useEffect, useLayoutEffect, useMemo, useRef, useState } from 'react';
import { Link } from 'react-router-dom';
import {
    ChevronLeftIcon,
    ChevronRightIcon,
    ChevronDownIcon,
    ChevronUpIcon,
    CogIcon as SettingsIcon,
    PlusIcon,
    XMarkIcon,
    TrashIcon,
} from '@heroicons/react/24/outline';
import { ChevronDownIcon as ChevronDownSolidIcon } from '@heroicons/react/20/solid';
import { useTeamEvents } from 'features/team/team-events/model/useTeamEvents';
import type { TeamEvent, TeamRoleLabel } from 'entities/team/model/types';
import { ApiError, isApiError } from 'shared/api/base';
import ConfirmDialog from 'shared/ui/confirm-dialog/ConfirmDialog';
import styles from './TeamCalendar.module.css';
import DateTimeRangePicker from './DateTimeRangePicker';

type Props = {
    teamId: string;
    currentUserId: string;
    currentUserRole: TeamRoleLabel;
};

type CalendarViewMode = 'week' | 'month';

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

const addMonths = (date: Date, amount: number) => {
    const next = new Date(date);
    next.setMonth(next.getMonth() + amount);
    return next;
};

const addMinutes = (date: Date, amount: number) =>
    new Date(date.getTime() + amount * 60 * 1000);

const startOfMonth = (date: Date) => {
    const next = new Date(date);
    next.setDate(1);
    next.setHours(0, 0, 0, 0);
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

const isSameMonth = (a: Date, b: Date) =>
    a.getFullYear() === b.getFullYear() &&
    a.getMonth() === b.getMonth();

const formatTime = (dateTime: string) =>
    new Intl.DateTimeFormat('en-GB', {
        hour: '2-digit',
        minute: '2-digit',
        hour12: false,
    }).format(new Date(dateTime));

const formatShortDate = (date: Date) =>
    new Intl.DateTimeFormat('en-GB', {
        day: 'numeric',
        month: 'short',
    }).format(date);

const formatMonthLabel = (date: Date) =>
    new Intl.DateTimeFormat('en-GB', {
        month: 'long',
        year: 'numeric',
    }).format(date);

const formatEventTimeRange = (event: TeamEvent) => {
    const start = new Date(event.startDateTime);
    const end = getEventEndDate(event);
    const startLabel = `${formatShortDate(start)} ${formatTime(event.startDateTime)}`;
    const endLabel = isSameDay(start, end)
        ? formatTime(end.toISOString())
        : `${formatShortDate(end)} ${formatTime(end.toISOString())}`;

    return `${startLabel} - ${endLabel}`;
};

const getEventEndDate = (event: TeamEvent) =>
    new Date(new Date(event.startDateTime).getTime() + event.durationMinutes * 60 * 1000);

const getDefaultEventStart = (
    selectedDay: Date | null,
    viewMode: CalendarViewMode,
    visibleDate: Date,
    today: Date
) => {
    const base = selectedDay ? new Date(selectedDay) : new Date();

    if (selectedDay) {
        base.setHours(10, 0, 0, 0);
        return base;
    }

    if (viewMode === 'week') {
        const visibleWeekStart = startOfWeek(visibleDate);
        if (!isSameDay(visibleWeekStart, startOfWeek(today))) {
            base.setTime(visibleWeekStart.getTime());
            base.setHours(10, 0, 0, 0);
            return base;
        }
    }

    if (viewMode === 'month') {
        const visibleMonthStart = startOfMonth(visibleDate);
        if (!isSameMonth(visibleMonthStart, today)) {
            base.setTime(visibleMonthStart.getTime());
            base.setHours(10, 0, 0, 0);
            return base;
        }
    }

    const roundedMinutes = Math.ceil(base.getMinutes() / 15) * 15;
    base.setMinutes(roundedMinutes, 0, 0);
    return base;
};

const getErrorMessage = (error: unknown, fallback: string) =>
    isApiError(error) ? (error as ApiError).message : fallback;

const getAvatarFallback = (displayName: string) =>
    displayName.trim().charAt(0).toUpperCase() || '?';

const TeamCalendar = ({ teamId, currentUserId, currentUserRole }: Props) => {
    const today = useMemo(() => new Date(), []);
    const [viewMode, setViewMode] = useState<CalendarViewMode>('week');
    const [visibleDate, setVisibleDate] = useState(() => startOfDay(today));
    const [selectedDay, setSelectedDay] = useState<Date | null>(() => startOfDay(today));
    const [isModeMenuOpen, setIsModeMenuOpen] = useState(false);
    const [isCreateOpen, setIsCreateOpen] = useState(false);
    const [eventName, setEventName] = useState('');
    const [description, setDescription] = useState('');
    const [startsAt, setStartsAt] = useState<Date | null>(null);
    const [endsAt, setEndsAt] = useState<Date | null>(null);
    const [editingEvent, setEditingEvent] = useState<TeamEvent | null>(null);
    const [formError, setFormError] = useState<string | null>(null);
    const [actionError, setActionError] = useState<string | null>(null);
    const [saving, setSaving] = useState(false);
    const [deletingEventId, setDeletingEventId] = useState<string | null>(null);
    const [eventToDelete, setEventToDelete] = useState<TeamEvent | null>(null);
    const [expandedEventId, setExpandedEventId] = useState<string | null>(null);
    const modeSwitcherRef = useRef<HTMLDivElement | null>(null);
    const rangeViewContentRef = useRef<HTMLDivElement | null>(null);
    const [rangeViewHeight, setRangeViewHeight] = useState<number | null>(null);
    const {
        events,
        loading: eventsLoading,
        error: eventsError,
        setRange: setEventsRange,
        refresh,
        createEvent,
        editEvent,
        removeEvent,
    } = useTeamEvents(teamId);
    const isFormOpen = isCreateOpen || !!editingEvent;
    const visibleWeekStart = useMemo(() => startOfWeek(visibleDate), [visibleDate]);
    const visibleMonthStart = useMemo(() => startOfMonth(visibleDate), [visibleDate]);

    const weekDays = useMemo(
        () => Array.from({ length: 7 }, (_, index) => addDays(visibleWeekStart, index)),
        [visibleWeekStart]
    );
    const monthDays = useMemo(() => {
        const monthStart = visibleMonthStart;
        const monthEnd = addDays(addMonths(monthStart, 1), -1);
        const gridStart = startOfWeek(monthStart);
        const gridEnd = addDays(startOfWeek(monthEnd), 6);
        const days: Date[] = [];

        for (let cursor = gridStart; cursor <= gridEnd; cursor = addDays(cursor, 1)) {
            days.push(cursor);
        }

        return days;
    }, [visibleMonthStart]);

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

        if (viewMode === 'month') {
            return {
                from: visibleMonthStart,
                to: addMonths(visibleMonthStart, 1),
            };
        }

        return {
            from: isCurrentWeek ? new Date() : visibleWeekStart,
            to: addDays(visibleWeekStart, 7),
        };
    }, [isCurrentWeek, selectedDay, viewMode, visibleMonthStart, visibleWeekStart]);

    useEffect(() => {
        setEventsRange(calendarRange);
    }, [calendarRange, setEventsRange]);

    useLayoutEffect(() => {
        if (!rangeViewContentRef.current) return;

        setRangeViewHeight(rangeViewContentRef.current.scrollHeight);
    }, [viewMode, selectedDay, visibleDate]);

    useEffect(() => {
        if (!isModeMenuOpen) return;

        const handlePointerDown = (event: MouseEvent) => {
            if (!modeSwitcherRef.current?.contains(event.target as Node)) {
                setIsModeMenuOpen(false);
            }
        };

        document.addEventListener('mousedown', handlePointerDown);
        return () => {
            document.removeEventListener('mousedown', handlePointerDown);
        };
    }, [isModeMenuOpen]);

    const handleRangeChange = (direction: -1 | 1) => {
        setVisibleDate(prev => viewMode === 'week' ? addDays(prev, direction * 7) : addMonths(prev, direction));
        setSelectedDay(null);
    };

    const handleDayClick = (day: Date) => {
        if (selectedDay && isSameDay(selectedDay, day)) {
            setSelectedDay(null);
            return;
        }

        setVisibleDate(startOfDay(day));
        setSelectedDay(startOfDay(day));
    };

    const handleRangeModeClick = () => {
        setSelectedDay(null);
        setIsModeMenuOpen(false);
    };

    const handleModeSelect = (mode: CalendarViewMode) => {
        setViewMode(mode);
        setSelectedDay(null);
        setIsModeMenuOpen(false);
    };

    const handleBottomModeToggle = () => {
        setViewMode(prev => {
            const nextMode = prev === 'week' ? 'month' : 'week';
            if (selectedDay) {
                setVisibleDate(startOfDay(selectedDay));
            }
            return nextMode;
        });
        setIsModeMenuOpen(false);
    };

    const openCreateForm = () => {
        const defaultStart = getDefaultEventStart(selectedDay, viewMode, visibleDate, today);
        setEditingEvent(null);
        setEventName('');
        setDescription('');
        setStartsAt(defaultStart);
        setEndsAt(addMinutes(defaultStart, 60));
        setFormError(null);
        setActionError(null);
        setIsCreateOpen(true);
    };

    const openEditForm = (teamEvent: TeamEvent) => {
        const eventStart = new Date(teamEvent.startDateTime);
        setIsCreateOpen(false);
        setEditingEvent(teamEvent);
        setEventName(teamEvent.title);
        setDescription(teamEvent.description ?? '');
        setStartsAt(eventStart);
        setEndsAt(getEventEndDate(teamEvent));
        setFormError(null);
        setActionError(null);
    };

    const closeEventForm = () => {
        if (saving) return;
        setIsCreateOpen(false);
        setEditingEvent(null);
        setFormError(null);
    };

    const handleEventSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        setFormError(null);
        setActionError(null);

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
            setSaving(true);
            const payload = {
                title: trimmedName,
                description: trimmedDescription || null,
                startDateTime: startsAt.toISOString(),
                durationMinutes,
            };

            if (editingEvent) {
                await editEvent(editingEvent.id, payload);
            } else {
                await createEvent(payload);
            }

            await refresh();
            setIsCreateOpen(false);
            setEditingEvent(null);
        } catch (e) {
            setFormError(getErrorMessage(e, editingEvent ? 'Failed to edit event.' : 'Failed to create event.'));
        } finally {
            setSaving(false);
        }
    };

    const handleDeleteEvent = async (teamEvent: TeamEvent) => {
        if (deletingEventId) return;

        setActionError(null);
        try {
            setDeletingEventId(teamEvent.id);
            await removeEvent(teamEvent.id);
            await refresh();
            setEventToDelete(null);
        } catch (e) {
            setActionError(getErrorMessage(e, 'Failed to delete event.'));
        } finally {
            setDeletingEventId(null);
        }
    };

    const canEditEvent = (teamEvent: TeamEvent) => teamEvent.createdById === currentUserId;

    const canDeleteEvent = (teamEvent: TeamEvent) =>
        teamEvent.createdById === currentUserId ||
        currentUserRole === 'Admin' ||
        currentUserRole === 'Leader';
    const currentModeLabel = viewMode === 'week' ? 'Week' : 'Month';
    const isRangeModeActive = selectedDay === null;
    const showDimmedDays = selectedDay !== null;

    return (
        <section className={styles.calendarPanel}>
            <div className={styles.calendarTopBar}>
                <div className={styles.calendarTitleSpacer} aria-hidden="true" />
                <button type="button" className={styles.addEventButton} onClick={openCreateForm}>
                    <PlusIcon className={styles.buttonIcon} />
                    Add Event
                </button>
            </div>

            {isFormOpen && (
                <div
                    className={styles.formOverlay}
                    role="presentation"
                    onClick={closeEventForm}
                >
                    <form
                        className={styles.eventForm}
                        onSubmit={handleEventSubmit}
                        onClick={(event) => event.stopPropagation()}
                    >
                        <div className={styles.formHeader}>
                            <h3 className={styles.formTitle}>{editingEvent ? 'Edit Event' : 'Add Event'}</h3>
                            <button
                                type="button"
                                className={styles.closeButton}
                                onClick={closeEventForm}
                                aria-label="Close event form"
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
                            <span className={styles.fieldLabel}>Description  (optional)</span>
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
                                onClick={closeEventForm}
                                disabled={saving}
                            >
                                Cancel
                            </button>
                            <button type="submit" className={styles.primaryButton} disabled={saving}>
                                {saving ? 'Saving...' : editingEvent ? 'Save Changes' : 'Save Event'}
                            </button>
                        </div>
                    </form>
                </div>
            )}

            <div className={styles.weekHeader}>
                <button
                    type="button"
                    className={styles.weekNavButton}
                    onClick={() => handleRangeChange(-1)}
                    aria-label={viewMode === 'week' ? 'Previous week' : 'Previous month'}
                >
                    <ChevronLeftIcon className={styles.weekNavIcon} />
                </button>

                <div className={styles.modeSwitcher} ref={modeSwitcherRef}>
                    <button
                        type="button"
                        className={[
                            styles.weekTitleActionButton,
                            styles.weekTitle,
                            isRangeModeActive ? styles.weekTitleActive : '',
                        ].filter(Boolean).join(' ')}
                        onClick={handleRangeModeClick}
                        aria-pressed={isRangeModeActive}
                    >
                        <span className={styles.weekTitleText}>{currentModeLabel}</span>
                    </button>

                    <button
                        type="button"
                        className={styles.modeMenuToggle}
                        onClick={() => setIsModeMenuOpen(prev => !prev)}
                        aria-label={`Open ${currentModeLabel.toLowerCase()} mode menu`}
                        aria-haspopup="menu"
                        aria-expanded={isModeMenuOpen}
                    >
                        <ChevronDownIcon className={styles.modeTitleIcon} />
                    </button>

                    {isModeMenuOpen && (
                        <div className={styles.modeMenu} role="menu">
                            {(['week', 'month'] as CalendarViewMode[]).map((mode) => (
                                <button
                                    key={mode}
                                    type="button"
                                    className={[
                                        styles.modeMenuButton,
                                        viewMode === mode ? styles.modeMenuButtonActive : '',
                                    ].filter(Boolean).join(' ')}
                                    onClick={() => handleModeSelect(mode)}
                                    role="menuitemradio"
                                    aria-checked={viewMode === mode}
                                >
                                    {mode === 'week' ? 'Week' : 'Month'}
                                </button>
                            ))}
                        </div>
                    )}
                </div>

                <button
                    type="button"
                    className={styles.weekNavButton}
                    onClick={() => handleRangeChange(1)}
                    aria-label={viewMode === 'week' ? 'Next week' : 'Next month'}
                >
                    <ChevronRightIcon className={styles.weekNavIcon} />
                </button>
            </div>

            <div
                className={styles.rangeViewViewport}
                style={rangeViewHeight ? { height: `${rangeViewHeight}px` } : undefined}
            >
                <div
                    ref={rangeViewContentRef}
                    className={styles.rangeViewContent}
                    key={viewMode}
                >
                    {viewMode === 'week' ? (
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
                                            showDimmedDays && !active ? styles.dayButtonDimmed : '',
                                        ].filter(Boolean).join(' ')}
                                        onClick={() => handleDayClick(day)}
                                    >
                                        <span className={styles.dayName}>{dayLabels[index]}</span>
                                        <span className={styles.dayNumber}>{day.getDate()}</span>
                                    </button>
                                );
                            })}
                        </div>
                    ) : (
                        <div className={styles.monthView}>
                            <p className={styles.periodLabel}>{formatMonthLabel(visibleMonthStart)}</p>

                            <div className={styles.calendarMonthWeekdaysRow}>
                                {dayLabels.map(label => (
                                    <span key={label} className={styles.calendarMonthWeekdayLabel}>{label}</span>
                                ))}
                            </div>

                            <div className={styles.calendarMonthDaysGrid}>
                                {monthDays.map(day => {
                                    const active = selectedDay ? isSameDay(selectedDay, day) : false;
                                    const current = isSameDay(today, day);
                                    const outsideMonth = !isSameMonth(day, visibleMonthStart);

                                    return (
                                        <button
                                            type="button"
                                            key={day.toISOString()}
                                            className={[
                                                styles.calendarMonthDayButton,
                                                active ? styles.calendarMonthDayButtonActive : '',
                                                current ? styles.calendarMonthDayButtonToday : '',
                                                outsideMonth ? styles.calendarMonthDayButtonMuted : '',
                                                showDimmedDays && !active ? styles.calendarMonthDayButtonDimmed : '',
                                            ].filter(Boolean).join(' ')}
                                            onClick={() => handleDayClick(day)}
                                        >
                                            <span className={styles.calendarMonthDayNumber}>{day.getDate()}</span>
                                        </button>
                                    );
                                })}
                            </div>
                        </div>
                    )}

                    <button
                        type="button"
                        className={styles.calendarModeToggle}
                        onClick={handleBottomModeToggle}
                        aria-label={viewMode === 'week' ? 'Switch to month view' : 'Switch to week view'}
                    >
                        {viewMode === 'week'
                            ? <ChevronDownIcon className={styles.calendarModeToggleIcon} />
                            : <ChevronUpIcon className={styles.calendarModeToggleIcon} />}
                    </button>
                </div>
            </div>

            <div className={styles.eventsList}>
                {eventsLoading && <p className={styles.calendarNote}>Loading events...</p>}
                {!eventsLoading && eventsError && <p className={styles.calendarError}>{eventsError}</p>}
                {!eventsLoading && actionError && <p className={styles.calendarError}>{actionError}</p>}
                {!eventsLoading && !eventsError && events.length === 0 && (
                    <p className={styles.calendarNote}>No events in this range.</p>
                )}
                {!eventsLoading && !eventsError && events.map(event => {
                    const canEdit = canEditEvent(event);
                    const canDelete = canDeleteEvent(event);
                    const showActions = canEdit || canDelete;

                    return (
                    <article
                        className={[
                            styles.eventCard,
                            expandedEventId === event.id ? styles.eventCardExpanded : '',
                        ].filter(Boolean).join(' ')}
                        key={event.id}
                    >
                        <div className={styles.eventRow}>
                            <div className={styles.eventDetails}>
                                <h4 className={styles.eventTitle}>{event.title}</h4>
                                <p className={styles.eventTime}>
                                    {formatEventTimeRange(event)}
                                </p>
                            </div>

                            {showActions && (
                                <div className={styles.eventActions}>
                                    {canEdit && (
                                        <button
                                            type="button"
                                            className={styles.eventActionButton}
                                            onClick={() => openEditForm(event)}
                                            disabled={deletingEventId === event.id}
                                        >
                                            <SettingsIcon className={styles.eventActionIcon} />
                                            Edit
                                        </button>
                                    )}
                                    {canDelete && (
                                        <button
                                            type="button"
                                            className={styles.deleteButton}
                                            onClick={() => setEventToDelete(event)}
                                            disabled={deletingEventId === event.id}
                                        >
                                            <TrashIcon className={styles.eventActionIcon} />
                                            {deletingEventId === event.id ? 'Deleting...' : 'Delete'}
                                        </button>
                                    )}
                                </div>
                            )}
                        </div>

                        <button
                            type="button"
                            className={styles.eventExpandButton}
                            onClick={() => setExpandedEventId(prev => prev === event.id ? null : event.id)}
                            aria-expanded={expandedEventId === event.id}
                            aria-label={expandedEventId === event.id ? 'Hide event details' : 'Show event details'}
                        >
                            {expandedEventId === event.id
                                ? <ChevronUpIcon className={styles.eventExpandIcon} />
                                : <ChevronDownIcon className={styles.eventExpandIcon} />}
                        </button>

                        {expandedEventId === event.id && (
                            <div className={styles.eventMeta}>
                                <div className={styles.eventMetaMain}>
                                    <div className={styles.eventCreator}>
                                        <Link
                                            to={`/profile/${event.createdById}`}
                                            className={styles.eventCreatorLink}
                                            aria-label={`Open ${event.createdByDisplayName} profile`}
                                        >
                                            {event.createdByAvatarUrl ? (
                                                <img
                                                    src={event.createdByAvatarUrl}
                                                    alt={event.createdByDisplayName}
                                                    className={styles.eventCreatorAvatar}
                                                    loading="lazy"
                                                    decoding="async"
                                                />
                                            ) : (
                                                <div className={styles.eventCreatorFallback}>
                                                    {getAvatarFallback(event.createdByDisplayName)}
                                                </div>
                                            )}
                                        </Link>

                                        <div className={styles.eventCreatorText}>
                                            <span className={styles.eventCreatorLabel}>Created by</span>
                                            <Link
                                                to={`/profile/${event.createdById}`}
                                                className={styles.eventCreatorNameLink}
                                            >
                                                <span className={styles.eventCreatorName}>{event.createdByDisplayName}</span>
                                            </Link>
                                        </div>
                                    </div>


                                    {event.description && (
                                        <div className={styles.eventDescriptionBox}>
                                            <p className={styles.eventCreatorLabel}>Description:</p>
                                            <p className={styles.eventDescription}>{event.description}</p>
                                        </div>
                                    )}
                                </div>
                            </div>
                        )}
                    </article>
                    );
                })}
            </div>

            <ConfirmDialog
                isOpen={!!eventToDelete}
                title="Delete event"
                message={`Are you sure you want to delete "${eventToDelete?.title ?? 'this event'}"?`}
                loading={!!deletingEventId}
                onConfirm={async () => {
                    if (eventToDelete) await handleDeleteEvent(eventToDelete);
                }}
                onCancel={() => {
                    if (!deletingEventId) setEventToDelete(null);
                }}
            />
        </section>
    );
};

export default TeamCalendar;
