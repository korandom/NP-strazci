import { vi, describe, it, expect, afterEach } from 'vitest';
import { fetchVehiclesByDistrict, updateVehicle, deleteVehicle, createVehicle, Vehicle } from '../VehicleService';

describe('VehicleService', () => {
    afterEach(() => {
        vi.restoreAllMocks();
    });

    const mockVehicle: Vehicle = {
        id: 1,
        type: 'auto',
        name: 'skoda2',
        districtId: 1
    };

    describe('fetchVehiclesByDistrict', () => {
        it('should return all vehicles in given district', async () => {
            // arrange
            const mockResponse = [mockVehicle];
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: true,
                json: async () => mockResponse
            }));

            // act
            const result = await fetchVehiclesByDistrict(mockVehicle.districtId);

            // assert
            expect(result).toEqual(mockResponse);
            expect(fetch).toHaveBeenCalledWith(`/api/Vehicle/in-district/${mockVehicle.districtId}`);
        });

        it('should throw an error if response is not ok', async () => {
            // arrange
            const errorMessage = "District id not found."
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => errorMessage
            }));

            // act + assert
            await expect(fetchVehiclesByDistrict(mockVehicle.districtId)).rejects.toThrow(errorMessage);
        });
    });

    describe('updateVehicle', () => {
        it('should update a vehicle', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({ ok: true }));

            // act
            await updateVehicle(mockVehicle);

            // assert
            expect(fetch).toHaveBeenCalledWith(
                `/api/Vehicle/update`,
                expect.objectContaining({
                    method: 'PUT',
                    headers: expect.objectContaining({
                        'Content-Type': 'application/json'
                    }),
                    body: JSON.stringify(mockVehicle)
                })
            );
        });

        it('should throw an error if response is not ok', async () => {
            // arrange
            const errorMessage = 'Failed to update vehicle';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => errorMessage
            }));

            // act + assert
            await expect(updateVehicle(mockVehicle)).rejects.toThrow(errorMessage);
            expect(fetch).toHaveBeenCalledWith(
                `/api/Vehicle/update`,
                expect.objectContaining({
                    method: 'PUT',
                    headers: expect.objectContaining({
                        'Content-Type': 'application/json'
                    }),
                    body: JSON.stringify(mockVehicle)
                })
            );
        });
    });

    describe('deleteVehicle', () => {
        it('should delete a vehicle', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({ ok: true }));

            // act
            await deleteVehicle(mockVehicle);

            // assert
            expect(fetch).toHaveBeenCalledWith(
                `/api/Vehicle/delete/${mockVehicle.id}`,
                { method: 'DELETE' }
            );
        });

        it('should throw an error if response is not ok', async () => {
            // arrange
            const errorMessage = 'Vehicle not found';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => errorMessage
            }));

            // act + assert
            await expect(deleteVehicle(mockVehicle)).rejects.toThrow(errorMessage);
            expect(fetch).toHaveBeenCalledWith(
                `/api/Vehicle/delete/${mockVehicle.id}`,
                { method: 'DELETE' }
            );
        });
    });

    describe('createVehicle', () => {
        it('should create a vehicle and return it if response is ok', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: true,
                json: async () => mockVehicle
            }));

            // act
            const result = await createVehicle(mockVehicle);

            // assert
            expect(result).toEqual(mockVehicle);
            expect(fetch).toHaveBeenCalledWith(
                `/api/Vehicle/create`,
                expect.objectContaining({
                    method: 'POST',
                    headers: expect.objectContaining({
                        'Content-Type': 'application/json'
                    }),
                    body: JSON.stringify(mockVehicle)
                })
            );
        });

        it('should throw an error if response is not ok', async () => {
            // arrange
            const errorMessage = 'Failed to create vehicle';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => errorMessage
            }));

            // act + assert
            await expect(createVehicle(mockVehicle)).rejects.toThrow(errorMessage);
        });
    });
});