import { Route, createRoute, deleteRoute, fetchRoutesByDistrict, updateRoute } from '../../Services/RouteService';
import { Vehicle, createVehicle, deleteVehicle, fetchVehiclesByDistrict, updateVehicle } from '../../Services/VehicleService';
import { Ranger, createRanger, deleteRanger, fetchRangersByDistrict, updateRanger} from '../../Services/RangerService';
import { District, fetchDistrictById} from '../../Services/DistrictService';
import { createContext, useContext, ReactNode, useState, useMemo } from 'react';

interface DistrictContextType {
    district: District|undefined, 
    routes: Route[],
    vehicles: Vehicle[],
    rangers: Ranger[],

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

    loading: boolean,
    error: any,
}

const DistrictContext = createContext<DistrictContextType>({} as DistrictContextType);

export const DistrictDataProvider = ({ children }: { children: ReactNode }): JSX.Element => {
    const [district, setDistrict] = useState<District | undefined>(undefined);
    const [routes, setRoutes] = useState<Route[]>([]);
    const [vehicles, setVehicles] = useState<Vehicle[]>([]);
    const [rangers, setRangers] = useState<Ranger[]>([]);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<any>();

    // District
    async function assignDistrict(districtId: number) {
        setLoading(true);
        try {
            const district = await fetchDistrictById(districtId);
            const routes = await fetchRoutesByDistrict(districtId);
            routes.sort((a, b) => a.name > b.name ? 1 : -1);
            const vehicles = await fetchVehiclesByDistrict(districtId);
            const rangers = await fetchRangersByDistrict(districtId);


            setDistrict(district);
            setRoutes(routes);
            setVehicles(vehicles);
            setRangers(rangers);

        }
        catch (error) {
            setError(error);
        }
        finally {
            setLoading(false);
        }
    }
    async function clearDistrict() {
        setDistrict(undefined);
        setRoutes([]);
        setVehicles([]);
        setRangers([]);
    }

    // Route Changes management 
    const addRoute = async (route: Route) => {
        try {

            const newRoute = await createRoute(route);
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
        } catch (error) {
            // rollback in case of error
            setRoutes(originalRoutes);
            setError(error);
        }
    }

    // Vehicle Changes management
    const addVehicle = async (vehicle: Vehicle) => {
        try {
            const newVehicle = await createVehicle(vehicle);
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
        } catch (error) {
            // rollback in case of error
            setRangers(originalRangers);
            setError(error);
        }
    }
    const memoValue = useMemo(
        () => ({
            district,
            routes,
            vehicles,
            rangers,
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
            changeRanger
        }),
        [district, routes, vehicles, rangers, loading, error]
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