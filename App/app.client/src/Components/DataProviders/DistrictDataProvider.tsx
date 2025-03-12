import { Route, createRoute, deleteRoute, fetchRoutesByDistrict, updateRoute } from '../../Services/RouteService';
import { Vehicle, createVehicle, deleteVehicle, fetchVehiclesByDistrict, updateVehicle } from '../../Services/VehicleService';
import { Ranger, createRanger, deleteRanger, fetchRangersByDistrict, updateRanger} from '../../Services/RangerService';
import { District, fetchDistrictById} from '../../Services/DistrictService';
import { createContext, useContext, ReactNode, useState, useMemo } from 'react';
import { Locked, fetchLocks, lockPlans, unlockPlans } from '../../Services/LockService';
import { HubConnection, HubConnectionBuilder, LogLevel} from '@microsoft/signalr';

interface DistrictContextType {
    district: District|undefined, 
    routes: Route[],
    vehicles: Vehicle[],
    rangers: Ranger[],
    locks: Locked[],

    assignDistrict: (districtId: number) => void,
    clearDistrict: () => void,

    addRoute: (route: Route) => void,
    removeRoute: (route: Route) => void,
    changeRoute: (route: Route) => void

    addVehicle: (vehicle: Vehicle) => void,
    removeVehicle: (vehicle: Vehicle) => void,
    changeVehicle: (vehicle: Vehicle) => void,

    addRanger: (ranger: Ranger) => void,
    removeRanger: (ranger: Ranger) => void,
    changeRanger: (ranger: Ranger) => void,

    addLock: (date: string) => void,
    removeLock: (date:string)=> void,

    loading: boolean,
    error: any,
}

const DistrictContext = createContext<DistrictContextType>({} as DistrictContextType);

/**
 * DistrictDataProvider manages the sources of a district - routes, vehicles, rangers and their operations in a centralized way.
 * It provides the context for removing, adding or updating sources. 
 * It receives updates via HubConnection.
 *
 * @param children - The child components that will have access to the district context.
 * @returns A JSX.Element that provides the context to its children.
 * 
 */
