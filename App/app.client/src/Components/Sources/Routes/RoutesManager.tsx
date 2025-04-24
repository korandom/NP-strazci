import { useState } from "react";
import { Route } from "../../../Services/RouteService";
import useDistrict from '../../../Hooks/useDistrict';
import RouteItem from "./RouteItem";
import RouteForm from "./RouteForm";

/**
 * RoutesManager displays a list of routes in the district and allows for creating new routes.
 * @param districtId Id of the district.
 * @returns A JSX.Element of List of routes.
 */
const RoutesManager = ({ districtId }: { districtId: number }): JSX.Element => {
    const { routes, addRoute} = useDistrict();
    const [isCreateActive, setIsCreateActive] = useState(false);

    // default empty route
    const emptyRoute: Route = {
        id: 0,
        name: "",
        priority: 0, 
        controlPlace: undefined, 
        districtId: districtId
    }

    // stop creating without saving new route 
    const cancelCreate = () => {
        setIsCreateActive(false);
    }

    // create - save new route
    const create = async (route: Route) => {
        setIsCreateActive(false);
        addRoute(route);
    }

    return (
        <div className="items-container">
            <h3 className="source-name">Trasy</h3>
            <button onClick={() => setIsCreateActive(true)}>Vytvořit</button>

            {isCreateActive &&
                <RouteForm initialRoute={emptyRoute} onCancel={cancelCreate} onSave={create} />
            }

            {routes?.map((route, index) =>
                <RouteItem route={route} key={index} />
            )}
        </div>
    )
}

export default RoutesManager;