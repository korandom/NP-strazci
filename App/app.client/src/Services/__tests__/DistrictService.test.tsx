import { vi, describe, it, expect, afterEach } from 'vitest';
import { fetchDistrictById, fetchAllDistricts, District } from '../DistrictService';

describe('DistrictService', () => {
    afterEach(() => {
        vi.restoreAllMocks();
    });

    describe('fetchDistrictById', () => {
        it('should return the district by given id', async () => {
            // arrange
            const districtId = 1;
            const mockResponse: District = {
                id: 1,
                name: 'District 1'
            };

            // mocking fetch
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: true,
                json: async () => mockResponse,
            }));

            // act
            const result = await fetchDistrictById(districtId);

            // assert
            expect(result).toEqual(mockResponse);
            expect(fetch).toHaveBeenCalledWith(
                `/api/District/${districtId}`
            );
        });

        it('should throw an error if the response is not ok', async () => {
            // arrange
            const districtId = 1;
            const mockErrorMessage = 'Error fetching district';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => mockErrorMessage,
            }));

            // expecting throwing an error
            await expect(fetchDistrictById(districtId)).rejects.toThrow(mockErrorMessage);

            // checking correct call
            expect(fetch).toHaveBeenCalledWith(
                `/api/District/${districtId}`
            );
        });
    });

    describe('fetchAllDistricts', () => {
        it('should return all districts', async () => {
            // arrange
            const mockResponse: District[] = [
                { id: 1, name: 'District 1' },
                { id: 2, name: 'District 2' }
            ];

            // mocking fetch
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: true,
                json: async () => mockResponse,
            }));

            // act
            const result = await fetchAllDistricts();

            // assert
            expect(result).toEqual(mockResponse);
            expect(fetch).toHaveBeenCalledWith(
                '/api/District/get-all'
            );
        });

        it('should throw an error if the response is not ok', async () => {
            // arrange
            const mockErrorMessage = 'Error fetching districts';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => mockErrorMessage,
            }));

            // expecting throwing an error
            await expect(fetchAllDistricts()).rejects.toThrow(mockErrorMessage);

            // checking correct call
            expect(fetch).toHaveBeenCalledWith(
                '/api/District/get-all'
            );
        });
    });
});