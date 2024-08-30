
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
export const fetchRoutesByDistrict = async (districtId: number): Promise<Route[]> => {
    const response = await fetch(`${BASE_URL}/in-district/${districtId}`);
    if (!response.ok) {
        throw new Error('Failed to fetch routes');
    }
    const routes = await response.json();
    return routes;
}

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
    console.error("updated");
}

export const deleteRoute = async (route: Route) => {
    const response = await fetch(`${BASE_URL}/delete/${route.id}`);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
}