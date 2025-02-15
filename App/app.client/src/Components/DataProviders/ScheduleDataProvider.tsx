import { createContext, useContext, ReactNode, useState, useMemo, useEffect } from 'react';
import {  Plan, addRoute, addVehicle, removeRoute, removeVehicle, updatePlan } from '../../Services/PlanService';
import useDistrict from './DistrictDataProvider';
import useAuth from '../Authentication/AuthProvider';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { calculateTwoWeeksRange, formatDate, getShiftedDate } from '../../Util/DateUtil';
import { RangerSchedule, fetchRangerSchedulesByDateRange } from '../../Services/RangerScheduleService';
import { Attendence, ReasonOfAbsence, updateAttendence } from '../../Services/AttendenceService';
import { Ranger } from '../../Services/RangerService';


interface ScheduleContextType {
    schedules: RangerSchedule[],
    dateRange: { start: Date, end: Date },
    resetSchedules: () => void,
    weekForward: () => void,
    weekBack: () => void,
    changeDate: (date: Date) => void,

    //attendence updating
    updateWorking: (date: string, ranger: Ranger, working: boolean) => void,
    updateAttendenceFrom: (date: string, ranger: Ranger, from: string) => void,
    updateReasonOfAbsence: (date: string, ranger: Ranger, reasonOfAbsence: ReasonOfAbsence) => void,
    
    //plan updating
    addPlannedVehicle: (date: string, rangerId: number, vehicleId: number) => void,
    removePlannedVehicle: (date: string, rangerId: number, vehicleId: number) => void,
    addPlannedRoute: (date: string, rangerId: number, routeId: number) => void,
    removePlannedRoute: (date: string, rangerId: number, routeId: number) => void,

    loading: boolean,
    error: any,
}

const ScheduleContext = createContext<ScheduleContextType>({} as ScheduleContextType);

/**
 * SchedulesProvider manages the state and logic for ranger Schedules in the application in a centralized way.
 * It provides the context for making changes to plans and attendences and  fetching ranger schedules for viewing. 
 * It receives updates of plans and attendences via HubConnection.
 *
 * @param children - The child components that will have access to the schedule context.
 * @returns A JSX.Element that provides the context to its children.
 * 
 * Automatically fetches new schedules when the district or user changes.
 * Manages weekly navigation (forward/backward) and resets of schedules.
 * Supports operations for adding/removing vehicles and routes from schedules.
 */
