import { createContext, useContext, ReactNode, useState, useMemo, useEffect } from 'react';
import {  Plan, addRoute, addVehicle, fetchPlansByDateRange, removeRoute, removeVehicle } from '../../Services/PlanService';
import useDistrict from './DistrictDataProvider';
import useAuth from '../Authentication/AuthProvider';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { calculateTwoWeeksRange, formatDate, getShiftedDate } from '../../Util/DateUtil';


interface PlanContextType {
    plans: Plan[],
    dateRange: { start: Date, end: Date },
    resetPlans: () => void,
    weekForward: () => void,
    weekBack: () => void,
    addPlannedVehicle: (date: string, rangerId: number, vehicleId: number) => void,
    removePlannedVehicle: (date: string, rangerId: number, vehicleId: number) => void,
    addPlannedRoute: (date: string, rangerId: number, routeId: number) => void,
    removePlannedRoute: (date: string, rangerId: number, routeId: number) => void,

    loading: boolean,
    error: any,
}

const PlanContext = createContext<PlanContextType>({} as PlanContextType);

/**
 * PlansProvider manages the state and logic for plans in the application in a centralized way.
 * It provides the context for making changes to plans and fetching plans for viewing. 
 * It receives updates of plans via HubConnection.
 *
 * @param children - The child components that will have access to the plans context.
 * @returns A JSX.Element that provides the context to its children.
 * 
 * Automatically fetches new plans when the district or user changes.
 * Manages weekly navigation (forward/backward) and resets of plans.
 * Supports operations for adding/removing vehicles and routes from plans.
 */
