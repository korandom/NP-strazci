import React, { useState } from "react";
import { Route, deleteRoute, updateRoute } from "../../Services/RouteService";

const RouteItem: React.FC<{ route: Route }> = ({ route }): JSX.Element => {
    const [isEdited, setIsEdited] = useState(false);
    const [editedRoute, setEditedRoute] = useState(route);

    const priorities = ["Nízká", "Střední", "Vysoká"];

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setEditedRoute({
            ...editedRoute,
            [name]: value
        });
    };

    const handleControlChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        editedRoute.controlPlace ?
            setEditedRoute({
                ...editedRoute,
                controlPlace: {
                    ...editedRoute.controlPlace,
                    [name]: value
                }
            }) : null;
    };

    const addControl = () => {
        setEditedRoute({
            ...editedRoute,
            controlPlace: {controlPlaceDescription:"", controlTime:""}
        });
    };

    const removeControl = () => {
        setEditedRoute({
            ...editedRoute,
            controlPlace: undefined
        });
    };

    const scratchChanges = () => {
        setIsEdited(false);
        setEditedRoute(route);
    };

    const saveChanges = async () => {
        setIsEdited(false);
        let checkedRoute = { ...editedRoute };

        if (
            editedRoute.controlPlace?.controlPlaceDescription.trim() === "" &&
            editedRoute.controlPlace?.controlTime.trim() === ""
        ) {
            checkedRoute = {
                ...checkedRoute,
                controlPlace: undefined,
            };
        }
        setEditedRoute(checkedRoute);
        await updateRoute(checkedRoute);
    };
    const confirmDeleteRoute = async() => {
        if (window.confirm(`Opravdu chcete smazat trasu ${route.name}? Tuto akci nelze vrátit.`)) {
           await deleteRoute(route);
        }
    }
    return (
            
        <div className="item">

            {isEdited ? (
                <>
                    <label className="item-edit-label" htmlFor="name">Jméno:</label>
                    <input
                        className="item-edit-input"
                        type="text"
                        id="name"
                        name="name"
                        value={editedRoute.name}
                        onChange={handleInputChange}
                    />
                    <label className="item-edit-label" htmlFor="priority">Priorita:</label>
                    <select
                        className="item-edit-input"
                        id="priority"
                        name="priority"
                        value={editedRoute.priority}
                        onChange={handleInputChange}
                    >
                        {priorities.map((priority, index) => (
                            <option key={index} value={index}>
                                {priority}
                            </option>
                        ))}
                    </select>
                    
                    {editedRoute.controlPlace ? (
                        <>

                            <label className="item-edit-label" htmlFor="controlTime">Čas kontroly:</label>
                            <input
                                className="item-edit-input"
                                type="time"
                                id="controlTime"
                                name="controlTime"
                                value={editedRoute.controlPlace.controlTime}
                                onChange={handleControlChange}
                            />
                            <label className="item-edit-label" htmlFor="controlPlaceDescription">Místo kontroly:</label>
                            <input
                                className="item-edit-input"
                                type="text"
                                id="controlPlaceDescription"
                                title="Control Place Description"
                                name="controlPlaceDescription"
                                value={editedRoute.controlPlace.controlPlaceDescription}
                                onChange={handleControlChange}
                            />
                            <button className="control-button" onClick={removeControl} >Odebrat kontrolu</button>
                        </>
                    ): (
                        <button className="control-button" onClick={addControl} >Přidat kontrolu</button>
                    )}
                    <button className="save-button" onClick={saveChanges}>Uložit</button>
                    <button className="scratch-button" onClick={scratchChanges}>Zahodit změny</button>
                </>
            ) : (
                <>
                        <div className="item-name"> {editedRoute.name} </div>
                        <div className="additional-item-info">
                                <div> Priorita: {priorities[editedRoute.priority]} </div>

                                {editedRoute.controlPlace?.controlTime &&
                                <div> Kontrola v čase: {editedRoute.controlPlace.controlTime}</div>}

                                {editedRoute.controlPlace?.controlPlaceDescription &&
                                <div> Popis místa kontroly: {editedRoute.controlPlace.controlPlaceDescription} </div>}
                        </div>
                    
                    <div className="item-buttons">
                        <button onClick={() => setIsEdited(true)}>
                                Upravit
                        </button>
                            <button className="delete-button" onClick={confirmDeleteRoute }>Smazat</button>
                    </div>
                </>
            )}  
        </div>
    )
};
export default RouteItem;