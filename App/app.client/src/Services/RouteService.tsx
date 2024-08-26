
const BASE_URL = '/api/Route';

type controlPlace = {
    controlTime: string;
    controlPlaceDescription: string;
}
export interface Route {
    id: number;
    name: string;
    priority: number;
    controlPlace: controlPlace;
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