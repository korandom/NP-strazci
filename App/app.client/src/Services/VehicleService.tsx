
const BASE_URL = '/api/Vehicle';
export interface Vehicle {
    id: number;
    type: string;
    name: string;
    districtId: number;
}

/**
 * Get all vehicles in district.
 * @param districtId Id of district.
 * @returns A promise of Array of vehicles.
 */
export const fetchVehiclesByDistrict = async (districtId: number): Promise<Vehicle[]> => {
    const response = await fetch(`${BASE_URL}/in-district/${districtId}`);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const routes = await response.json();
    return routes;
}

/**
 * Update a vehicle, it must be already created.
 * @param vehicle Updated vehicle.
 */
export const updateVehicle = async (vehicle: Vehicle) => {
    const response = await fetch(`${BASE_URL}/update`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(vehicle)
    });
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
}

/**
 * Delete vehicle, irreversibly.
 * @param vehicle Vehicle being deleted.
 */
export const deleteVehicle = async (vehicle: Vehicle) => {
    const response = await fetch(`${BASE_URL}/delete/${vehicle.id}`, {method:'DELETE'});
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
}

/**
 * Create new vehicle, id can be 0.
 * @param vehicle Vehicle being created.
 * @returns Newly created vehicle.
 */
export const createVehicle = async (vehicle: Vehicle): Promise<Vehicle> => {
    const response = await fetch(`${BASE_URL}/create`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(vehicle)
    });
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
}