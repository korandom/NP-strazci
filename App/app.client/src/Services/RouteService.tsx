
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

class RouteService {
    private routes: Route[] = [];

    async fetchRoutesByDistrict(districtId: string): Promise<Route[]> {
        const response = await fetch(`${BASE_URL}/in-district/${districtId}`);
        if (!response.ok) {
            throw new Error('Failed to fetch routes');
        }
        const routes = await response.json();
        this.routes = routes;
        return routes;
    }

    getRoutes(): Route[] {
        if (this.routes.length == 0)
            this.fetchRoutesByDistrict("1");
        return this.routes;
    }
}

const routeService = new RouteService();
export default routeService;