export const DistrictDataProvider = ({ children }: { children: ReactNode }): JSX.Element => {
    const [district, setDistrict] = useState<District | undefined>(undefined);
    const [routes, setRoutes] = useState<Route[]>([]);
    const [vehicles, setVehicles] = useState<Vehicle[]>([]);
    const [rangers, setRangers] = useState<Ranger[]>([]);
    const [locks, setLocks] = useState<Locked[]>([]);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<any>();
    const [hubConnection, setHubConnection] = useState<HubConnection>();

    const connect = async (districtId: number) => {
        const connection = new HubConnectionBuilder()
            .withUrl('/districtHub')
            .configureLogging(LogLevel.Information) 
            .build();

        connection.on('RouteUpdated', (route: Route) => {
            setRoutes(prevRoutes => prevRoutes.map(r => r.id === route.id ? route : r));
        });

        connection.on('RouteAdded', (route: Route) => {
            setRoutes(prevRoutes => [...prevRoutes, route]);
        });

        connection.on('RouteDeleted', (route: Route) => {
            setRoutes(prevRoutes => prevRoutes.filter(r => r.id !== route.id));
        });

        connection.on('VehicleUpdated', (vehicle: Vehicle) => {
            setVehicles(prevVehicles => prevVehicles.map(v => v.id === vehicle.id ? vehicle : v));
        });

        connection.on('VehicleAdded', (vehicle: Vehicle) => {
            setVehicles(prevVehicles => [...prevVehicles, vehicle]);
        });

        connection.on('VehicleDeleted', (vehicle: Vehicle) => {
            setVehicles(prevVehicles => prevVehicles.filter(v => v.id !== vehicle.id));
        });

        connection.on('RangerUpdated', (ranger: Ranger) => {
            setRangers(prevRangers => prevRangers.map(r => r.id === ranger.id ? ranger : r));
        });

        connection.on('RangerAdded', (ranger: Ranger) => {
            setRangers(prevRangers => [...prevRangers, ranger]);
        });

        connection.on('RangerDeleted', (ranger: Ranger) => {
            setRangers(prevRangers => prevRangers.filter(r => r.id !== ranger.id));
        });

        connection.on('LockAdded', (lock: Locked) => {
            setLocks(prevLocks => [...prevLocks, lock]);
        });

        connection.on('LockDeleted', (lock: Locked) => {
            setLocks(prevLocks => prevLocks.filter(l => l.date !== lock.date));
        });

        await connection.start()
            .catch(err => console.error("Error starting connection: ", err));

        await connection.invoke('AddToDistrictGroup', districtId);


        setHubConnection(connection);
    };

       
 
    // District
    async function assignDistrict(districtId: number) {
        setLoading(true);
        try {
            const fetchedDistrict = await fetchDistrictById(districtId);
            const fetchedRoutes = await fetchRoutesByDistrict(districtId);
            routes.sort((a, b) => a.name > b.name ? 1 : -1);
            const fetchedVehicles = await fetchVehiclesByDistrict(districtId);
            const fetchedRangers = await fetchRangersByDistrict(districtId);
            const fetchedLocks = await fetchLocks(districtId);
            connect(districtId);

            setDistrict(fetchedDistrict);
            setRoutes(fetchedRoutes);
            setVehicles(fetchedVehicles);
            setRangers(fetchedRangers);
            setLocks(fetchedLocks);

        }
        catch (error: any) {
            setError(error);
        }
        finally {
            setLoading(false);
        }
    }
    async function clearDistrict() {
        if (hubConnection) {
            hubConnection.stop();
        }
        setDistrict(undefined);
        setRoutes([]);
        setVehicles([]);
        setRangers([]);
        setLocks([]);
    }

    // Route Changes management 
    const addRoute = async (route: Route) => {
        try {

            const newRoute = await createRoute(route);
            hubConnection?.invoke("SendRouteNotification", "RouteAdded", newRoute.districtId, newRoute);
            setRoutes([...routes, newRoute]);
        } catch (error) {
            setError(error);
        }
    };

    const removeRoute = async (route: Route) => {
        const originalRoutes = [...routes];
        setRoutes(routes.filter(r => r.id !== route.id));

        try {
            await deleteRoute(route);
            hubConnection?.invoke("SendRouteNotification", "RouteDeleted", route.districtId, route);

        } catch (error) {
            // rollback in case of error
            setRoutes(originalRoutes);
            setError(error);
        }
    };

    const changeRoute = async (route: Route) => {
        const originalRoutes = [...routes];
        setRoutes(routes.map(r => r.id === route.id ? route : r));

        try {
            await updateRoute(route);
            hubConnection?.invoke("SendRouteNotification", "RouteUpdated", route.districtId, route);

        } catch (error) {
            // rollback in case of error
            setRoutes(originalRoutes);
            setError(error);
        }
    };

    // Vehicle Changes management
    const addVehicle = async (vehicle: Vehicle) => {
        try {
            const newVehicle = await createVehicle(vehicle);
            hubConnection?.invoke("SendVehicleNotification", "VehicleAdded", newVehicle.districtId, newVehicle);
            setVehicles([...vehicles, newVehicle]);
        } catch (error) {
            setError(error);
        }
    };

    const removeVehicle = async (vehicle: Vehicle) => {
        const originalVehicles = [...vehicles];
        setVehicles(vehicles.filter(v => v.id !== vehicle.id));

        try {
            await deleteVehicle(vehicle);
            hubConnection?.invoke("SendVehicleNotification", "VehicleDeleted", vehicle.districtId, vehicle);

        } catch (error) {
            // rollback in case of error
            setVehicles(originalVehicles);
            setError(error);
        }
    };
    const changeVehicle = async (vehicle: Vehicle) => {
        const originalVehicles = [...vehicles];
        setVehicles(vehicles.map(v => v.id === vehicle.id ? vehicle : v));

        try {
            await updateVehicle(vehicle);
            hubConnection?.invoke("SendVehicleNotification", "VehicleUpdated", vehicle.districtId, vehicle);

        } catch (error) {
            // rollback in case of error
            setVehicles(originalVehicles);
            setError(error);
        }
    }

    // Ranger Change Management
    const addRanger = async (ranger: Ranger) => {
        try {
            const newRanger = await createRanger(ranger);
            hubConnection?.invoke("SendRangerNotification", "RangerAdded", newRanger.districtId, newRanger);
            setRangers([...rangers, newRanger]);
        } catch (error) {
            setError(error);
        }
    };

    const removeRanger = async (ranger: Ranger) => {
        const originalRangers = [...rangers];
        setRangers(rangers.filter(r => r.id !== ranger.id));

        try {
            await deleteRanger(ranger);
            hubConnection?.invoke("SendRangerNotification", "RangerDeleted", ranger.districtId, ranger);
        } catch (error) {
            // rollback in case of error
            setRangers(originalRangers);
            setError(error);
        }
    };
    const changeRanger = async (ranger: Ranger) => {
        const originalRangers = [...rangers];
        setRangers(rangers.map(r => r.id === ranger.id ? ranger : r));

        try {
            await updateRanger(ranger);
            hubConnection?.invoke("SendRangerNotification", "RangerUpdated", ranger.districtId, ranger);
        } catch (error) {
            // rollback in case of error
            setRangers(originalRangers);
            setError(error);
        }
    };
    const addLock = async (date: string) => {
        if (!district) throw new Error("Není pøiøazen obvod.");

        setLocks([...locks, { date: date, districtId: district?.id }]);
        try {
            await lockPlans(date, district.id);
            hubConnection?.invoke("SendLockNotification", "LockAdded", district.id, { date: date, districtId: district.id });
        } catch (error) {
            // rollback in case of error
            setLocks(locks.filter(l => l.date !== date));
            setError(error);
        }
    };
    const removeLock = async (date: string) => {
        if (!district) throw new Error("Není pøiøazen obvod.");

        setLocks(locks.filter(l => l.date !== date));
        try {
            await unlockPlans(date, district?.id);
            hubConnection?.invoke("SendLockNotification", "LockDeleted", district.id, { date: date, districtId: district.id });

        } catch (error) {
            // rollback in case of error
            setLocks([...locks, { date: date, districtId: district?.id }]);
            setError(error);
        }
    };
    const memoValue = useMemo(
        () => ({
            district,
            routes,
            vehicles,
            rangers,
            locks,
            loading,
            error,
            assignDistrict,
            clearDistrict,
            addRoute,
            removeRoute,
            changeRoute,
            addVehicle,
            removeVehicle,
            changeVehicle,
            addRanger,
            removeRanger,
            changeRanger,
            addLock,
            removeLock
        }),
        [district, routes, vehicles, rangers, locks, loading, error]
    );

    return (
        <DistrictContext.Provider value={memoValue}>
            {children}
        </DistrictContext.Provider>
    );
};

export default function useDistrict() {
    return useContext(DistrictContext);
}