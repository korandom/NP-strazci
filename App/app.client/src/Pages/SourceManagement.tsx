import useDistrict from '../Hooks/useDistrict';
import "./Style/SourceManagement.css"
import RoutesManager from "../Components/Sources/Routes/RoutesManager";
import VehicleManager from "../Components/Sources/Vehicles/VehicleManager";
import RangerManager from "../Components/Sources/Rangers/RangerManager";

const SourceManagement = (): JSX.Element => {
    const { district, error } = useDistrict();

    return (
        <>
            {error ? (
                <div className="error">
                    {error.message}
                </div>
            ) : !district ? (
                <div className="error">
                    <p>Není vybrán žádný obvod, vyberte obvod z menu.</p>
                </div>
            ) : (
                <div className="district-container">

                    <h2 className="district-name">Obvod {district.name}</h2>

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