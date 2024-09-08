import { createContext, useContext, ReactNode, useState, useMemo, useEffect } from 'react';
import {  Plan, addRoute, addVehicle, fetchPlansByDateRange, removeRoute, removeVehicle } from '../../Services/PlanService';
import useDistrict from './DistrictDataProvider';
import useAuth from '../Authentication/AuthProvider';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

interface PlanContextType {
    plans: Plan[],
    month: string,
    monthRange: { startDate: string, endDate: string },
    resetToCurrentMonth: () => void,
    changeMonth: (newMonth: string) => void,
    addPlannedVehicle: (date: string, rangerId: number, vehicleId: number) => void,
    removePlannedVehicle: (date: string, rangerId: number, vehicleId: number) => void,
    addPlannedRoute: (date: string, rangerId: number, routeId: number) => void,
    removePlannedRoute: (date: string, rangerId: number, routeId: number) => void,

    loading: boolean,
    error: any,
}

const PlanContext = createContext<PlanContextType>({} as PlanContextType);

export const MonthlyPlansProvider = ({ children }: { children: ReactNode }): JSX.Element => {
    const { district } = useDistrict();
    const { user } = useAuth();
    const [plans, setPlans] = useState<Plan[]>([]);
    const [month, setMonth] = useState<string>(getCurrentMonth());
    const [monthRange, setMonthRange] = useState<{ startDate: string, endDate: string }>(calculateMonthRange(month));
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
                await connection.invoke('AddToPlanGroup', month, district.id);
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
    }, [district, month]);

    // get current month in string format
    function getCurrentMonth(): string {
        const currentDate = new Date();
        const currentMonth = currentDate.getMonth() + 1;
        return `${currentDate.getFullYear()}-${currentMonth.toString().padStart(2, '0')}`;
    }

    // calculating start and end days of a month
    function calculateMonthRange (month: string): { startDate: string, endDate: string } {
        const start = new Date(month + '-01');
        const end = new Date(start.getFullYear(), start.getMonth() + 1, 1);

        const startDate = start.toISOString().split('T')[0];
        const endDate = end.toISOString().split('T')[0];
        return { startDate, endDate };
    }

    // fetch plans by date range
    const fetchPlans = async (startDate: string, endDate: string) => {
        try {
            if (!district) {
                throw new Error("Není vybrán žádný obvod.");
            }
            setLoading(true);
            const fetchedPlans = await fetchPlansByDateRange(district.id, startDate, endDate);
            setPlans(fetchedPlans);
            setError(null);
        } catch (err: any) {
            setError(err);
        } finally {
            setLoading(false);
        }
    };

    // fetch plans when month range or district changes, only if authentized
    useEffect(() => {
        if (!user) {
            setPlans([]);
        }
        else {
            fetchPlans(monthRange.startDate, monthRange.endDate);
        }
    }, [monthRange, district, user]);

    // change month
    const changeMonth = (newMonth: string) => {
        if (newMonth === month) {
            return;
        }
        setMonth(newMonth);
        setMonthRange(calculateMonthRange(newMonth));
    };

    // reset
    const resetToCurrentMonth = () => {
        const currentMonth = getCurrentMonth();
        changeMonth(currentMonth);
    };

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
            hubConnection?.invoke("UpdatePlan", month, district?.id, updatedPlan);
        }
        catch (error) {
            setError(error);
        }
    }

    const removePlannedVehicle = async (date: string, rangerId: number, vehicleId: number) => {
        try {
            var updatedPlan = await removeVehicle(date, rangerId, vehicleId);
            setPlans(prevPlans => updatePlans(prevPlans, updatedPlan));
            hubConnection?.invoke("UpdatePlan", month, district?.id, updatedPlan);
        }
        catch (error) {
            setError(error);
        }
    }

    const addPlannedRoute = async (date: string, rangerId: number, routeId: number) => {
        try {
            var updatedPlan = await addRoute(date, rangerId, routeId);
            setPlans(prevPlans => updatePlans(prevPlans, updatedPlan));
            hubConnection?.invoke("UpdatePlan", month, district?.id, updatedPlan);
        }
        catch (error) {
            setError(error);
        }
    }

    const removePlannedRoute = async (date: string, rangerId: number, routeId: number) => {
        try {
            var updatedPlan = await removeRoute(date, rangerId, routeId);
            setPlans(prevPlans => updatePlans(prevPlans, updatedPlan));
            hubConnection?.invoke("UpdatePlan", month, district?.id, updatedPlan);
        }
        catch (error) {
            setError(error);
        }
    }


    const memoValue = useMemo(
        () => ({
            plans,
            month,
            monthRange,
            resetToCurrentMonth,
            changeMonth,
            addPlannedVehicle,
            removePlannedVehicle,
            removePlannedRoute,
            addPlannedRoute,
            loading,
            error,
        }),
        [plans, loading, error]
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