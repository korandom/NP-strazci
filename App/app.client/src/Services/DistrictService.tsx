const BASE_URL = '/api/District';

export interface District {
    districtId: number,
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