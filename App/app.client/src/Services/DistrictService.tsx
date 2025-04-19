const BASE_URL = '/api/District';

export interface District {
    id: number,
    name: string
}

/**
 * Get District by Id.
 * @param districtId Id of district.
 * @returns Promise of a District
 */
export const fetchDistrictById = async (districtId: number): Promise<District> => {
    const response = await fetch(`${BASE_URL}/${districtId}`);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
}

/**
 * Get all districts. 
 * @returns A list of all Districts
 */
export const fetchAllDistricts = async (): Promise<District[]> => {
    const response = await fetch(`${BASE_URL}/get-all`);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
}