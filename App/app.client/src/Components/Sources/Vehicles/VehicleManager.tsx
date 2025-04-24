import { useState } from "react";
import useDistrict from '../../../Hooks/useDistrict';
import VehicleItem from "./VehicleItem";
import VehicleForm from "./VehicleForm";
import { Vehicle } from "../../../Services/VehicleService";

/**
 * VehicleManager displays a list of vehicles in the district and allows for creating new vehicles.
 * @param districtId Id of the district.
 * @returns {JSX.Element} of List of vehicles.
 */
const VehicleManager = ({ districtId }: { districtId: number }): JSX.Element => {
    const { vehicles, addVehicle } = useDistrict();
    const [isCreateActive, setIsCreateActive] = useState(false);

    // empty - default vehicle
    const emptyVehicle: Vehicle = {
        id: 0,
        name: "",
        type: "",
        districtId: districtId
    }

    // cancel creating without saving changes
    const cancelCreate = () => {
        setIsCreateActive(false);
    }

    // create and save new vehicle information
    const create = async (vehicle: Vehicle) => {
        setIsCreateActive(false);
        addVehicle(vehicle);
    }

    return (
        <div className="items-container">
            <h3 className="source-name">Dopravní prostředky</h3>
            <button onClick={() => setIsCreateActive(true)}>Vytvořit</button>

            {isCreateActive &&
                <VehicleForm initialVehicle={emptyVehicle} onCancel={cancelCreate} onSave={create} />
            }

            {vehicles?.map((vehicle, index) =>
                <VehicleItem vehicle={vehicle } key={index} />
            )}
        </div>
    )
}

export default VehicleManager;