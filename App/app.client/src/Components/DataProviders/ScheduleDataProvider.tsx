import { createContext, ReactNode, useState, useMemo, useEffect, useRef } from 'react';
import {  Plan, addRoute, addVehicle, removeRoute, removeVehicle, updatePlan } from '../../Services/PlanService';
import useDistrict from '../../Hooks/useDistrict';
import useAuth from '../../Hooks/useAuth';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { calculateTwoWeeksRange, formatDate, getShiftedDate } from '../../Util/DateUtil';
import { RangerSchedule, fetchRangerSchedulesByDateRange } from '../../Services/RangerScheduleService';
import { Attendence, ReasonOfAbsence, updateAttendence } from '../../Services/AttendenceService';
import { Ranger } from '../../Services/RangerService';
import { toError } from '../../Util/toError';


interface ScheduleContextType {
    schedules: RangerSchedule[],
    dateRange: { start: Date, end: Date },
    resetSchedules: () => void,
    weekForward: () => void,
    weekBack: () => void,
    changeDate: (date: Date) => void,
    triggerReload: ()=> void,

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
    error: Error|null,
}

export const ScheduleContext = createContext<ScheduleContextType>({} as ScheduleContextType);

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
    const [error, setError] = useState<Error|null>(null);
    const [hubConnection, setHubConnection] = useState<HubConnection>();
    const isInitializing = useRef(false);

    useEffect(() => {
        const connect = async () => {
            if (isInitializing.current) return;
            if (!district) return;
            isInitializing.current = true;

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

            connection.on('Reload', () => {
                changeDate(dateRange.start);
            })

            await connection.start()
                .catch(setError);

            if (district) {
                await connection.invoke('AddToPlanGroup', district.id);
            }

            setHubConnection(connection);
            return connection;
        };

        connect().catch(setError).finally(() => isInitializing.current = false);

        return () => {
            if (hubConnection) {
                hubConnection.stop();
            }
        };
    }, [district]);

    // triggers other clients on major changes, requiring reloading data from server
    const triggerReload = () => {
        try {
            changeDate(dateRange.start);
            hubConnection?.invoke("TriggerReload", district?.id);
        }
        catch (error){
            setError(toError(error));
        }
    }
    
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

    // reset schedules when district changes
    useEffect(() => {
        resetSchedules();
    }, [district]);

    // reset schedules to today
    const resetSchedules = () => {
        changeDate(new Date());
    };

    // change date, fetch new schedule data based on date, only if the user is authorized
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
        catch (err) {
            setError(toError(error));
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
        catch (error) {
            setError(toError(error));
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
        catch (error) {
            setError(toError(error));
        }
        finally {
            setLoading(false);
        }
    }

    //splits schedules into first and second week for synchronization after update 
    const splitSchedules = (newSchedules: RangerSchedule[], range: { start: Date, end: Date }) => {
        const firstWeekEnd = getShiftedDate(range.start, 6);
        const secondWeekStart = getShiftedDate(range.end, -6);

        const firstWeekSchedules = newSchedules.filter(schedule =>
            new Date(schedule.date) >= range.start && new Date(schedule.date) <= firstWeekEnd
        );
        const secondWeekSchedules = newSchedules.filter(schedule =>
            new Date(schedule.date) >= secondWeekStart && new Date(schedule.date) <= range.end
        );

        setFirstWeek(firstWeekSchedules);
        setSecondWeek(secondWeekSchedules);
    }

    // when plan updates - it is updated in the schedule
    const updatePlanInSchedule = (prevSchedules: RangerSchedule[], updatedPlan: Plan): RangerSchedule[] => {
        // update existing plan if it already exists
        const updatedSchedules = prevSchedules.map(schedule =>
            schedule.date === updatedPlan.date && schedule.ranger.id === updatedPlan.ranger.id
                ? { ...schedule, routeIds: updatedPlan.routeIds, vehicleIds: updatedPlan.vehicleIds }
                : schedule
        );
        // add new plan if it didnt exist
        const finalSchedules = updatedSchedules.some(schedule =>
            schedule.date === updatedPlan.date && schedule.ranger.id === updatedPlan.ranger.id
        )
            ? updatedSchedules
            : [...updatedSchedules, { date: updatedPlan.date, ranger: updatedPlan.ranger, routeIds: updatedPlan.routeIds, vehicleIds: updatedPlan.vehicleIds, reasonOfAbsence: ReasonOfAbsence.None, working: true, from: null }];

        splitSchedules(finalSchedules, dateRange);
        return finalSchedules;
    };

    // when attendence is updated, update it in schedule structure
    const updateAttendenceInSchedule = (prevSchedules: RangerSchedule[], updatedAttendence: Attendence): RangerSchedule[] => {
        // update existing attendence if it already exists
        const updatedSchedules = prevSchedules.map(schedule =>
            schedule.date === updatedAttendence.date && schedule.ranger.id === updatedAttendence.ranger.id
                ? { ...schedule, working: updatedAttendence.working, reasonOfAbsence: updatedAttendence.reasonOfAbsence, from: updatedAttendence.from }
                : schedule
        );
        // add new attendence if it didnt exist
        const finalSchedules = updatedSchedules.some(schedule =>
            schedule.date === updatedAttendence.date && schedule.ranger.id === updatedAttendence.ranger.id
        )
            ? updatedSchedules
            : [...updatedSchedules, { date: updatedAttendence.date, ranger: updatedAttendence.ranger, routeIds: [], vehicleIds: [], reasonOfAbsence: updatedAttendence.reasonOfAbsence, working: updatedAttendence.working, from: updatedAttendence.from }];

        splitSchedules(finalSchedules, dateRange);
        return finalSchedules;
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
            setError(toError(error));
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
            setError(toError(error));
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
            setError(toError(error));
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
            setError(toError(error));
        }
    }
    const addPlannedVehicle = async (date: string, rangerId: number, vehicleId: number) => {
        try {
            const updatedPlan = await addVehicle(date, rangerId, vehicleId);
            setSchedules(prevSchedules =>  updatePlanInSchedule(prevSchedules, updatedPlan));
            hubConnection?.invoke("UpdatePlan", district?.id, updatedPlan);
        }
        catch (error) {
            setError(toError(error));
        }
    }

    const removePlannedVehicle = async (date: string, rangerId: number, vehicleId: number) => {
        try {
            const updatedPlan = await removeVehicle(date, rangerId, vehicleId);
            setSchedules(prevPlans => updatePlanInSchedule(prevPlans, updatedPlan));
            hubConnection?.invoke("UpdatePlan", district?.id, updatedPlan);
        }
        catch (error) {
            setError(toError(error));
        }
    }

    const addPlannedRoute = async (date: string, rangerId: number, routeId: number) => {
        try {
            const updatedPlan = await addRoute(date, rangerId, routeId);
            setSchedules(prevPlans => updatePlanInSchedule(prevPlans, updatedPlan));
            hubConnection?.invoke("UpdatePlan", district?.id, updatedPlan);
        }
        catch (error) {
            setError(toError(error));
        }
    }

    const removePlannedRoute = async (date: string, rangerId: number, routeId: number) => {
        try {
            const updatedPlan = await removeRoute(date, rangerId, routeId);
            setSchedules(prevPlans => updatePlanInSchedule(prevPlans, updatedPlan));
            hubConnection?.invoke("UpdatePlan", district?.id, updatedPlan);
        }
        catch (error) {
            setError(toError(error));
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
            triggerReload,
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

