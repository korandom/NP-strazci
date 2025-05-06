import { describe, it, expect, afterEach, vi } from 'vitest';
import { fetchRangersByDistrict, getCurrentRanger, updateRanger, deleteRanger, createRanger, Ranger} from '../RangerService';

describe('RangerService', () => {
    afterEach(() => {
        vi.restoreAllMocks();
    });

    const mockRanger: Ranger = {
        id: 1,
        firstName: 'Name',
        lastName: 'Surname',
        email: 'est@gmail.com',
        districtId: 1
    };

    describe('fetchRangersByDistrict', () => {
        it('should return all rangers in given district', async () => {
            // arrange
            const mockResponse = [mockRanger];
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: true,
                json: async () => mockResponse
            }));

            // act
            const result = await fetchRangersByDistrict(mockRanger.districtId);

            // assert
            expect(result).toEqual(mockResponse);
            expect(fetch).toHaveBeenCalledWith(`/api/Ranger/in-district/${mockRanger.districtId}`);
        });

        it('should throw an error if response is not ok', async () => {
            const errorMessage = 'District id not valid.';
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => errorMessage
            }));

            // act + assert
            await expect(fetchRangersByDistrict(mockRanger.districtId)).rejects.toThrow(errorMessage);
        });
    });

    describe('getCurrentRanger', () => {
        it('should return the current ranger', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: true,
                json: async () => mockRanger
            }));

            // act
            const result = await getCurrentRanger();

            // assert
            expect(result).toEqual(mockRanger);
            expect(fetch).toHaveBeenCalledWith(`/api/Ranger`);
        });

        it('should return undefined if response is not ok', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({ ok: false }));

            // act
            const result = await getCurrentRanger();

            // assert
            expect(result).toBeUndefined();
        });
    });

    describe('updateRanger', () => {
        it('should update a ranger', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({ ok: true }));

            // act
            await updateRanger(mockRanger);

            // assert
            expect(fetch).toHaveBeenCalledWith(
                `/api/Ranger/update`,
                expect.objectContaining({
                    method: 'PUT',
                    headers: expect.objectContaining({
                        'Content-Type': 'application/json'
                    }),
                    body: JSON.stringify(mockRanger)
                })
            );
        });

        it('should throw an error if response is not ok', async () => {
            // arrange
            const errorMessage = 'Failed to update ranger';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => errorMessage
            }));

            // act + assert
            await expect(updateRanger(mockRanger)).rejects.toThrow(errorMessage);
            expect(fetch).toHaveBeenCalledWith(
                `/api/Ranger/update`,
                expect.objectContaining({
                    method: 'PUT',
                    headers: expect.objectContaining({
                        'Content-Type': 'application/json'
                    }),
                    body: JSON.stringify(mockRanger)
                })
            );
        });
    });

    describe('deleteRanger', () => {
        it('should delete a ranger', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({ ok: true }));

            // act
            await deleteRanger(mockRanger);

            // assert
            expect(fetch).toHaveBeenCalledWith(
                `/api/Ranger/delete/${mockRanger.id}`,
                { method: 'DELETE' }
            );

        });

        it('should throw an error if response is not ok', async () => {
            // arrange
            const errorMessage = 'Ranger id not found.';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => errorMessage
            }));

            // act + assert
            await expect(deleteRanger(mockRanger)).rejects.toThrow(errorMessage);
            expect(fetch).toHaveBeenCalledWith(
                `/api/Ranger/delete/${mockRanger.id}`,
                { method: 'DELETE' }
            );
        });
    });

    describe('createRanger', () => {
        it('should create a ranger and return it', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: true,
                json: async () => mockRanger
            }));

            // act
            const result = await createRanger(mockRanger);

            // assert
            expect(result).toEqual(mockRanger);
            expect(fetch).toHaveBeenCalledWith(
                `/api/Ranger/create`,
                expect.objectContaining({
                    method: 'POST',
                    headers: expect.objectContaining({
                        'Content-Type': 'application/json'
                    }),
                    body: JSON.stringify(mockRanger)
                })
            );
        });

        it('should throw an error if response is not ok', async () => {
            // arrange
            const errorMessage = 'District id not found.';
            // mock fetch method to fail
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => errorMessage
            }));

            // act + assert
            await expect(createRanger(mockRanger)).rejects.toThrow(errorMessage);
        });
    });
});