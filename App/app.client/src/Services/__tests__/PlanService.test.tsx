import { describe, it, expect, afterEach, vi } from 'vitest';
import { updatePlan, updatePlans, addRoute, removeRoute, addVehicle, removeVehicle, fetchGeneratedRoutePlan, Plan } from '../PlanService';





describe('PlanService', () => {
    afterEach(() => {
        vi.restoreAllMocks();
    });

    const mockPlan: Plan = {
        date: '2024-04-01',
        ranger: { id: 1, firstName: 'John', lastName: 'Doe', districtId: 1, email: 'john@doe.com' },
        routeIds: [1, 2],
        vehicleIds: [3]
    };

    const mockResult = { ...mockPlan };

    describe('updatePlan', () => {
        it('should successfully update a single plan by calling /api/Plan/update', async () => {

            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValue({ ok: true }));

            // act 
            await updatePlan(mockPlan);

            //asser
            expect(fetch).toHaveBeenCalledWith(
                '/api/Plan/update',
                expect.objectContaining({
                    method: 'POST',
                    headers: expect.objectContaining({
                        'Content-Type': 'application/json'
                    }),
                    body: JSON.stringify(mockPlan)
                })
            );
        });
        it('should throw an error if the response is not ok', async () => {
            // arrange
            const mockErrorMessage = 'District does not exist.';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => mockErrorMessage,
            }));

            // expecting throwing an error
            await expect(updatePlan(mockPlan)).rejects.toThrow(mockErrorMessage);

            // checking correct call
            expect(fetch).toHaveBeenCalledWith(
                '/api/Plan/update',
                expect.objectContaining({
                    method: 'POST',
                    headers: expect.objectContaining({
                        'Content-Type': 'application/json'
                    }),
                    body: JSON.stringify(mockPlan)
                })
            );
        });
    });

    describe('updatePlans', () => {
        it('should successfully update multiple plans', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValue({ ok: true }));

            // act 
            await updatePlans([mockPlan]);

            //assert
            expect(fetch).toHaveBeenCalledWith(
                '/api/Plan/updateAll',
                expect.objectContaining({
                    method: 'POST',
                    body: JSON.stringify([mockPlan])
                })
            );
        });
        it('should throw an error if the response is not ok', async () => {
            // arrange
            const mockErrorMessage = 'District does not exist.';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => mockErrorMessage,
            }));

            //act
            // expecting throwing an error
            await expect(updatePlans([mockPlan])).rejects.toThrow(mockErrorMessage);

            //assert
            expect(fetch).toHaveBeenCalledWith(
                '/api/Plan/updateAll',
                expect.objectContaining({
                    method: 'POST',
                    body: JSON.stringify([mockPlan])
                })
            );
        });
    });

    describe('addRoute', () => {
        it('should add a route to a plan', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValue({
                ok: true,
                json: async () => mockResult
            }));

            // act
            const result = await addRoute(mockPlan.date, mockPlan.ranger.id, 10);

            // assert
            expect(result).toEqual(mockResult);
            expect(fetch).toHaveBeenCalledWith(
                `/api/Plan/add-route/${mockPlan.date}/${mockPlan.ranger.id}?routeId=10`,
                { method: 'PUT' }
            );
        });

        it('should throw an error if the response is not ok', async () => {
            // arrange
            const mockErrorMessage = 'Route Does not exist.';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => mockErrorMessage,
            }));

            //act
            // expecting throwing an error
            await expect(addRoute(mockPlan.date, mockPlan.ranger.id, 10)).rejects.toThrow(mockErrorMessage);

            //assert
            expect(fetch).toHaveBeenCalledWith(
                `/api/Plan/add-route/${mockPlan.date}/${mockPlan.ranger.id}?routeId=10`,
                { method: 'PUT' }
            );
        });

    });

    describe('removeRoute', () => {
        it('should remove a route from a plan', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValue({
                ok: true,
                json: async () => mockResult
            }));

            // act
            const result = await removeRoute(mockPlan.date, mockPlan.ranger.id, 10);

            // assert
            expect(result).toEqual(mockResult);
            expect(fetch).toHaveBeenCalledWith(
                `/api/Plan/remove-route/${mockPlan.date}/${mockPlan.ranger.id}?routeId=10`,
                { method: 'PUT' }
            );
        });
        it('should throw an error if the response is not ok', async () => {
            // arrange
            const mockErrorMessage = 'Route Does not exist.';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => mockErrorMessage,
            }));

            // expecting throwing an error
            await expect(removeRoute(mockPlan.date, mockPlan.ranger.id, 10)).rejects.toThrow(mockErrorMessage);

            //assert
            expect(fetch).toHaveBeenCalledWith(
                `/api/Plan/remove-route/${mockPlan.date}/${mockPlan.ranger.id}?routeId=10`,
                { method: 'PUT' }
            );
        });
    });

    describe('addVehicle', () => {
        it('should add a vehicle to a plan', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValue({
                ok: true,
                json: async () => mockResult
            }));

            // act
            const result = await addVehicle(mockPlan.date, mockPlan.ranger.id, 10);

            // assert
            expect(result).toEqual(mockResult);
            expect(fetch).toHaveBeenCalledWith(
                `/api/Plan/add-vehicle/${mockPlan.date}/${mockPlan.ranger.id}?vehicleId=10`,
                { method: 'PUT' }
            );
        });
        it('should throw an error if the response is not ok', async () => {
            // arrange
            const mockErrorMessage = 'Vehicle Does not exist.';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => mockErrorMessage,
            }));

            // expecting throwing an error
            await expect(addVehicle(mockPlan.date, mockPlan.ranger.id, 10)).rejects.toThrow(mockErrorMessage);

            //assert
            expect(fetch).toHaveBeenCalledWith(
                `/api/Plan/add-vehicle/${mockPlan.date}/${mockPlan.ranger.id}?vehicleId=10`,
                { method: 'PUT' }
            );
        });
    });


    describe('removeVehicle', () => {
        it('should remove a vehicle from a plan', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValue({
                ok: true,
                json: async () => mockResult
            }));

            // act
            const result = await removeVehicle(mockPlan.date, mockPlan.ranger.id, 10);

            // assert
            expect(result).toEqual(mockResult);
            expect(fetch).toHaveBeenCalledWith(
                `/api/Plan/remove-vehicle/${mockPlan.date}/${mockPlan.ranger.id}?vehicleId=10`,
                { method: 'PUT' }
            );
        });
        it('should throw an error if the response is not ok', async () => {
            // arrange
            const mockErrorMessage = 'Vehicle Does not exist.';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => mockErrorMessage,
            }));

            // expecting throwing an error
            await expect(removeVehicle(mockPlan.date, mockPlan.ranger.id, 10)).rejects.toThrow(mockErrorMessage);

            //assert
            expect(fetch).toHaveBeenCalledWith(
                `/api/Plan/remove-vehicle/${mockPlan.date}/${mockPlan.ranger.id}?vehicleId=10`,
                { method: 'PUT' }
            );
        });
    });

    describe('fetchGeneratedRoutePlan', () => {
        it('should fetch generated route plan', async () => {
            // arrange
            const mockGenerate = {
                success: true,
                message: 'Generated successfully',
                plans: [mockPlan]
            };

            vi.stubGlobal('fetch', vi.fn().mockResolvedValue({
                ok: true,
                json: async () => mockGenerate
            }));

            // act
            const result = await fetchGeneratedRoutePlan(1, '2024-04-01');

            // assert
            expect(result).toEqual(mockGenerate);
            expect(fetch).toHaveBeenCalledWith(
                '/api/Plan/generate/1/2024-04-01'
            );
        });
        it('should throw an error if the response is not ok', async () => {
            // arrange
            const mockErrorMessage = 'Unexpected error.';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => mockErrorMessage,
            }));

            // expecting throwing an error
            await expect(fetchGeneratedRoutePlan(1, '2024-04-01')).rejects.toThrow(mockErrorMessage);

            //assert
            expect(fetch).toHaveBeenCalledWith(
                '/api/Plan/generate/1/2024-04-01'
            );
        });
    });
});