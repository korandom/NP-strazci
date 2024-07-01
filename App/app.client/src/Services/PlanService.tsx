import { Route } from './RouteService';
import { Ranger } from './RangerService';
import { Vehicle } from './VehicleService';

const BASE_URL = '/api/Plan';


export interface Plan {
    date: Date;
    ranger: Ranger;
    routes: Route[];
    vehicles: Vehicle[];
    locked: boolean;
};

export const fetchPlansByDate = async (date: string): Promise<Plan[]> => {
    const response = await fetch(`${BASE_URL}/${date}`);
    if (!response.ok) {
        throw new Error('Failed to fetch plans');
    }
    const result = await response.json();
    return result;
};