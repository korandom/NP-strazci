import { describe, it, expect, afterEach, vi } from 'vitest';
import { fetchRoutesByDistrict, updateRoute, deleteRoute, createRoute,  Route} from '../RouteService';

describe('RouteService', () => {
    afterEach(() => {
        vi.restoreAllMocks();
    });

    const mockRoute: Route = {
        id: 1,
        name: 'route',
        priority: 2,
        controlPlace: {
            controlTime: '08:00',
            controlPlaceDescription: 'description'
        },
        districtId: 1
    };

    describe('fetchRoutesByDistrict', () => {
        it('should return all routes in the given district', async () => {
            // arrange
            const mockResponse = [mockRoute];
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: true,
                json: async () => mockResponse
            }));

            // act
            const result = await fetchRoutesByDistrict(mockRoute.districtId);

            // assert
            expect(result).toEqual(mockResponse);
            expect(fetch).toHaveBeenCalledWith(`/api/Route/in-district/${mockRoute.districtId}`);
        });

        it('should throw an error if response is not ok', async () => {
            // arrange
            const errorMessage = 'District not found';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => errorMessage
            }));

            // act + assert
            await expect(fetchRoutesByDistrict(mockRoute.districtId)).rejects.toThrow(errorMessage);
        });
    });

    describe('updateRoute', () => {
        it('should update route', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({ ok: true }));

            // act
            await updateRoute(mockRoute);

            // assert
            expect(fetch).toHaveBeenCalledWith(
                `/api/Route/update`,
                expect.objectContaining({
                    method: 'PUT',
                    headers: expect.objectContaining({
                        'Content-Type': 'application/json'
                    }),
                    body: JSON.stringify(mockRoute)
                })
            );
        });

        it('should throw an error if response is not ok', async () => {
            // arrange
            const errorMessage = 'Failed to update route';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => errorMessage
            }));

            // act + assert
            await expect(updateRoute(mockRoute)).rejects.toThrow(errorMessage);
        });
    });

    describe('deleteRoute', () => {
        it('should delete a route', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({ ok: true }));

            // act
            await deleteRoute(mockRoute);

            // assert
            expect(fetch).toHaveBeenCalledWith(`/api/Route/delete/${mockRoute.id}`, { method: 'DELETE' });
        });

        it('should throw an error if response is not ok', async () => {
            // arrange
            const errorMessage = 'Route not found';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => errorMessage
            }));

            // act + assert
            await expect(deleteRoute(mockRoute)).rejects.toThrow(errorMessage);
        });
    });

    describe('createRoute', () => {
        it('should create a route and return it', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: true,
                json: async () => mockRoute
            }));

            // act
            const result = await createRoute(mockRoute);

            // assert
            expect(result).toEqual(mockRoute);
            expect(fetch).toHaveBeenCalledWith(
                `/api/Route/create`,
                expect.objectContaining({
                    method: 'POST',
                    headers: expect.objectContaining({
                        'Content-Type': 'application/json'
                    }),
                    body: JSON.stringify(mockRoute)
                })
            );
        });

        it('should throw an error if response is not ok', async () => {
            // arrange
            const errorMessage = 'Failed to create route';

            //mock fetch failure
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => errorMessage
            }));

            // act + assert
            await expect(createRoute(mockRoute)).rejects.toThrow(errorMessage);
        });
    });
});