const BASE_URL = '/api/Lock';

export interface Locked {
    date: string;
    districtId: number;
}

/**
 * Lock plans in district for a certain date.
 * @param date Date of plans to be locked.
 * @param districtId Id of district, where plans should be locked.
 */
export const lockPlans = async (date: string, districtId: number) => {
    const response = await fetch(`${BASE_URL}/lock/${districtId}/${date}`, { method: 'POST' })

    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
}

/**
 * Únlock plans in district for a certain date.
 * @param date Date of plans to be unlocked.
 * @param districtId Id of district, where plans should be unlocked.
 */
export const unlockPlans = async (date: string, districtId: number) => {
    const response = await fetch(`${BASE_URL}/unlock/${districtId}/${date}`, { method: 'DELETE' })

    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
}

/**
 * Get all locks for a specific district.
 * @param districtId Id of district.
 * @returns A list of Locks in that district.
 */
export const fetchLocks = async (districtId: number): Promise<Locked[]> => {
    const response = await fetch(`${BASE_URL}/locks/${districtId}`);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
};