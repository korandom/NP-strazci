import { useState } from "react";
import useDistrict from "../../DataProviders/DistrictDataProvider";
import RangerItem from "./RangerItem";
import RangerForm from "./RangerForm";
import { Ranger } from "../../../Services/RangerService";

/**
 * RangerManager displays a list of rangers in the district and allows for creating new rangers.
 * @param districtId Id of the district.
 * @returns A JSX.Element of List of rangers.
 */
const RangerManager = ({ districtId }: { districtId: number }): JSX.Element => {
    const { rangers, addRanger} = useDistrict();
    const [isCreateActive, setIsCreateActive] = useState(false);

    // empty starting ranger for creation
    const emptyRanger: Ranger= {
        id: 0,
        firstName: "",
        lastName: "",
        email:"",
        districtId: districtId
    }

    // dont save newly created ranger
    const cancelCreate = () => {
        setIsCreateActive(false);
    }

    // save newly created ranger
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