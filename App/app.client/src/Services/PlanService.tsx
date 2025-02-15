import { Ranger } from './RangerService';

const BASE_URL = '/api/Plan';


export interface Plan {
    date: string;
    ranger: Ranger;
    routeIds: number[];
    vehicleIds: number[];
};
/* unnecessary
export const fetchPlansByDate = async (districtId : number, date: string): Promise<Plan[]> => {
    const response = await fetch(`${BASE_URL}/${districtId}/${date}`);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
};

export const fetchPlansByDateRange = async (districtId: number, start: string, end: string): Promise<Plan[]> => {
    const response = await fetch(`${BASE_URL}/by-dates/${districtId}/${start}/${end}`);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
};
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
        throw new Error("Při updatování plánu nastala chyba.");
    }
}
export const addRoute = async (date: string, rangerId: number, routeId: number): Promise<Plan> => {
    const response = await fetch(`${BASE_URL}/add-route/${date}/${rangerId}?routeId=${routeId}`, {method: 'PUT'})
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
}

export const removeRoute = async (date: string, rangerId: number, routeId: number): Promise<Plan> => {
    const response = await fetch(`${BASE_URL}/remove-route/${date}/${rangerId}?routeId=${routeId}`, { method: 'PUT' })
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
}

export const addVehicle = async (date: string, rangerId: number, vehicleId: number): Promise<Plan> => {
    const response = await fetch(`${BASE_URL}/add-vehicle/${date}/${rangerId}?vehicleId=${vehicleId}`, { method: 'PUT' })
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
}

export const removeVehicle = async (date: string, rangerId: number, vehicleId: number): Promise<Plan> => {
    const response = await fetch(`${BASE_URL}/remove-vehicle/${date}/${rangerId}?vehicleId=${vehicleId}`, { method: 'PUT' })
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
}

