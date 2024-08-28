const BASE_URL = '/api/District';

export interface District {
    id: number,
    name: string
}

export const fetchDistrictById = async (districtId: number): Promise<District> => {
    const response = await fetch(`${BASE_URL}/${districtId}`);
    if (!response.ok) {
        throw new Error('Failed to fetch district');
    }
    const result = await response.json();
    return result;
}

export const fetchAllDistricts = async (): Promise<District[]> => {
    const response = await fetch(`${BASE_URL}/get-all`);
    if (!response.ok) {
        throw new Error('Failed to fetch districts');
    }
    const result = await response.json();
    return result;
}