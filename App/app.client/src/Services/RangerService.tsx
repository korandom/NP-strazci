
const BASE_URL = '/api/Ranger';
export interface Ranger {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    districtId: number;
}
/**
 * Gets all rangers in district.
 * @param districtId Id of the district.
 * @returns An Array of rangers in district.
 */
export const fetchRangersByDistrict = async (districtId : number): Promise<Ranger[]> => {
    const response = await fetch(`${BASE_URL}/in-district/${districtId}`);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
};

/**
 * Get ranger that is assigned to currently signed in user.
 * @returns Ranger or null, if user has no assigned ranger.
 */
export const getCurrentRanger = async (): Promise<Ranger | undefined> => {
    const response = await fetch(`${BASE_URL}`);
    if (!response.ok) {
        // log message
        return undefined;
    }
    const result = await response.json();
    return result;
}

/**
 * Update information of ranger, ranger must already be created.
 * @param ranger Ranger being updated.
 */
export const updateRanger = async (ranger: Ranger) => {
    const response = await fetch(`${BASE_URL}/update`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(ranger)
    });
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
}

/**
 * Delete ranger, is irreversibly deleted from plans and everything.
 * @param ranger Ranger being deleted.
 */
export const deleteRanger = async (ranger: Ranger) => {
    const response = await fetch(`${BASE_URL}/delete/${ranger.id}`, {method:'DELETE'});
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
}

/**
 * Create new ranger.
 * @param ranger Ranger being created, Id of ranger can be 0
 * @returns Created ranger.
 */
export const createRanger = async (ranger: Ranger): Promise<Ranger> => {
    const response = await fetch(`${BASE_URL}/create`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(ranger)
    });
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
}