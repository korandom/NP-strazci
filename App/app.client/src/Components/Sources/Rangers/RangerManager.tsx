import { useState } from "react";
import useDistrict from "../../DistrictContext/DistrictDataProvider";
import RangerItem from "./RangerItem";
import RangerForm from "./RangerForm";
import { Ranger } from "../../../Services/RangerService";


const RangerManager = ({ districtId }: { districtId: number }): JSX.Element => {
    const { rangers, addRanger} = useDistrict();
    const [isCreateActive, setIsCreateActive] = useState(false);

    const emptyRanger: Ranger= {
        id: 0,
        firstName: "",
        lastName: "",
        email:"",
        districtId: districtId
    }

    const cancelCreate = () => {
        setIsCreateActive(false);
    }

    const create = async (ranger: Ranger) => {
        setIsCreateActive(false);
        addRanger(ranger);
    }

    return (
        <div className="items-container">
            <h3 className="source-name">Strážci</h3>
            <button onClick={() => setIsCreateActive(true)}>Vytvořit</button>

            {isCreateActive &&
                <RangerForm initialRanger={emptyRanger} onCancel={cancelCreate} onSave={create} />
            }

            {rangers?.map((ranger, index) =>
                <RangerItem ranger={ranger} key={index} />
            )}
        </div>
    )
}

export default RangerManager;