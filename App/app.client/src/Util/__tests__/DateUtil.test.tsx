import { describe, it, expect  } from 'vitest';
import { formatDate, getShiftedDate, generateDateRange } from '../DateUtil';

describe('formatDate', () => {
    it('formats a date to YYYY-MM-DD', () => {
        const date = new Date('2024-04-05');
        expect(formatDate(date)).toBe('2024-04-05');
    });
});

describe('getShiftedDate', () => {
    it('returns a date shifted forward', () => {
        const date = new Date('2024-04-01');
        expect(getShiftedDate(date, 2).toDateString()).toBe(new Date('2024-04-03').toDateString());
    });

    it('returns a date shifted backward', () => {
        const date = new Date('2024-04-01');
        expect(getShiftedDate(date, -1).toDateString()).toBe(new Date('2024-03-31').toDateString());
    });
    it('returns a date shifted backward across a year', () => {
        const date = new Date('2024-01-01');
        expect(getShiftedDate(date, -1).toDateString()).toBe(new Date('2023-12-31').toDateString());
    });
});

describe('generateDateRange', () => {
    it('returns an array of dates from start to end, inclusive', () => {
        const start = new Date('2024-04-01');
        const end = new Date('2024-04-03');
        const range = generateDateRange(start, end);
        expect(range).toHaveLength(3);
        expect(formatDate(range[0])).toBe('2024-04-01');
        expect(formatDate(range[2])).toBe('2024-04-03');
    });
});
