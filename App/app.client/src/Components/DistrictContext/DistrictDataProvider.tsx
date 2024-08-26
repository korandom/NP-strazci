import { Route, fetchRoutesByDistrict } from '../../Services/RouteService';
import { Vehicle, fetchVehiclesByDistrict } from '../../Services/VehicleService';
import { Ranger, fetchRangersByDistrict} from '../../Services/RangerService';
import { District, fetchDistrictById} from '../../Services/DistrictService';
import { createContext, useContext, ReactNode, useState, useMemo } from 'react';

interface DistrictContextType {
    district: District|undefined, 
    routes: Route[] | undefined,
    vehicles: Vehicle[] | undefined,
    rangers: Ranger[] | undefined,
    assignDistrict: (districtId: number) => void,
    clearDistrict: () => void,
    loading: boolean,
    error: any,
}

const DistrictContext = createContext<DistrictContextType>({} as DistrictContextType);

export const DistrictDataProvider = ({ children }: { children: ReactNode }): JSX.Element => {
    const [district, setDistrict] = useState<District | undefined>(undefined);
    const [routes, setRoutes] = useState<Route[] | undefined>(undefined);
    const [vehicles, setVehicles] = useState<Vehicle[] | undefined>(undefined);
    const [rangers, setRangers] = useState<Ranger[] | undefined>(undefined);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<any>();


    async function assignDistrict(districtId: number) {
        setLoading(true);
        try {
            const district = await fetchDistrictById(districtId);
            const routes = await fetchRoutesByDistrict(districtId);
            const vehicles = await fetchVehiclesByDistrict(districtId);
            const rangers = await fetchRangersByDistrict(districtId);


            setDistrict(district);
            setRoutes(routes);
            setVehicles(vehicles);
            setRangers(rangers);

        }
        catch (error) {
            console.error("FAILED");
            setError(error);
        }
        finally {
            setLoading(false);
        }
    }
    async function clearDistrict() {
        setDistrict(undefined);
        setRoutes(undefined);
        setVehicles(undefined);
        setRangers(undefined);
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
            clearDistrict
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