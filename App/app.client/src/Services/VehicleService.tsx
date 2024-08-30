
const BASE_URL = '/api/Vehicle';
export interface Vehicle {
    id: number;
    type: string;
    name: string;
    districtId: number;
}

export const fetchVehiclesByDistrict = async (districtId: number): Promise<Vehicle[]> => {
    const response = await fetch(`${BASE_URL}/in-district/${districtId}`);
    if (!response.ok) {
        throw new Error('Failed to fetch routes');
    }
    const routes = await response.json();
    return routes;
}

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

export const deleteVehicle = async (vehicle: Vehicle) => {
    const response = await fetch(`${BASE_URL}/delete/${vehicle.id}`);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
}