import { ReasonOfAbsence } from "./AttendenceService";
import { Ranger } from "./RangerService";


const BASE_URL = '/api/Plan';
export interface RangerSchedule {
    date: string;
    ranger: Ranger;
    working: boolean;
    from: string | null;
    reasonOfAbsence: ReasonOfAbsence;
    routeIds: number[];
    vehicleIds: number[];
}

export const fetchRangerSchedulesByDateRange = async (districtId: number, start: string, end: string): Promise<RangerSchedule[]> => {
    const response = await fetch(`${BASE_URL}/by-dates/${districtId}/${start}/${end}`);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
};