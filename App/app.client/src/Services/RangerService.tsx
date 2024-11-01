
const BASE_URL = '/api/Ranger';
export interface Ranger {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    districtId: number;
}

export const fetchRangersByDistrict = async (districtId : number): Promise<Ranger[]> => {
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

export const deleteRanger = async (ranger: Ranger) => {
    const response = await fetch(`${BASE_URL}/delete/${ranger.id}`, {method:'DELETE'});
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
}

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