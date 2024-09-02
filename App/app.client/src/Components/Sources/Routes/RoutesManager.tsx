import { useState } from "react";
import { Route } from "../../../Services/RouteService";
import useDistrict from "../../DistrictContext/DistrictDataProvider";
import RouteItem from "./RouteItem";
import RouteForm from "./RouteForm";


const RoutesManager = ({ districtId }: { districtId: number }): JSX.Element => {
    const { routes, addRoute} = useDistrict();
    const [isCreateActive, setIsCreateActive] = useState(false);

    const emptyRoute: Route = {
        id: 0,
        name: "Nová trasa",
        priority: 0, 
        controlPlace: undefined, 
        districtId: districtId
    }

    const cancelCreate = () => {
        setIsCreateActive(false);
    }

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