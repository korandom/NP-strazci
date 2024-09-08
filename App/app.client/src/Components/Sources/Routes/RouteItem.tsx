import React, { useState } from "react";
import { Route } from "../../../Services/RouteService";
import RouteForm from "./RouteForm";
import useDistrict from "../../DataProviders/DistrictDataProvider";

const RouteItem: React.FC<{ route: Route }> = ({ route }): JSX.Element => {
    const [isEdited, setIsEdited] = useState(false);
    const { changeRoute, removeRoute } = useDistrict();
    const priorities = ["Nízká", "Střední", "Vysoká"];

    const scratchChanges = () => {
        setIsEdited(false);
    };

    const saveChanges = async (updatedRoute:Route) => {
        setIsEdited(false);
        changeRoute(updatedRoute);
    };
    const confirmDeleteRoute = async() => {
        if (window.confirm(`Opravdu chcete smazat trasu ${route.name}? Tuto akci nelze vrátit.`)) {
            removeRoute(route);
        }
    }
    return (
       <>
            {isEdited ? (
                <RouteForm initialRoute={route} onCancel={scratchChanges} onSave={saveChanges }/>
            ) : (
                    <div className = "item">
                        <div className="item-name"> {route.name} </div>
                        <div className="additional-item-info">
                                <div> Priorita: {priorities[route.priority]} </div>

                                {route.controlPlace?.controlTime &&
                                <div> Kontrola v čase: {route.controlPlace.controlTime}</div>}

                                {route.controlPlace?.controlPlaceDescription &&
                                <div> Popis místa kontroly: {route.controlPlace.controlPlaceDescription} </div>}
                        </div>
                    
                    <div className="item-buttons">
                        <button onClick={() => setIsEdited(true)}>
                                Upravit
                        </button>
                            <button className="delete-button" onClick={confirmDeleteRoute }>Smazat</button>
                    </div>
                </div >
            )}  
       </>
    )
};
export default RouteItem;