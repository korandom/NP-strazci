
const BASE_URL = '/api/Ranger';
export interface Ranger {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    districtId: number;
}

export const fetchRangersByDistrict = async (districtId : string): Promise<Ranger[]> => {
    const response = await fetch(`${BASE_URL}/in-district/${districtId}`);
    if (!response.ok) {
        throw new Error('Failed to fetch rangers');
    }
    const result = await response.json();
    return result;
};

export const getCurrentRanger = async (): Promise<Ranger | undefined> => {
    const response = await fetch(`${BASE_URL}`);
    if (!response.ok) {
        //const errorMessage = await response.text();
        // log message
        return undefined;
    }
    const result = await response.json();
    return result;
}