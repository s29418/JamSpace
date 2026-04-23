import React, { useMemo, useState } from 'react';
import {
    ChevronLeftIcon,
    ChevronRightIcon,
} from '@heroicons/react/24/outline';
import styles from './TeamCalendar.module.css';

type Props = {
    start: Date;
    end: Date;
    onChange: (range: { start: Date; end: Date }) => void;
};

const dayLabels = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
const timeStepMinutes = 15;

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
    next.setMonth(next.getMonth() + amount, 1);
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

const isSameMonth = (a: Date, b: Date) =>
    a.getFullYear() === b.getFullYear() &&
    a.getMonth() === b.getMonth();

const formatMonth = (date: Date) =>
    new Intl.DateTimeFormat('en-GB', {
        month: 'long',
        year: 'numeric',
    }).format(date);

const formatTime = (date: Date) =>
    new Intl.DateTimeFormat('en-GB', {
        hour: '2-digit',
        minute: '2-digit',
        hour12: false,
    }).format(date);

const formatShortDate = (date: Date) =>
    new Intl.DateTimeFormat('en-GB', {
        day: '2-digit',
        month: 'short',
    }).format(date);

const minutesFromStartOfDay = (date: Date) =>
    date.getHours() * 60 + date.getMinutes();

const withTime = (date: Date, minutes: number) => {
    const next = startOfDay(date);
    next.setMinutes(minutes, 0, 0);
    return next;
};

const createTimeOptions = () =>
    Array.from(
        { length: Math.floor((24 * 60) / timeStepMinutes) },
        (_, index) => index * timeStepMinutes
    );

const getCalendarDays = (viewMonth: Date) => {
    const firstDay = new Date(viewMonth.getFullYear(), viewMonth.getMonth(), 1);
    const gridStart = startOfWeek(firstDay);

    return Array.from({ length: 42 }, (_, index) => addDays(gridStart, index));
};

const createEndOptions = (start: Date) => {
    const options: Date[] = [];
    const maxEnd = addMinutes(start, 1439);

    for (let offset = timeStepMinutes; offset <= 1425; offset += timeStepMinutes) {
        options.push(addMinutes(start, offset));
    }

    options.push(maxEnd);
    return options;
};

const DateTimeRangePicker = ({ start, end, onChange }: Props) => {
    const [viewMonth, setViewMonth] = useState(() => new Date(start.getFullYear(), start.getMonth(), 1));
    const today = useMemo(() => new Date(), []);
    const calendarDays = useMemo(() => getCalendarDays(viewMonth), [viewMonth]);
    const startTimeOptions = useMemo(() => createTimeOptions(), []);
    const endOptions = useMemo(() => createEndOptions(start), [start]);
    const selectedStartMinutes = minutesFromStartOfDay(start);
    const selectedEnd = endOptions.some(option => option.getTime() === end.getTime())
        ? end
        : endOptions[0];

    const commitStart = (nextStart: Date) => {
        const minEnd = addMinutes(nextStart, timeStepMinutes);
        const maxEnd = addMinutes(nextStart, 1439);
        let nextEnd = end;

        if (nextEnd <= nextStart || nextEnd > maxEnd) {
            nextEnd = addMinutes(nextStart, 60);
        }

        if (nextEnd < minEnd) nextEnd = minEnd;
        if (nextEnd > maxEnd) nextEnd = maxEnd;

        onChange({ start: nextStart, end: nextEnd });
    };

    const handleDaySelect = (day: Date) => {
        commitStart(withTime(day, selectedStartMinutes));
    };

    const handleStartTimeChange = (value: string) => {
        const nextStart = withTime(start, Number(value));
        commitStart(nextStart);
    };

    const handleEndChange = (value: string) => {
        const nextEnd = new Date(Number(value));
        onChange({ start, end: nextEnd });
    };

    return (
        <div className={styles.dateTimePicker}>
            <div className={styles.datePickerPanel}>
                <div className={styles.monthHeader}>
                    <button
                        type="button"
                        className={styles.monthNavButton}
                        onClick={() => setViewMonth(prev => addMonths(prev, -1))}
                        aria-label="Previous month"
                    >
                        <ChevronLeftIcon className={styles.monthNavIcon} />
                    </button>

                    <span className={styles.monthLabel}>{formatMonth(viewMonth)}</span>

                    <button
                        type="button"
                        className={styles.monthNavButton}
                        onClick={() => setViewMonth(prev => addMonths(prev, 1))}
                        aria-label="Next month"
                    >
                        <ChevronRightIcon className={styles.monthNavIcon} />
                    </button>
                </div>

                <div className={styles.monthWeekdays}>
                    {dayLabels.map(day => (
                        <span key={day}>{day}</span>
                    ))}
                </div>

                <div className={styles.monthGrid}>
                    {calendarDays.map(day => {
                        const selected = isSameDay(day, start);
                        const current = isSameDay(day, today);
                        const muted = !isSameMonth(day, viewMonth);

                        return (
                            <button
                                type="button"
                                key={day.toISOString()}
                                className={[
                                    styles.monthDayButton,
                                    selected ? styles.monthDayButtonSelected : '',
                                    current ? styles.monthDayButtonToday : '',
                                    muted ? styles.monthDayButtonMuted : '',
                                ].filter(Boolean).join(' ')}
                                onClick={() => handleDaySelect(day)}
                            >
                                {day.getDate()}
                            </button>
                        );
                    })}
                </div>
            </div>

            <div className={styles.timePickerPanel}>
                <label className={styles.field}>
                    <span className={styles.fieldLabel}>From</span>
                    <select
                        className={styles.select}
                        value={selectedStartMinutes}
                        onChange={(event) => handleStartTimeChange(event.target.value)}
                    >
                        {startTimeOptions.map(minutes => (
                            <option key={minutes} value={minutes}>
                                {formatTime(withTime(start, minutes))}
                            </option>
                        ))}
                    </select>
                </label>

                <label className={styles.field}>
                    <span className={styles.fieldLabel}>To</span>
                    <select
                        className={styles.select}
                        value={selectedEnd.getTime()}
                        onChange={(event) => handleEndChange(event.target.value)}
                    >
                        {endOptions.map(option => (
                            <option key={option.getTime()} value={option.getTime()}>
                                {formatTime(option)}
                                {!isSameDay(option, start) ? ` (${formatShortDate(option)})` : ''}
                            </option>
                        ))}
                    </select>
                </label>
            </div>
        </div>
    );
};

export default DateTimeRangePicker;
