import { describe, it, expect, afterEach, vi } from 'vitest';
import { fetchRangerSchedulesByDateRange, RangerSchedule } from '../RangerScheduleService';
import { ReasonOfAbsence } from '../AttendenceService';

describe('RangerScheduleService', () => {
    afterEach(() => {
        vi.restoreAllMocks();
    });

    const mockDistrictId = 1;
    const mockStartDate = '2024-04-01';
    const mockEndDate = '2024-04-05';

    describe('fetchRangerSchedulesByDateRange', () => {
        it('should return the assembled ranger schedules within the date range', async () => {
            // arrange
            const mockSchedules: RangerSchedule[] = [
                {
                    date: '2024-04-01',
                    ranger: { id: 1, firstName: 'Name', lastName: 'Surname', email: 'test@gmail.com', districtId: 1 },
                    working: true,
                    from: null,
                    reasonOfAbsence: ReasonOfAbsence.None,
                    routeIds: [1, 2],
                    vehicleIds: [3]
                }
            ];

            // mocking fetch
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: true,
                json: async () => mockSchedules,
            }));

            // act
            const result = await fetchRangerSchedulesByDateRange(mockDistrictId, mockStartDate, mockEndDate);

            // assert
            expect(result).toEqual(mockSchedules);
            expect(fetch).toHaveBeenCalledWith(
                `/api/Plan/by-dates/${mockDistrictId}/${mockStartDate}/${mockEndDate}`
            );
        });

        it('should throw an error if the response is not ok', async () => {
            // arrange
            const mockErrorMessage = 'Error getting ranger schedules.';

            // mocking failing fetch
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => mockErrorMessage,
            }));

            // act + assert
            await expect(fetchRangerSchedulesByDateRange(mockDistrictId, mockStartDate, mockEndDate)).rejects.toThrow(mockErrorMessage);

            expect(fetch).toHaveBeenCalledWith(
                `/api/Plan/by-dates/${mockDistrictId}/${mockStartDate}/${mockEndDate}`
            );
        });
    });
});