export const SchedulesProvider = ({ children }: { children: ReactNode }): JSX.Element => {
    const { district } = useDistrict();
    const { user } = useAuth();
    const [schedules, setSchedules] = useState<RangerSchedule[]>([]);
    const [firstWeek, setFirstWeek] = useState<RangerSchedule[]>([]);
    const [SecondWeek, setSecondWeek] = useState<RangerSchedule[]>([]);
    const [dateRange, setDateRange] = useState<{ start: Date, end: Date }>(calculateTwoWeeksRange(new Date()));
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<any>();
    const [hubConnection, setHubConnection] = useState<HubConnection>();

    useEffect(() => {
        const connect = async () => {
            if (!district) return;
            const connection = new HubConnectionBuilder()
                .withUrl('/rangerScheduleHub')
                .configureLogging(LogLevel.Information)
                .build();

            connection.on('PlanUpdated', (plan: Plan) => {
                setSchedules(prevSchedules => updatePlanInSchedule(prevSchedules, plan));
            });

            connection.on('AttendenceUpdated', (attendence: Attendence) => {
                setSchedules(prevSchedules => updateAttendenceInSchedule(prevSchedules, attendence));
            })

            await connection.start()
                .catch(setError);

            if (district) {
                await connection.invoke('AddToPlanGroup', district.id);
            }

            setHubConnection(connection);
            return connection;
        };

        connect().catch(setError);

        return () => {
            if (hubConnection) {
                hubConnection.stop();
            }
        };
    }, [district]);

    // fetch schedules by date range 
    const fetchSchedules = (start: Date, end: Date) : Promise<RangerSchedule[]> => {
        if (!district) {
            throw new Error("Není vybrán žádný obvod.");
        }
        const startDate = formatDate(start);
        const endDate = formatDate(end);
        const fetchedSchedules = fetchRangerSchedulesByDateRange(district.id, startDate, endDate);
        return fetchedSchedules;
    };

    // reset schedules when district or user changes
    useEffect(() => {
        resetSchedules();
    }, [district, user]);

    // reset schedules, fetch new schedules only if user is authorized
    const resetSchedules = () => {
        changeDate(new Date());
    };

    const changeDate= (date: Date) => {
        if (!user) {
            setSchedules([]);
        }
        else {
            setLoading(true);
            const range = calculateTwoWeeksRange(date);
            setDateRange(range);
            initializeSchedules(range.start, range.end);
            setLoading(false);
        }
    };

    // fetches and sets new schedules according to the range
    // start must be monday of one week, end is sunday of second week
    const initializeSchedules = async (start: Date, end: Date) => {
        try {
            const endFirstWeek = getShiftedDate(start, 6);
            const startSecondWeek = getShiftedDate(end, -6);

            const [firstWeekSchedules, secondWeekSchedules] = await Promise.all([
                fetchSchedules(start, endFirstWeek),
                fetchSchedules(startSecondWeek, end),
            ]);

            setFirstWeek(firstWeekSchedules);
            setSecondWeek(secondWeekSchedules);
            setSchedules([...firstWeekSchedules, ...secondWeekSchedules]);
            setError(null);
        }
        catch (err: any) {
            setError(err);
        }
    }

    // move schedules range a week forward and update schedules
    const weekForward = async () => {
        try {
            setLoading(true);

            const nextWeekStart = getShiftedDate(dateRange.end, 1);
            const nextWeekEnd = getShiftedDate(dateRange.end, 7);
            const newSecondWeekSchedules = await fetchSchedules(nextWeekStart, nextWeekEnd);

            setFirstWeek(SecondWeek);
            setSecondWeek(newSecondWeekSchedules);
            setSchedules([...SecondWeek, ...newSecondWeekSchedules]);
            setDateRange({start: getShiftedDate(nextWeekStart, -7),end: nextWeekEnd})
            setError(null);
        }
        catch (err: any) {
            setError(err);
        }
        finally {
            setLoading(false);
        }
    }

    // move schedules range a week back and update schedules
    const weekBack = async() => {
        try {
            setLoading(true);

            const previousWeekStart = getShiftedDate(dateRange.start, -7);
            const previousWeekEnd = getShiftedDate(dateRange.start, -1);
            const newFirstWeekSchedules = await fetchSchedules(previousWeekStart, previousWeekEnd);

            setFirstWeek(newFirstWeekSchedules);
            setSecondWeek(firstWeek);
            setSchedules([...newFirstWeekSchedules, ...firstWeek]);
            setDateRange({ start: previousWeekStart, end: getShiftedDate(previousWeekEnd, 7) })
            setError(null);
        }
        catch (err: any) {
            setError(err);
        }
        finally {
            setLoading(false);
        }
    }

    // when plan updates - it is updated in the schedule
    const updatePlanInSchedule = (prevSchedules: RangerSchedule[], updatedPlan: Plan): RangerSchedule[] => {
        const planIndex = prevSchedules.findIndex(plan => plan.date === updatedPlan.date && plan.ranger.id === updatedPlan.ranger.id);

        if (planIndex !== -1) {
            // plan already exists - it ideally should even if empty
            const updatedSchedules = [...prevSchedules];
            updatedSchedules[planIndex].routeIds = updatedPlan.routeIds;
            updatedSchedules[planIndex].vehicleIds = updatedPlan.vehicleIds;
            return updatedSchedules;
        } else {
            // add new schedule
            return [...prevSchedules, { date: updatedPlan.date, ranger: updatedPlan.ranger, routeIds: updatedPlan.routeIds, vehicleIds: updatedPlan.vehicleIds, reasonOfAbsence: ReasonOfAbsence.None, working: true, from: null}];
        }
    };

    // when attendence is updated, update it in schedule structure
    const updateAttendenceInSchedule = (prevSchedules: RangerSchedule[], updatedAttendence: Attendence): RangerSchedule[] => {
        const attendenceIndex = prevSchedules.findIndex(attendence => attendence.date === updatedAttendence.date && attendence.ranger.id === updatedAttendence.ranger.id);

        if (attendenceIndex !== -1) {
            // attendence already exists
            const updatedSchedules = [...prevSchedules];
            updatedSchedules[attendenceIndex].working = updatedAttendence.working;
            updatedSchedules[attendenceIndex].reasonOfAbsence = updatedAttendence.reasonOfAbsence;
            updatedSchedules[attendenceIndex].from = updatedAttendence.from;

            return updatedSchedules;
        } else {
            // add new schedule
            return [...prevSchedules, { date: updatedAttendence.date, ranger: updatedAttendence.ranger, routeIds: [], vehicleIds: [], reasonOfAbsence: updatedAttendence.reasonOfAbsence, working: updatedAttendence.working, from: updatedAttendence.from }];
        }
    }

    const updateWorking = async (date: string, ranger: Ranger, working: boolean) => {
        try {
            if (!working) {
                clearPlan(date, ranger);
            }
            const updatedAttendence : Attendence = {date: date, ranger: ranger, from: null, reasonOfAbsence: ReasonOfAbsence.None, working: working }
            await updateAttendence(updatedAttendence);
            setSchedules(prevSchedules => updateAttendenceInSchedule(prevSchedules, updatedAttendence));
            hubConnection?.invoke("UpdateAttendence", district?.id, { ...updatedAttendence, reasonOfAbsence: Number(updatedAttendence.reasonOfAbsence) });
        }
        catch (error) {
            setError(error);
        }
    }

    const updateAttendenceFrom = async (date: string, ranger: Ranger, from: string) => {
        try {
            const updatedAttendence: Attendence = { date: date, ranger: ranger, from: from, reasonOfAbsence: ReasonOfAbsence.None, working: true };
            await updateAttendence(updatedAttendence);
            setSchedules(prevSchedules => updateAttendenceInSchedule(prevSchedules, updatedAttendence));
            hubConnection?.invoke("UpdateAttendence", district?.id, { ...updatedAttendence,  reasonOfAbsence: Number(updatedAttendence.reasonOfAbsence)});
        }
        catch (error) {
            setError(error);
        }
    }

    const updateReasonOfAbsence = async (date: string, ranger: Ranger, reasonOfAbsence: ReasonOfAbsence) => {
        try {
            const updatedAttendence: Attendence = { date: date, ranger: ranger, from: null, reasonOfAbsence: reasonOfAbsence, working: false };
            await updateAttendence(updatedAttendence);
            setSchedules(prevSchedules => updateAttendenceInSchedule(prevSchedules, updatedAttendence));
            hubConnection?.invoke("UpdateAttendence", district?.id, { ...updatedAttendence, reasonOfAbsence: Number(updatedAttendence.reasonOfAbsence) });
        }
        catch (error) {
            setError(error);
        }
    }
    const clearPlan = async (date: string, ranger: Ranger) => {
        const clearedPlan: Plan = { date: date, ranger: ranger, vehicleIds: [], routeIds: [] };
        try {
            await updatePlan(clearedPlan);
            setSchedules(prevSchedules => updatePlanInSchedule(prevSchedules, clearedPlan));
            hubConnection?.invoke("UpdatePlan", district?.id, clearedPlan);
        }
        catch (error) {
            setError(error);
        }
    }
    const addPlannedVehicle = async (date: string, rangerId: number, vehicleId: number) => {
        try {
            var updatedPlan = await addVehicle(date, rangerId, vehicleId);
            setSchedules(prevSchedules =>  updatePlanInSchedule(prevSchedules, updatedPlan));
            hubConnection?.invoke("UpdatePlan", district?.id, updatedPlan);
        }
        catch (error) {
            setError(error);
        }
    }

    const removePlannedVehicle = async (date: string, rangerId: number, vehicleId: number) => {
        try {
            var updatedPlan = await removeVehicle(date, rangerId, vehicleId);
            setSchedules(prevPlans => updatePlanInSchedule(prevPlans, updatedPlan));
            hubConnection?.invoke("UpdatePlan", district?.id, updatedPlan);
        }
        catch (error) {
            setError(error);
        }
    }

    const addPlannedRoute = async (date: string, rangerId: number, routeId: number) => {
        try {
            var updatedPlan = await addRoute(date, rangerId, routeId);
            setSchedules(prevPlans => updatePlanInSchedule(prevPlans, updatedPlan));
            hubConnection?.invoke("UpdatePlan", district?.id, updatedPlan);
        }
        catch (error) {
            setError(error);
        }
    }

    const removePlannedRoute = async (date: string, rangerId: number, routeId: number) => {
        try {
            var updatedPlan = await removeRoute(date, rangerId, routeId);
            setSchedules(prevPlans => updatePlanInSchedule(prevPlans, updatedPlan));
            hubConnection?.invoke("UpdatePlan", district?.id, updatedPlan);
        }
        catch (error) {
            setError(error);
        }
    }


    const memoValue = useMemo(
        () => ({
            schedules,
            dateRange,
            resetSchedules,
            weekForward,
            weekBack,
            changeDate,
            updateWorking,
            updateReasonOfAbsence,
            updateAttendenceFrom,
            addPlannedVehicle,
            removePlannedVehicle,
            removePlannedRoute,
            addPlannedRoute,
            loading,
            error,
        }),
        [schedules, dateRange, loading, error]
    );

    return (
        <ScheduleContext.Provider value={memoValue}>
            {children}
        </ScheduleContext.Provider>
    );
};

export default function useSchedule() {
    return useContext(ScheduleContext);
}