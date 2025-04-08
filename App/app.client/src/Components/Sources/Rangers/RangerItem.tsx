import React, { useState } from "react";
import { Ranger } from "../../../Services/RangerService";
import RangerForm from "./RangerForm";
import useDistrict from "../../DataProviders/DistrictDataProvider";

/**
 * Ranger item manages and displays information of a single ranger.
 * It allows editing or deleting the ranger.
 * It uses RangerForm to provide editing.
 * @param ranger Ranger
 * @returns A JSX.Element of Ranger information.
 */
const RangerItem: React.FC<{ ranger: Ranger }> = ({ ranger }): JSX.Element => {
    const [isEdited, setIsEdited] = useState(false);
    const { changeRanger, removeRanger } = useDistrict();

    // unset changes
    const scratchChanges = () => {
        setIsEdited(false);
    };

    // save changes
    const saveChanges = async (updatedRanger: Ranger) => {
        setIsEdited(false);
        changeRanger(updatedRanger);
    };

    // delete ranger with confirmation
    const confirmDeleteRanger = async () => {
        if (window.confirm(`Opravdu chcete smazat strážce ${ranger.firstName} ${ranger.lastName}? Tuto akci nelze vrátit.`)) {
            removeRanger(ranger);
        }
    }

    return (

        <>

            {isEdited ? (
                <RangerForm initialRanger={ranger} onCancel={scratchChanges} onSave={saveChanges}/>
            ) : (
                <div className="item">
                    <div className="item-name">{ranger.firstName} {ranger.lastName} </div>
                    <div className="additional-item-info">
                        <div> Email: {ranger.email} </div>
                    </div>

                    <div className="item-buttons">
                        <button onClick={() => setIsEdited(true)}>
                            Upravit
                        </button>
                        <button className="delete-button" onClick={confirmDeleteRanger }>Smazat</button>
                    </div>
                </div>
            )}
        </>
    )
};
export default RangerItem;