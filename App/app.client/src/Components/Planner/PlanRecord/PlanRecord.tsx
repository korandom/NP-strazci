import React from 'react';
import { useState, useEffect } from 'react';
import './PlanRecord.css';
import { Plan } from '../../../Services/PlanService';
import useDistrict from '../../DataProviders/DistrictDataProvider';
import useAuth from '../../Authentication/AuthProvider';
import usePlans from '../../DataProviders/PlanDataProvider';

// Konkrétní záznam plánu jednoho strážce, bez detailů
const PlanRecord: React.FC<{ plan: Plan, includeRangerName: boolean, isEditable: boolean }> = ({ plan, includeRangerName, isEditable }) => {
    const { hasRole } = useAuth();
    const { routes, vehicles } = useDistrict();
    const { addPlannedRoute, addPlannedVehicle, removePlannedRoute, removePlannedVehicle } = usePlans();

    const [editing, setEditing] = useState(false);
    const [selectedRouteId, setSelectedRouteId] = useState<number | undefined>(undefined);
    const [plannedRouteIds, setPlannedRouteIds] = useState(plan.routeIds);
    const [selectedVehicleId, setSelectedVehicleId] = useState<number | undefined>(undefined);
    const [plannedVehicleIds, setPlannedVehicleIds] = useState(plan.vehicleIds);

   
    const toggleEdit = () => {
        isEditable ? setEditing(!editing) : null;
    };

    // selecting route to add from dropdown
    const handleRouteSelect = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const selectedId = Number(event.target.value);
        setSelectedRouteId(selectedId);
    };

    // update routeIds
    useEffect(() => {
        setPlannedRouteIds(plan.routeIds);
    }, [plan.routeIds]);

  
    const addRouteToPlan = () => {
        if (selectedRouteId != undefined) { 
            
            if (routes.some(route => route.id === selectedRouteId)) {

                if (plannedRouteIds.some(id => selectedRouteId === id)) {
                    return;
                }

                setPlannedRouteIds([...plannedRouteIds, selectedRouteId]);
                setSelectedRouteId(undefined); 
                addPlannedRoute(plan.date, plan.ranger.id, selectedRouteId);
            }
        }
    };

    const deleteRouteFromPlan = (routeId: number) => {
        setPlannedRouteIds(plannedRouteIds.filter(id => id !== routeId));
        removePlannedRoute(plan.date, plan.ranger.id, routeId);
    };


    // selecting vehicle to add from dropdown
    const handleVehicleSelect = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const selectedId = Number(event.target.value);
        setSelectedVehicleId(selectedId);
    };

    // update vehicleIds
    useEffect(() => {
        setPlannedVehicleIds(plan.vehicleIds);
    }, [plan.vehicleIds]);

    const addVehicleToPlan = () => {
        if (selectedVehicleId != undefined) {

            if (vehicles.some(vehicle => vehicle.id === selectedVehicleId)) {

                if (plannedVehicleIds.some(id => id === selectedVehicleId)) {
                    return;
                }

                setPlannedVehicleIds([...plannedVehicleIds, selectedVehicleId]);
                setSelectedRouteId(undefined);
                addPlannedVehicle(plan.date, plan.ranger.id, selectedVehicleId);
            }
        }
    };

    const deleteVehicleFromPlan = (vehicleId: number) => {
        setPlannedVehicleIds(plannedVehicleIds.filter(id => id !== vehicleId));
        removePlannedVehicle(plan.date, plan.ranger.id, vehicleId);
    };

    return (
        <div className='planRecord'>
            {includeRangerName ? <p><strong>{plan.ranger.firstName} {plan.ranger.lastName}</strong></p> : null}
            <div className={editing? 'editing container': 'container'}>
                <div className='planned-items-container'>
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
                <div className='planned-items-container'>
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
                    {isEditable ?
                        (editing ? 'x' : '✎')
                    : null}
                </div>
            </div>
        </div>
    );
};

export default PlanRecord;