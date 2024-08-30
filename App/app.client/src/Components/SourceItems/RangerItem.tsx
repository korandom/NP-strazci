import React, { useState } from "react";
import { Ranger, deleteRanger, updateRanger } from "../../Services/RangerService";

const RangerItem: React.FC<{ ranger: Ranger }> = ({ ranger }): JSX.Element => {
    const [isEdited, setIsEdited] = useState(false);
    const [editedRanger, setEditedRanger] = useState(ranger);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setEditedRanger({
            ...editedRanger,
            [name]: value
        });
    };

    const scratchChanges = () => {
        setEditedRanger(ranger);
        setIsEdited(false);
    };

    const saveChanges = async () => {
        await updateRanger(editedRanger);
        setIsEdited(false);
    };
    const confirmDeleteRanger = async () => {
        if (window.confirm(`Opravdu chcete smazat strážce ${ranger.firstName} ${ranger.lastName}? Tuto akci nelze vrátit.`)) {
            await deleteRanger(ranger);
        }
    }

    return (

        <div className="item">

            {isEdited ? (
                <>
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
                    <button className="save-button" onClick={saveChanges}>Uložit</button>
                    <button className="scratch-button" onClick={scratchChanges}>Zahodit změny</button>
                </>
            ) : (
                <>
                    <div className="item-name">{editedRanger.firstName} {editedRanger.lastName} </div>
                    <div className="additional-item-info">
                        <div> Email: {editedRanger.email} </div>
                    </div>

                    <div className="item-buttons">
                        <button onClick={() => setIsEdited(true)}>
                            Upravit
                        </button>
                        <button className="delete-button" onClick={confirmDeleteRanger }>Smazat</button>
                    </div>
                </>
            )}
        </div>
    )
};
export default RangerItem;