import React, { useState } from "react";
import { Vehicle } from "../../../Services/VehicleService";
import VehicleForm from "./VehicleForm";
import useDistrict from '../../../Hooks/useDistrict';

/**
 * Route item manages and displays information of a single vehicle.
 * It allows editing or deleting the vehicle.
 * It uses VehicleForm to provide editing.
 * @param vehicle Vehicle
 * @returns A JSX.Element of Vehicle information.
 */
const VehicleItem: React.FC<{ vehicle: Vehicle }> = ({ vehicle }): JSX.Element => {
    const [isEdited, setIsEdited] = useState(false);
    const { changeVehicle, removeVehicle} = useDistrict();

    // cancel editing without saving changes
    const scratchChanges = () => {
        setIsEdited(false);
    };

    // save changes
    const saveChanges = async (updatedVehicle: Vehicle) => {
        setIsEdited(false);
        changeVehicle(updatedVehicle);
    };

    // delete vehicle with confirmation
    const confirmDeleteVehicle = async () => {
        if (window.confirm(`Opravdu chcete smazat dopravní prostředek ${vehicle.name}? Tuto akci nelze vrátit.`)) {
            removeVehicle(vehicle);
        }
    }

    return (

        <>
            {isEdited ? (
                <VehicleForm initialVehicle={vehicle} onCancel={scratchChanges} onSave={saveChanges}/>
            ) : (
                <div className="item">
                    <div className="item-name"> {vehicle.name} </div>
                    <div className="additional-item-info">
                        <div> Typ: {vehicle.type} </div>
                    </div>

                    <div className="item-buttons">
                        <button onClick={() => setIsEdited(true)}>
                            Upravit
                        </button>
                        <button className="delete-button" onClick={confirmDeleteVehicle }>Smazat</button>
                    </div>
                </div>
            )}
        </>
    )
};
export default VehicleItem;