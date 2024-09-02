import { useState } from "react";
import { Ranger } from "../../../Services/RangerService";

interface RangerFormProps {
    initialRanger: Ranger;
    onSave: (ranger: Ranger) => void;
    onCancel: () => void;
}

const RangerForm: React.FC<RangerFormProps> = ({ initialRanger, onSave, onCancel }): JSX.Element => {
    const [editedRanger, setEditedRanger] = useState(initialRanger);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setEditedRanger({
            ...editedRanger,
            [name]: value
        });
    };

    return (
        <div className="item">
            <label className="item-edit-label" htmlFor="firstName">Jméno:</label>
            <input
                className="item-edit-input"
                type="text"
                id="firstName"
                name="firstName"
                value={editedRanger.firstName}
                onChange={handleInputChange}
            />
            <label className="item-edit-label" htmlFor="lastName">Příjmení:</label>
            <input
                className="item-edit-input"
                type="text"
                id="lastName"
                name="lastName"
                value={editedRanger.lastName}
                onChange={handleInputChange}
            />
            <label className="item-edit-label" htmlFor="type">Email:</label>
            <input
                className="item-edit-input"
                type="email"
                id="email"
                name="email"
                value={editedRanger.email}
                onChange={handleInputChange}
            />

            <button className="save-button" onClick={() => onSave(editedRanger)}>Uložit</button>
            <button className="scratch-button" onClick={onCancel}>Zrušit</button>
        </div>
    )
}

export default RangerForm;                        