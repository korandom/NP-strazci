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
    const [plannedRouteIds, setPlannedRoutesIds] = useState(plan.routes.map(route => route.id));
    useEffect(() => {
        setPlannedRoutesIds(plan.routes.map(route => route.id));
    }, [plan.routes]);

    const addRouteToPlan = () => {
        if (selectedRouteId != undefined) { 
            
            if (routes.some(route => route.id === selectedRouteId)) {

                if (plannedRouteIds.some(id => selectedRouteId === id)) {
                    return;
                }

                setPlannedRoutesIds([...plannedRouteIds, selectedRouteId]);
                setSelectedRouteId(undefined); 
                addRoute(plan.date, plan.ranger.id, selectedRouteId);
            }
        }
    };
    const deleteRouteFromPlan = (routeId :number) => {
        setPlannedRoutesIds(plannedRouteIds.filter(id => id !== routeId));
        removeRoute(plan.date, plan.ranger.id, routeId);
    }

    // state managment for vehicles
    const [selectedVehicleId, setSelectedVehicleId] = useState<number | undefined>(undefined);
    const handleVehicleSelect = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const selectedId = Number(event.target.value);
        setSelectedVehicleId(selectedId);
    };

    const [plannedVehicleIds, setPlannedVehicleIds] = useState(plan.vehicles.map(vehicle => vehicle.id));        ;
    useEffect(() => {
        setPlannedVehicleIds(plan.vehicles.map(vehicle=> vehicle.id));
    }, [plan.vehicles]);

    const addVehicleToPlan = () => {
        if (selectedVehicleId != undefined) {

            if (vehicles.some(vehicle => vehicle.id === selectedVehicleId)) {

                if (plannedVehicleIds.some(id => id === selectedVehicleId)) {
                    return;
                }

                setPlannedVehicleIds([...plannedVehicleIds, selectedVehicleId]);
                setSelectedRouteId(undefined);
                addVehicle(plan.date, plan.ranger.id, selectedVehicleId);
            }
        }
    }
    const deleteVehicleFromPlan = (vehicleId: number) => {
        setPlannedVehicleIds(plannedVehicleIds.filter(id => id !== vehicleId));
        removeVehicle(plan.date, plan.ranger.id, vehicleId);
    }

    return (
        <div className='planRecord'>
            {includeRangerName ? <p><strong>{plan.ranger.firstName} {plan.ranger.lastName}</strong></p> : null}
            <div className='container'>
                <div className='vehicles-container'>
                    {plannedVehicleIds.map((id, index) => {
                        const vehicle = vehicles.find(v => v.id === id);
                        if (!vehicle) {
                            return null;
                        }
                        return (
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
                        )
                    })
                    }
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
                    {plannedRouteIds.map((id, index) => {
                        const route = routes.find(route => route.id === id);

                        if (!route) {
                            return null;
                        }
                        
                        return (

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
                        )})
                    }
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