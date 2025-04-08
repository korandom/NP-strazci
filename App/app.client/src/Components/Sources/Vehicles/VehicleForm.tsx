import { useState } from "react";
import { Vehicle } from "../../../Services/VehicleService";

interface VehicleFormProps {
    initialVehicle: Vehicle;
    onSave: (vehicle: Vehicle) => void;
    onCancel: () => void;
}
/**
 * VehicleForm is a component, that renders a form for creating and editing vehicle details.
 * 
 * @param param0- VehicleFormProps
 * @param param0[initialVehicle] - The initial data for the vehicle or default (empty)
 * @param param0[onSave] - Callback function for saving the data
 * @param param0[onCancel] - Callback function for cancelling changes
 * 
 * @returns {JSX.Element} of a form for creating and editing vehicle details.
 */
const VehicleForm: React.FC<VehicleFormProps> = ({ initialVehicle, onSave, onCancel }): JSX.Element => {
    const [editedVehicle, setEditedVehicle] = useState(initialVehicle);

    // handle inputing/ changes to vehicle information
    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setEditedVehicle({
            ...editedVehicle,
            [name]: value
        });
    };

    // save changes
    const save = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        onSave(editedVehicle)
    }
    return (
        <form onSubmit={save} className="item">
            <label className="item-edit-label" htmlFor="name">Identifikační jméno:</label>
            <input
                className="item-edit-input"
                type="text"
                id="name"
                name="name"
                value={editedVehicle.name}
                onChange={handleInputChange}
                required
            />

            <label className="item-edit-label" htmlFor="type">Typ:</label>
            <input
                className="item-edit-input"
                type="text"
                id="type"
                name="type"
                value={editedVehicle.type}
                onChange={handleInputChange}
                required
            />

            <button type="submit" className="save-button" >Uložit</button>
            <button className="scratch-button" onClick={onCancel}>Zrušit</button>
        </form>
    )
}

export default VehicleForm;                        