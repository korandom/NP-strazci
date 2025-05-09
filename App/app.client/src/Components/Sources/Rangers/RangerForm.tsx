import { useState } from "react";
import { Ranger } from "../../../Services/RangerService";

interface RangerFormProps {
    initialRanger: Ranger;
    onSave: (ranger: Ranger) => void;
    onCancel: () => void;
}

/**
 * RangerForm is a component, that renders a form for creating and editing ranger details.
 * 
 * @param param0- RangerFormProps
 * @param param0[initialRanger] - The initial data for the ranger or default (empty)
 * @param param0[onSave] - Callback function for saving the data
 * @param param0[onCancel] - Callback function for cancelling changes
 * 
 * @returns {JSX.Element} of a form for creating and editing ranger details.
 */
const RangerForm: React.FC<RangerFormProps> = ({ initialRanger, onSave, onCancel }): JSX.Element => {
    const [editedRanger, setEditedRanger] = useState(initialRanger);

    // handle changing data of edited ranger
    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setEditedRanger({
            ...editedRanger,
            [name]: value
        });
    };

    // save the changes
    const save = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        onSave(editedRanger);
    }

    return (
        <form onSubmit={save} className="item">
            <label className="item-edit-label" htmlFor="firstName">Jméno:</label>
            <input
                className="item-edit-input"
                type="text"
                id="firstName"
                name="firstName"
                value={editedRanger.firstName}
                onChange={handleInputChange}
                required
            />
            <label className="item-edit-label" htmlFor="lastName">Příjmení:</label>
            <input
                className="item-edit-input"
                type="text"
                id="lastName"
                name="lastName"
                value={editedRanger.lastName}
                onChange={handleInputChange}
                required
            />
            <label className="item-edit-label" htmlFor="email">Email:</label>
            <input
                className="item-edit-input"
                type="email"
                id="email"
                name="email"
                value={editedRanger.email}
                onChange={handleInputChange}
                required
            />

            <button type="submit" className="save-button">Uložit</button>
            <button className="scratch-button" onClick={onCancel}>Zrušit</button>
        </form>
    )
}

export default RangerForm;                        