import { useState } from "react";
import useDistrict from "../../DataProviders/DistrictDataProvider";
import VehicleItem from "./VehicleItem";
import VehicleForm from "./VehicleForm";
import { Vehicle } from "../../../Services/VehicleService";


const VehicleManager = ({ districtId }: { districtId: number }): JSX.Element => {
    const { vehicles, addVehicle } = useDistrict();
    const [isCreateActive, setIsCreateActive] = useState(false);

    const emptyVehicle: Vehicle = {
        id: 0,
        name: "",
        type: "",
        districtId: districtId
    }

    const cancelCreate = () => {
        setIsCreateActive(false);
    }

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