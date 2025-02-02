import { Ranger } from './RangerService';

//const BASE_URL = '/api/Attendence';

export enum ReasonOfAbsence {
    None,
    NV,
    D,
    S,
    ST,
    N,
    PV,
    P,
    X
}
export interface Attendence {
    date: string;
    ranger: Ranger;
    working: Boolean;
    from: string;
    reasonOfAbsence: ReasonOfAbsence;
};
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