export const PlansProvider = ({ children }: { children: ReactNode }): JSX.Element => {
    const { district } = useDistrict();
    const { user } = useAuth();
    const [plans, setPlans] = useState<Plan[]>([]);
    const [week1Plans, setWeek1Plans] = useState<Plan[]>([]);
    const [week2Plans, setWeek2Plans] = useState<Plan[]>([]);
    const [dateRange, setDateRange] = useState<{ start: Date, end: Date }>(calculateTwoWeeksRange(new Date()));
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<any>();
    const [hubConnection, setHubConnection] = useState<HubConnection>();

    useEffect(() => {
        const connect = async () => {
            if (!district) return;
            const connection = new HubConnectionBuilder()
                .withUrl('/planHub')
                .configureLogging(LogLevel.Information)
                .build();

            connection.on('PlanUpdated', (plan: Plan) => {
                setPlans(prevPlans => updatePlans(prevPlans, plan));
            });

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

    // fetch plans by date range 
    const fetchPlans = (start: Date, end: Date) : Promise<Plan[]> => {
        if (!district) {
            throw new Error("Není vybrán žádný obvod.");
        }
        const startDate = formatDate(start);
        const endDate = formatDate(end);
        const fetchedPlans = fetchPlansByDateRange(district.id, startDate, endDate);
        return fetchedPlans;
    };

    // reset plans when district or user changes
    useEffect(() => {
        resetPlans();
    }, [district, user]);

    // reset plans, fetch new plans only if user is authorized
    const resetPlans = () => {
        if (!user) {
            setPlans([]);
        }
        else {
            setLoading(true);
            const range = calculateTwoWeeksRange(new Date());
            setDateRange(range);
            initializePlans(range.start, range.end);
            setLoading(false);
        }
    };

    // fetches and sets new plans according to the range
    // start must be monday of one week, end is sunday of second week
    const initializePlans = async (start: Date, end: Date) => {
        try {
            const endFirstWeek = getShiftedDate(start, 6);
            const startSecondWeek = getShiftedDate(end, -6);

            const [firstWeekPlans, secondWeekPlans] = await Promise.all([
                fetchPlans(start, endFirstWeek),
                fetchPlans(startSecondWeek, end),
            ]);

            setWeek1Plans(firstWeekPlans);
            setWeek2Plans(secondWeekPlans);
            setPlans([...firstWeekPlans, ...secondWeekPlans]);
            setError(null);
        }
        catch (err: any) {
            setError(err);
        }
    }

    // move plans range a week forward and update plans
    const weekForward = async () => {
        try {
            setLoading(true);

            const nextWeekStart = getShiftedDate(dateRange.end, 1);
            const nextWeekEnd = getShiftedDate(dateRange.end, 7);
            const newSecondWeekPlans = await fetchPlans(nextWeekStart, nextWeekEnd);

            setWeek1Plans(week2Plans);
            setWeek2Plans(newSecondWeekPlans);
            setPlans([...week2Plans, ...newSecondWeekPlans]);
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

    // move plans range a week back and update plans
    const weekBack = async() => {
        try {
            setLoading(true);

            const previousWeekStart = getShiftedDate(dateRange.start, -7);
            const previousWeekEnd = getShiftedDate(dateRange.start, -1);
            const newFirstWeekPlans = await fetchPlans(previousWeekStart, previousWeekEnd);

            setWeek1Plans(newFirstWeekPlans);
            setWeek2Plans(week1Plans);
            setPlans([...newFirstWeekPlans, ...week1Plans]);
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

    // updates or adds a singular plan into an array and returns the updated array
    const updatePlans = (prevPlans: Plan[], updatedPlan: Plan): Plan[] => {
        const planIndex = prevPlans.findIndex(plan => plan.date === updatedPlan.date && plan.ranger.id === updatedPlan.ranger.id);

        if (planIndex !== -1) {
            // update existing plan
            const updatedPlans = [...prevPlans];
            updatedPlans[planIndex] = updatedPlan;
            return updatedPlans;
        } else {
            // add new plan
            return [...prevPlans, updatedPlan];
        }
    };


    const addPlannedVehicle = async (date: string, rangerId: number, vehicleId: number) => {
        try {
            var updatedPlan = await addVehicle(date, rangerId, vehicleId);
            setPlans(prevPlans =>  updatePlans(prevPlans, updatedPlan));
            hubConnection?.invoke("UpdatePlan", district?.id, updatedPlan);
        }
        catch (error) {
            setError(error);
        }
    }

    const removePlannedVehicle = async (date: string, rangerId: number, vehicleId: number) => {
        try {
            var updatedPlan = await removeVehicle(date, rangerId, vehicleId);
            setPlans(prevPlans => updatePlans(prevPlans, updatedPlan));
            hubConnection?.invoke("UpdatePlan", district?.id, updatedPlan);
        }
        catch (error) {
            setError(error);
        }
    }

    const addPlannedRoute = async (date: string, rangerId: number, routeId: number) => {
        try {
            var updatedPlan = await addRoute(date, rangerId, routeId);
            setPlans(prevPlans => updatePlans(prevPlans, updatedPlan));
            hubConnection?.invoke("UpdatePlan", district?.id, updatedPlan);
        }
        catch (error) {
            setError(error);
        }
    }

    const removePlannedRoute = async (date: string, rangerId: number, routeId: number) => {
        try {
            var updatedPlan = await removeRoute(date, rangerId, routeId);
            setPlans(prevPlans => updatePlans(prevPlans, updatedPlan));
            hubConnection?.invoke("UpdatePlan", district?.id, updatedPlan);
        }
        catch (error) {
            setError(error);
        }
    }


    const memoValue = useMemo(
        () => ({
            plans,
            dateRange,
            resetPlans,
            weekForward,
            weekBack,
            addPlannedVehicle,
            removePlannedVehicle,
            removePlannedRoute,
            addPlannedRoute,
            loading,
            error,
        }),
        [plans, dateRange, loading, error]
    );

    return (
        <PlanContext.Provider value={memoValue}>
            {children}
        </PlanContext.Provider>
    );
};

export default function usePlans() {
    return useContext(PlanContext);
}