const BASE_URL = '/api/Lock';

export interface Locked {
    date: string;
    districtId: number;
}

export const lockPlans = async (date: string, districtId: number) => {
    const response = await fetch(`${BASE_URL}/lock/${districtId}/${date}`, { method: 'POST' })

    if (!response.ok) {
        const message = await response.text();
        console.error(message);
        throw new Error(message);
    }
}

export const unlockPlans = async (date: string, districtId: number) => {
    const response = await fetch(`${BASE_URL}/unlock/${districtId}/${date}`, { method: 'DELETE' })

    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
}

export const fetchLocks = async (districtId: number): Promise<Locked[]> => {
    const response = await fetch(`${BASE_URL}/locks/${districtId}`);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
};