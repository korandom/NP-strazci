import React, { useState } from "react";
import { Vehicle } from "../../../Services/VehicleService";
import VehicleForm from "./VehicleForm";
import useDistrict from "../../DistrictContext/DistrictDataProvider";

const VehicleItem: React.FC<{ vehicle: Vehicle }> = ({ vehicle }): JSX.Element => {
    const [isEdited, setIsEdited] = useState(false);
    const { changeVehicle, removeVehicle} = useDistrict();

    const scratchChanges = () => {
        setIsEdited(false);
    };

    const saveChanges = async (updatedVehicle: Vehicle) => {
        setIsEdited(false);
        changeVehicle(updatedVehicle);
    };
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