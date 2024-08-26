import { Route } from './RouteService';
import { Ranger } from './RangerService';
import { Vehicle } from './VehicleService';

const BASE_URL = '/api/Plan';


export interface Plan {
    date: string;
    ranger: Ranger;
    routes: Route[];
    vehicles: Vehicle[];
    locked: boolean;
};

export const fetchPlansByDate = async (districtId : number, date: string): Promise<Plan[]> => {
    const response = await fetch(`${BASE_URL}/${districtId}/${date}`);
    if (!response.ok) {
        throw new Error('Failed to fetch plans');
    }
    const result = await response.json();
    return result;
};

export const fetchPlansByDateRange = async (districtId: number, start: string, end: string): Promise<Plan[]> => {
    const response = await fetch(`${BASE_URL}/by-dates/${districtId}/${start}/${end}`);
    if (!response.ok) {
        throw new Error('Failed to fetch plans');
    }
    const result = await response.json();
    return result;
};

export const addRoute = async (date: string, rangerId: number, routeId: number) => {
    const response = await fetch(`${BASE_URL}/add-route/${date}/${rangerId.toString()}?routeId=${routeId.toString()}`, {method: 'PUT'})
    if (!response.ok) {
        throw new Error('Failed to add route to plan');
    }
}

export const removeRoute = async (date: string, rangerId: number, routeId: number) => {
    const response = await fetch(`${BASE_URL}/remove-route/${date}/${rangerId.toString()}?routeId=${routeId.toString()}`, { method: 'PUT' })
    if (!response.ok) {
        throw new Error('Failed to remove route to plan');
    }
}

export const addVehicle = async (date: string, rangerId: number, vehicleId: number) => {
    const response = await fetch(`${BASE_URL}/add-vehicle/${date}/${rangerId.toString()}?vehicleId=${vehicleId.toString()}`, { method: 'PUT' })
    if (!response.ok) {
        throw new Error('Failed to add vehicle to plan');
    }
}

export const removeVehicle = async (date: string, rangerId: number, vehicleId: number) => {
    const response = await fetch(`${BASE_URL}/remove-vehicle/${date}/${rangerId.toString()}?vehicleId=${vehicleId.toString()}`, { method: 'PUT' })
    if (!response.ok) {
        throw new Error('Failed to remove vehicle to plan');
    }
}