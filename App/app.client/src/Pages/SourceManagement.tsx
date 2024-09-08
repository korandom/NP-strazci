import useDistrict from "../Components/DataProviders/DistrictDataProvider";
import "./Style/SourceManagement.css"
import RoutesManager from "../Components/Sources/Routes/RoutesManager";
import VehicleManager from "../Components/Sources/Vehicles/VehicleManager";
import RangerManager from "../Components/Sources/Rangers/RangerManager";

const SourceManagement = (): JSX.Element => {
    const { district, error } = useDistrict();

    return (
        <>
            {(error || !district) ? (
                <div className="error">
                    {error.message}
                </div>
            ) : (
                    <div className="district-container" >

                        <h2 className="district-name">Oblast {district?.name}</h2>

                        <div className="sources-container">

                            <RoutesManager districtId={district.id} />

                            <VehicleManager districtId={district.id} /> 

                            <RangerManager districtId={district.id} />
                        </div>
                    </div>
            )}
        </>
    );
};
export default SourceManagement;