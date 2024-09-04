import { Route } from './RouteService';
import { Ranger } from './RangerService';
import { Vehicle } from './VehicleService';

const BASE_URL = '/api/Plan';


export interface Plan {
    date: string;
    ranger: Ranger;
    routes: Route[];
    vehicles: Vehicle[];
};

export interface Locked {
    date: string;
    districtId: number;
}

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
    const response = await fetch(`${BASE_URL}/add-route/${date}/${rangerId}?routeId=${routeId}`, {method: 'PUT'})
    if (!response.ok) {
        throw new Error('Failed to add route to plan');
    }
}

export const removeRoute = async (date: string, rangerId: number, routeId: number) => {
    const response = await fetch(`${BASE_URL}/remove-route/${date}/${rangerId}?routeId=${routeId}`, { method: 'PUT' })
    if (!response.ok) {
        throw new Error('Failed to remove route to plan');
    }
}

export const addVehicle = async (date: string, rangerId: number, vehicleId: number) => {
    const response = await fetch(`${BASE_URL}/add-vehicle/${date}/${rangerId}?vehicleId=${vehicleId}`, { method: 'PUT' })
    if (!response.ok) {
        throw new Error('Failed to add vehicle to plan');
    }
}

export const removeVehicle = async (date: string, rangerId: number, vehicleId: number) => {
    const response = await fetch(`${BASE_URL}/remove-vehicle/${date}/${rangerId}?vehicleId=${vehicleId}`, { method: 'PUT' })
    if (!response.ok) {
        throw new Error('Failed to remove vehicle to plan');
    }
}

export const lockPlans = async (date: string, districtId: number) => {
    const response = await fetch(`${BASE_URL}/lock/${districtId}/${date}`, { method: 'POST' })

    if (!response.ok) {
        const message = await response.text();
        console.error(message);
        throw new Error(message);
    }
}

export const unlockPlans = async (date: string, districtId: number) => {
    const response = await fetch(`${BASE_URL}/unlock/${districtId}/${date}`, { method: 'DELETE' })

    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
}

export const fetchLocks = async (districtId: number): Promise<Locked[]> => {
    const response = await fetch(`${BASE_URL}/locks/${districtId}`);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
};

