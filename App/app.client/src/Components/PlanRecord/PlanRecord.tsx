import React from 'react';
import { useState, useEffect } from 'react';
import './PlanRecord.css';
import { Plan, addRoute, removeRoute, addVehicle, removeVehicle } from '../../Services/PlanService';
import useDistrict from '../DistrictContext/DistrictDataProvider';
import useAuth from '../Authentication/AuthProvider';


// Konkrétní záznam plánu jednoho strážce, bez detailů
const PlanRecord: React.FC<{ plan: Plan, includeRangerName: boolean, isEditable: boolean }> = ({ plan, includeRangerName, isEditable }) => {
    // authorization for adding vehicles (only headOfDistrict)
    const { hasRole } = useAuth();

    // state managment for editing
    const [editing, setEditing] = useState(false);
    const toggleEdit = () => {
        isEditable ? setEditing(!editing) : null;
    }

    // data for adding routes and vehicles from district
    const { routes, vehicles } = useDistrict();

    // state managment for routes
    const [selectedRouteId, setSelectedRouteId] = useState<number | undefined>(undefined);
    const handleRouteSelect = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const selectedId = Number(event.target.value);
        setSelectedRouteId(selectedId);
    };
    const [plannedRoutes, setPlannedRoutes] = useState(plan.routes);
    useEffect(() => {
        setPlannedRoutes(plan.routes);
    }, [plan.routes]);

    const addRouteToPlan = () => {
        if (selectedRouteId != undefined) {
            const selectedRoute = routes?.find(route => route.id === selectedRouteId);
            
            if (selectedRoute) {
                if (plannedRoutes.find(route => route.id === selectedRoute.id)) {
                    return;
                }

                setPlannedRoutes([...plannedRoutes, selectedRoute]);
                setSelectedRouteId(undefined); 
                addRoute(plan.date, plan.ranger.id, selectedRoute.id);
            }
        }
    };
    const deleteRouteFromPlan = (routeId :number) => {
        setPlannedRoutes(plannedRoutes.filter(route => route.id !== routeId));
        removeRoute(plan.date, plan.ranger.id, routeId);
    }

    // state managment for vehicles
    const [selectedVehicleId, setSelectedVehicleId] = useState<number | undefined>(undefined);
    const handleVehicleSelect = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const selectedId = Number(event.target.value);
        setSelectedVehicleId(selectedId);
    };

    const [plannedVehicles, setPlannedVehicles] = useState(plan.vehicles);
    useEffect(() => {
        setPlannedVehicles(plan.vehicles);
    }, [plan.vehicles]);

    const addVehicleToPlan = () => {
        if (selectedVehicleId != undefined) {
            const selectedVehicle = vehicles?.find(vehicle => vehicle.id === selectedVehicleId);

            if (selectedVehicle) {
                if (plannedVehicles.find(vehicle => vehicle.id === selectedVehicle.id)) {
                    return;
                }

                setPlannedVehicles([...plannedVehicles, selectedVehicle]);
                setSelectedRouteId(undefined);
                addVehicle(plan.date, plan.ranger.id, selectedVehicle.id);
            }
        }
    }
    const deleteVehicleFromPlan = (vehicleId: number) => {
        setPlannedVehicles(plannedVehicles.filter(vehicle => vehicle.id !== vehicleId));
        removeVehicle(plan.date, plan.ranger.id, vehicleId);
    }

    return (
        <div className='planRecord'>
            {includeRangerName ? <p><strong>{plan.ranger.firstName} {plan.ranger.lastName}</strong></p> : null}
            <div className='container'>
                <div className='vehicles-container'>
                    {plannedVehicles.map((vehicle, index) => (
                        <div className='vehicle' key={index}>
                            <div className='identification'>
                                {/* change for picture of the vehicle based on type/ what is detail what is shown?*/}
                                {vehicle.type}
                                
                            </div>
                            {vehicle.name && !editing && (
                                <div className="tooltip">
                                    <i>i</i>
                                    <div className="tooltip-text">{vehicle.name}</div>
                                </div>
                            )}
                            {

                                hasRole("HeadOfDistrict") && editing &&
                                <button onClick={() => deleteVehicleFromPlan(vehicle.id)}>×</button>
                            }
                        </div>
                    ))}
                    { hasRole("HeadOfDistrict") && editing && (
                        <div className="add">
                            <select
                                className='dropdown'
                                value={selectedVehicleId}
                                onChange={handleVehicleSelect}
                            >
                                <option value={undefined}>Nový prostředek</option>
                                {vehicles?.map((vehicle) => (
                                    <option key={vehicle.id} value={vehicle.id}>{vehicle.name}</option>
                                ))}
                            </select>
                            <button onClick={addVehicleToPlan}>✔</button>
                        </div>
                    )}
                </div>
                {/* add not only routes, but other actions as well?*/}
                <div className='routes-container'>
                    {plannedRoutes.map((route, index) => (
                        <div className={"route priority-" + route.priority.toString()} key={index}>
                            <div className='identification'>
                                <p>{route.name}</p>
                            </div>
                            {route.controlPlace && !editing && (
                                <div className="tooltip">
                                    <i>i</i>
                                    <div className="tooltip-text">
                                        Čas: {route.controlPlace.controlTime} <br />
                                        Místo: {route.controlPlace.controlPlaceDescription}
                                    </div>
                                </div>
                            )}
                            {
                                editing &&
                                <button onClick={() => deleteRouteFromPlan(route.id)}>×</button>
                            }
                        </div>
                    ))}
                    {editing && (
                        <div className="add">
                            <select
                                className='dropdown'
                                value={selectedRouteId}
                                onChange={handleRouteSelect}
                            >
                                <option value={undefined}>Nová trasa</option>
                                {routes?.map((route) => (
                                    <option key={route.id} value={route.id} className={"priority-" + route.priority.toString()}>{route.name}</option>
                                ))}
                            </select>
                            <button onClick={addRouteToPlan}>✔</button>
                        </div>
                    )}
                </div>
            </div>
            <div className='tools'>
                <div className='edit' onClick={toggleEdit}>
                    {/*more advanced when authorization is implemented - if its the rangers or if user is the head*/}
                    {isEditable ?
                        (editing ? 'x' : '✎')
                    : null}
                </div>
            </div>
        </div>
    );
};

export default PlanRecord;