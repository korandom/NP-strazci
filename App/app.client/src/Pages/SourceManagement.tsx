import { useState } from "react";
import useDistrict from "../Components/DistrictContext/DistrictDataProvider";
import RangerItem from "../Components/SourceItems/RangerItem";
import RouteItem from "../Components/SourceItems/RouteItem";
import VehicleItem from "../Components/SourceItems/VehicleItem";
import "./Style/SourceManagement.css"

const SourceManagement = (): JSX.Element => {
    const { district, routes, vehicles, rangers } = useDistrict();

    return (
        <>
            {!district ? (
                <div className="error">
                    Není vybrán žádný obvod.
                </div>
            ) : (
                    <div className="district-container" >

                        <h2 className="district-name">Oblast {district?.name}</h2>

                        <div className="sources-container">

                            <div className="items-container">
                                <h3 className="source-name">Trasy</h3>
                                {routes?.map((route, index) =>
                                    <RouteItem route={route} key={index} />
                                )}
                            </div>

                            <div className="items-container">
                                <h3 className="source-name">Dopravní prostředky</h3>
                                {vehicles?.map((vehicle, index) =>
                                    <VehicleItem vehicle={vehicle} key={index} />
                                )}
                            </div>

                            <div className="items-container">
                                <h3 className="source-name">Strážci</h3>
                                {rangers?.map((ranger, index) =>
                                    <RangerItem ranger={ranger} key={index} />
                                )}
                            </div>
                        </div>
                    </div>
            )}
        </>
    );
};
export default SourceManagement;