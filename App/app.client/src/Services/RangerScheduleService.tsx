import { ReasonOfAbsence } from "./AttendenceService";
import { Ranger } from "./RangerService";


const BASE_URL = '/api/Plan';
/** Structure that represents attendence and plan merged, makes it easier to work since they are connected and displayed together*/
export interface RangerSchedule {
    date: string;
    ranger: Ranger;
    working: boolean;
    from: string | null;
    reasonOfAbsence: ReasonOfAbsence;
    routeIds: number[];
    vehicleIds: number[];
}

/**
 * Get ranger schedules in a district between start and end date, inclusive range.
 * @param districtId Id of district.
 * @param start Date of start.
 * @param end Date of end.
 * @returns Array of ranger schedules.
 */
export const fetchRangerSchedulesByDateRange = async (districtId: number, start: string, end: string): Promise<RangerSchedule[]> => {
    const response = await fetch(`${BASE_URL}/by-dates/${districtId}/${start}/${end}`);
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message);
    }
    const result = await response.json();
    return result;
};