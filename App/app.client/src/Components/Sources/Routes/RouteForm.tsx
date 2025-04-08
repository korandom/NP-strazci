import { useState } from "react";
import { Route } from "../../../Services/RouteService";

interface RouteFormProps {
    initialRoute: Route;
    onSave: (route: Route) => void;
    onCancel: () => void;
}

/**
 * RouteForm is a component, that renders a form for creating and editing route details.
 * 
 * @param param0- RouteFormProps
 * @param param0[initialRoute] - The initial data for the route or default (empty)
 * @param param0[onSave] - Callback function for saving the data
 * @param param0[onCancel] - Callback function for cancelling changes
 * 
 * @returns {JSX.Element} of a form for creating and editing route details.
 */
const RouteForm: React.FC<RouteFormProps> = ({ initialRoute, onSave, onCancel }): JSX.Element => {
    const [editedRoute, setEditedRoute] = useState(initialRoute);
    const priorities = ["Měsíční", "Čtrtnáctidenní", "Týdenní", "Denní"];

    // handle change in other than control point data
    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setEditedRoute({
            ...editedRoute,
            [name]: value
        });
    };

    // handle chage in control point data
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

    // adding control point data
    const addControl = () => {
        setEditedRoute({
            ...editedRoute,
            controlPlace: { controlPlaceDescription: "", controlTime: "" }
        });
    };

    // removing control point data
    const removeControl = () => {
        setEditedRoute({
            ...editedRoute,
            controlPlace: undefined
        });
    };

    // save changes
    const save = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
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
        onSave(checkedRoute);
    };

    return (
        <form onSubmit={save} className="item">
            <label className="item-edit-label" htmlFor="name">Jméno:</label>
            <input
                className="item-edit-input"
                type="text"
                id="name"
                name="name"
                value={editedRoute.name}
                onChange={handleInputChange}
                required
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
                        required
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
                        required
                    />
                    <button type="button" className="control-button" onClick={removeControl} >Odebrat kontrolu</button>
                </>
            ) : (
                    <button type="button" className="control-button" onClick={addControl} >Přidat kontrolu</button>
            )}
            <button type="submit" className="save-button">Uložit</button>
            <button type="button" className="scratch-button" onClick={onCancel}>Zrušit</button>
        </form>
    )
}

export default RouteForm;                        