import { vi, describe, it, expect, afterEach } from 'vitest';
import { lockPlans, unlockPlans, fetchLocks, Locked } from '../LockService';

describe('LockService', () => {
    afterEach(() => {
        vi.restoreAllMocks();
    });

    const mockDate = '2024-04-01';
    const mockDistrictId = 1;

    describe('lockPlans', () => {
        it('should lock the plans for the district on the given date', async () => {
            // arrange
            const mockResponse = { message: 'Successfully locked day against changes.' };

            // mocking fetch
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: true,
                json: async () => mockResponse,
            }));

            // act
            await lockPlans(mockDate, mockDistrictId);

            // assert
            expect(fetch).toHaveBeenCalledWith(
                `/api/Lock/lock/${mockDistrictId}/${mockDate}`,
                expect.objectContaining({
                    method: 'POST',
                })
            );
        });

        it('should throw an error if the response is not ok', async () => {
            // arrange
            const mockErrorMessage = 'District id not found.';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => mockErrorMessage,
            }));

            // expecting throwing an error
            await expect(lockPlans(mockDate, mockDistrictId)).rejects.toThrow(mockErrorMessage);

            // checking correct call
            expect(fetch).toHaveBeenCalledWith(
                `/api/Lock/lock/${mockDistrictId}/${mockDate}`,
                expect.objectContaining({
                    method: 'POST',
                })
            );
        });
    });

    describe('unlockPlans', () => {
        it('should unlock the plans for the district on the given date', async () => {
            // arrange
            const mockResponse = { message: 'Successfully unlocked.' };

            // mocking fetch
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: true,
                json: async () => mockResponse,
            }));

            // act
            await unlockPlans(mockDate, mockDistrictId);

            // assert
            expect(fetch).toHaveBeenCalledWith(
                `/api/Lock/unlock/${mockDistrictId}/${mockDate}`,
                expect.objectContaining({
                    method: 'DELETE',
                })
            );
        });

        it('should throw an error if the response is not ok', async () => {
            // arrange
            const mockErrorMessage = 'Lock does not exist.';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => mockErrorMessage,
            }));

            // expecting throwing an error
            await expect(unlockPlans(mockDate, mockDistrictId)).rejects.toThrow(mockErrorMessage);

            // checking correct call
            expect(fetch).toHaveBeenCalledWith(
                `/api/Lock/unlock/${mockDistrictId}/${mockDate}`,
                expect.objectContaining({
                    method: 'DELETE',
                })
            );
        });
    });

    describe('fetchLocks', () => {
        it('should return the list of locks for a given district', async () => {
            // arrange
            const mockResponse: Locked[] = [
                { date: '2024-04-01', districtId: 1 },
                { date: '2024-04-02', districtId: 1 },
            ];
            // mocking fetch
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: true,
                json: async () => mockResponse,
            }));

            // act
            const result = await fetchLocks(mockDistrictId);

            // assert
            expect(result).toEqual(mockResponse);
            expect(fetch).toHaveBeenCalledWith(
                `/api/Lock/locks/${mockDistrictId}`
            );
        });

        it('should throw an error if the response is not ok', async () => {
            // arrange
            const mockErrorMessage = 'Locks not found.';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => mockErrorMessage,
            }));

            // expecting throwing an error
            await expect(fetchLocks(mockDistrictId)).rejects.toThrow(mockErrorMessage);

            // checking correct call
            expect(fetch).toHaveBeenCalledWith(
                `/api/Lock/locks/${mockDistrictId}`
            );
        });
    });
});