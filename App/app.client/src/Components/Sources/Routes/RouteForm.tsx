import { useState } from "react";
import { Route } from "../../../Services/RouteService";

interface RouteFormProps {
    initialRoute: Route;
    onSave: (route: Route) => void;
    onCancel: () => void;
}

const RouteForm: React.FC<RouteFormProps> = ({ initialRoute, onSave, onCancel }): JSX.Element => {
    const [editedRoute, setEditedRoute] = useState(initialRoute);
    const priorities = ["Měsíční", "Čtrtnáctidenní", "Týdenní", "Denní"];

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
            controlPlace: { controlPlaceDescription: "", controlTime: "" }
        });
    };

    const removeControl = () => {
        setEditedRoute({
            ...editedRoute,
            controlPlace: undefined
        });
    };

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