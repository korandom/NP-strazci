import React, { useState } from "react";
import { Vehicle, deleteVehicle, updateVehicle } from "../../Services/VehicleService";

const VehicleItem: React.FC<{ vehicle: Vehicle }> = ({ vehicle }): JSX.Element => {
    const [isEdited, setIsEdited] = useState(false);
    const [editedVehicle, setEditedVehicle] = useState(vehicle);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setEditedVehicle({
            ...editedVehicle,
            [name]: value
        });
    };

    const scratchChanges = () => {
        setEditedVehicle(vehicle);
        setIsEdited(false);
    };

    const saveChanges = async () => {
        await updateVehicle(editedVehicle);
        setIsEdited(false);
    };
    const confirmDeleteVehicle = async () => {
        if (window.confirm(`Opravdu chcete smazat dopravní prostředek ${vehicle.name}? Tuto akci nelze vrátit.`)) {
            await deleteVehicle(vehicle);
        }
    }

    return (

        <div className="item">

            {isEdited ? (
                <>
                    <label className="item-edit-label" htmlFor="name">Identifikační jméno:</label>
                    <input
                        className="item-edit-input"
                        type="text"
                        id="name"
                        name="name"
                        value={editedVehicle.name}
                        onChange={handleInputChange}
                    />

                    <label className="item-edit-label" htmlFor="type">Typ:</label>
                    <input
                        className="item-edit-input"
                        type="text"
                        id="type"
                        name="type"
                        value={editedVehicle.type}
                        onChange={handleInputChange}
                    />
                    <button className="save-button" onClick={saveChanges }>Uložit</button>
                    <button className="scratch-button" onClick={scratchChanges}>Zahodit změny</button>
                </>
            ) : (
                <>
                    <div className="item-name"> {editedVehicle.name} </div>
                    <div className="additional-item-info">
                        <div> Typ: {editedVehicle.type} </div>
                    </div>

                    <div className="item-buttons">
                        <button onClick={() => setIsEdited(true)}>
                            Upravit
                        </button>
                        <button className="delete-button" onClick={confirmDeleteVehicle }>Smazat</button>
                    </div>
                </>
            )}
        </div>
    )
};
export default VehicleItem;