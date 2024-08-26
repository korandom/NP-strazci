
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
