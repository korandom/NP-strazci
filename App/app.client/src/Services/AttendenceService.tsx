import { Ranger } from './RangerService';

const BASE_URL = '/api/Attendence';

export enum ReasonOfAbsence {
    None = 0,
    NV = 1,
    D = 2,
    S = 3,
    ST = 4,
    N = 5,
    PV = 6,
    P = 7,
    X = 8
}

export interface Attendence {
    date: string;
    ranger: Ranger;
    working: Boolean;
    from: string | null;
    reasonOfAbsence: ReasonOfAbsence;
};

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
/* unnecessary
export const fetchAttendenceByDateRange = async (districtId: number, start: string, end: string): Promise<Attendence[]> => {
    const response = await fetch(`${BASE_URL}/by-dates/${districtId}/${start}/${end}`);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
};*/