
const BASE_URL = '/api/Route';

type controlPlace = {
    controlTime: string;
    controlPlaceDescription: string;
}
export interface Route {
    id: number;
    name: string;
    priority: number;
    controlPlace: controlPlace | undefined;
    districtId: number;
}

/**
 * Get all routes in district.
 * @param districtId Id of district.
 * @returns An array of routes.
 */
export const fetchRoutesByDistrict = async (districtId: number): Promise<Route[]> => {
    const response = await fetch(`${BASE_URL}/in-district/${districtId}`);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const routes = await response.json();
    return routes;
}

/**
 * Update route information, route must be already created.
 * @param route Route being updated.
 */
export const updateRoute = async (route: Route) => {
    const response = await fetch(`${BASE_URL}/update`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(route)
    });
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
}

/**
 * Delete route, is irreversible.
 * @param route Route being deleted.
 */
export const deleteRoute = async (route: Route) => {
    const response = await fetch(`${BASE_URL}/delete/${route.id}`, {method: 'DELETE'});
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
}
/**
 * Create new route, route Id can be 0.
 * @param route Route being created.
 * @returns New created route.
 */
export const createRoute = async (route: Route) : Promise<Route> => {
    const response = await fetch(`${BASE_URL}/create`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(route)
    });
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
}