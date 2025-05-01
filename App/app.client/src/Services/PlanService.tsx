import { formatDate, getShiftedDate } from '../Util/DateUtil';
import { Ranger } from './RangerService';
import { Route } from './RouteService';

const BASE_URL = '/api/Plan';

/**
 * Interface representing a plan of a specific ranger for a day, containing routes and vehicles.
 */
export interface Plan {
    date: string;
    ranger: Ranger;
    routeIds: number[];
    vehicleIds: number[];
}

export interface GenerateResult {
    success: boolean;
    message: string;
    plans: Plan[];
}

/**
 * Update plan on the server.
 * @param plan Plan being updated.
 */
export const updatePlan = async (plan: Plan) => {
    const response = await fetch(`${BASE_URL}/update`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(plan)
    });
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
}

/**
 * Update multiple plans on the server.
 * @param plans Plans being updated.
 */
export const updatePlans = async (plans: Plan[]) => {
    const response = await fetch(`${BASE_URL}/update-all`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(plans)
    });
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
}

/**
 * Add route to a plan of a ranger for a day on the backend.
 * @param date Date of the plan.
 * @param rangerId Id of Ranger, whoms is the route being assigned to.
 * @param routeId Id of Route being added.
 * @returns Updated plan of the ranger.
 */
export const addRoute = async (date: string, rangerId: number, routeId: number): Promise<Plan> => {
    const response = await fetch(`${BASE_URL}/add-route/${date}/${rangerId}?routeId=${routeId}`, {method: 'PUT'})
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
}

/**
 * Remove route from a plan of a ranger on a specific day on the backend.
 * @param date Date of the plan.
 * @param rangerId Id of Ranger, whose plan the route is being removed from.
 * @param routeId Id of Route being removed.
 * @returns Updated plan of the ranger.
 */
export const removeRoute = async (date: string, rangerId: number, routeId: number): Promise<Plan> => {
    const response = await fetch(`${BASE_URL}/remove-route/${date}/${rangerId}?routeId=${routeId}`, { method: 'PUT' })
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
}

/**
 * Add vehicle to a plan of a ranger for a day on the backend.
 * @param date Date of the plan.
 * @param rangerId Id of Ranger, whoms is the vehicle being assigned to.
 * @param routeId Id of Vehicle being added.
 * @returns Updated plan of the ranger.
 */
export const addVehicle = async (date: string, rangerId: number, vehicleId: number): Promise<Plan> => {
    const response = await fetch(`${BASE_URL}/add-vehicle/${date}/${rangerId}?vehicleId=${vehicleId}`, { method: 'PUT' })
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
}

/**
 * Remove vehicle from a plan of a ranger on a specific day on the backend.
 * @param date Date of the plan.
 * @param rangerId Id of Ranger, whose plan the vehicle is being removed from.
 * @param routeId Id of Vehicle being removed.
 * @returns Updated plan of the ranger.
 */
export const removeVehicle = async (date: string, rangerId: number, vehicleId: number): Promise<Plan> => {
    const response = await fetch(`${BASE_URL}/remove-vehicle/${date}/${rangerId}?vehicleId=${vehicleId}`, { method: 'PUT' })
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
}

/**
 * Mock generating route plans
 * @param startDate date of the start period
 * @returns fake plans.
 */
export const generateRoutePlanMock = async (startDate: string, rangers: Ranger[], routes: Route[]): Promise<Plan[]> => {
    //wait 30 seconds
    const wait = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));
    await wait(30000);

    const fakePlans: Plan [] = [
        {
            date: startDate,
            ranger: rangers[1],
            routeIds: [routes[1].id, routes[2].id],
            vehicleIds: []
        },
        {
            date: formatDate(getShiftedDate(new Date(startDate), 4)),
            ranger: rangers[2],
            routeIds: [routes[0].id],
            vehicleIds: []
        },
        {
            date: startDate,
            ranger: rangers[3],
            routeIds: [routes[3].id],
            vehicleIds: []
        }
    ]
    return fakePlans;
}

/**
 * Generate a week long route plan from district with id districtId that starts at date start.
 * @param districtId Id of district.
 * @param start Start of the week.
 * @returns A Generate result with generated plans if succesfull.
 */
export const fetchGeneratedRoutePlan = async (districtId: number, start: string): Promise<GenerateResult> => {
    const response = await fetch(`${BASE_URL}/generate/${districtId}/${start}`);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
};

