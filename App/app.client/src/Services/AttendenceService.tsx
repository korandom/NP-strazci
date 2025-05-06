import { Ranger } from './RangerService';

const BASE_URL = '/api/Attendence';

export enum ReasonOfAbsence {
    None = 0,
    NV = 1,
    D = 2,
    N = 3,
    PV = 4,
    P = 5
}

export interface Attendence {
    date: string;
    ranger: Ranger;
    working: boolean;
    from: string | null;
    reasonOfAbsence: ReasonOfAbsence;
}

/**
 * Update Attendence on the server. If Attendence does not exist, it creates it.
 * @param attendence Attendence being updated.
 * @returns Updated Attendence
 */
export const updateAttendence = async (attendence: Attendence): Promise<Attendence> => {
    const transformedAttendence = {
        ...attendence,
        reasonOfAbsence: Number(attendence.reasonOfAbsence) 
    };
    const response = await fetch(`${BASE_URL}/update`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(transformedAttendence)
    });
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
}