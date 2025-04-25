import React from 'react';
import { useState, useEffect } from 'react';
import './PlanRecord.css';
import useDistrict from '../../../Hooks/useDistrict';
import useAuth from '../../../Hooks/useAuth';
import useSchedule from '../../../Hooks/useSchedule';
import { RangerSchedule } from '../../../Services/RangerScheduleService';
import { ReasonOfAbsence } from '../../../Services/AttendenceService';

/**
 * PlanRecord is a component for rendering a single RangerSchedule for viewing. 
 * 
 * @param param0 - props
 * @param param0[schedule] - specific rangerSchedule
 * @param param0[includeRangerName] - whether name of the ranger should be rendered as well (name is not showed in a table for every cell)
 * @param param0[isEditable] - is editing of the record allowed
 * 
 * @returns {JSX.Element} A rendered record of a rangerSchedule.
 */
const PlanRecord: React.FC<{ schedule: RangerSchedule, includeRangerName: boolean, isEditable: boolean }> = ({ schedule, includeRangerName, isEditable }) => {
    const { hasRole } = useAuth();
    const { routes, vehicles } = useDistrict();
    const { addPlannedRoute, addPlannedVehicle, removePlannedRoute, removePlannedVehicle, updateWorking, updateReasonOfAbsence } = useSchedule();

    const [editing, setEditing] = useState(false);
    const [plannedRouteIds, setPlannedRouteIds] = useState(schedule.routeIds);
    const [plannedVehicleIds, setPlannedVehicleIds] = useState(schedule.vehicleIds);

    const [working, setWorking] = useState(schedule.working);
    const [reasonOfAbsence, setReasonOfAbsence] = useState(schedule.reasonOfAbsence);

    const toggleEdit = () => {
        isEditable ? setEditing(!editing) : null;
    };

    // selecting route to add from dropdown
    const handleRouteSelect = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const selectedId = Number(event.target.value);
        if (!selectedId || isNaN(selectedId)) return;

        // reset value
        event.target.value = "";

        if (routes.some(route => route.id === selectedId)) {

            if (plannedRouteIds.some(id => selectedId === id)) {
                return;
            }

            setPlannedRouteIds([...plannedRouteIds, selectedId]);
            addPlannedRoute(schedule.date, schedule.ranger.id, selectedId);
        }

    };

    // update routeIds
    useEffect(() => {
        setPlannedRouteIds(schedule.routeIds);
    }, [schedule.routeIds]);


    const deleteRouteFromPlan = (routeId: number) => {
        setPlannedRouteIds(plannedRouteIds.filter(id => id !== routeId));
        removePlannedRoute(schedule.date, schedule.ranger.id, routeId);
    };


    // selecting vehicle to add from dropdown
    const handleVehicleSelect = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const selectedId = Number(event.target.value);
        if (!selectedId || isNaN(selectedId)) return;

        // reset value
        event.target.value = "";

        if (vehicles.some(vehicle => vehicle.id === selectedId)) {

            if (plannedVehicleIds.some(id => id === selectedId)) {
                return;
            }
                
            setPlannedVehicleIds([...plannedVehicleIds, selectedId]);
            addPlannedVehicle(schedule.date, schedule.ranger.id, selectedId);
        }
    };

    // update vehicleIds
    useEffect(() => {
        setPlannedVehicleIds(schedule.vehicleIds);
    }, [schedule.vehicleIds]);


    const deleteVehicleFromPlan = (vehicleId: number) => {
        setPlannedVehicleIds(plannedVehicleIds.filter(id => id !== vehicleId));
        removePlannedVehicle(schedule.date, schedule.ranger.id, vehicleId);
    };

    // update working localy when it changes globally
    useEffect(() => {
        setWorking(schedule.working);
    }, [schedule.working]);

    const changeWorking = (newWorking: boolean) => {
        setWorking(newWorking);
        updateWorking(schedule.date, schedule.ranger, newWorking);
    }

    useEffect(() => {
        setReasonOfAbsence(schedule.reasonOfAbsence);
    }, [schedule.reasonOfAbsence]);

    // handle selecting a reason of absence - sets it locally and updates in the global state
    const handleReasonOfAbsenceSelect = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const selectedReason = Number(event.target.value) as ReasonOfAbsence;
        setReasonOfAbsence(selectedReason);
        updateReasonOfAbsence(schedule.date, schedule.ranger, selectedReason);
    };

    return (
        <div className='planRecord'>
            {includeRangerName ? <p><strong>{schedule.ranger.firstName} {schedule.ranger.lastName}</strong></p> : null}
            <div className="workingButton">{working ? (
                <button className="positive" onClick={() => changeWorking(false)} disabled={!isEditable}><strong>✔</strong></button>
                ) : (
                    <button className="negative" onClick={() => changeWorking(true)} disabled={!isEditable}>×</button>
                )}
            </div>
            {working ? (
                <>
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
                                        {/* change for picture of the vehicle based on type/ what is detail what is shown in the future dev?*/}
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
                                        <button className="delete-source" onClick={() => deleteVehicleFromPlan(vehicle.id)}>×</button>
                                    }
                                </div>
                            )
                        })
                        }
                        { hasRole("HeadOfDistrict") && editing && (
                            <div className="add">
                                <select
                                    className='dropdown vehicles'
                                    defaultValue=""
                                    onChange={handleVehicleSelect}
                                >
                                    <option value="">Nový prostředek</option>
                                    {vehicles?.map((vehicle) => (
                                        <option key={vehicle.id} value={vehicle.id}>{vehicle.name}</option>
                                    ))}
                                </select>
                            </div>
                        )}
                    </div>
                    {/* could add not only routes, but other actions as well?*/}
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
                                        <button className="delete-source" onClick={() => deleteRouteFromPlan(route.id)}>×</button>
                                    }
                                </div>
                            )})
                        }
                        {editing && (
                            <div className="add">
                                <select
                                    className='dropdown routes'
                                    defaultValue=""
                                    onChange={handleRouteSelect}
                                >
                                    <option value="">Nová trasa</option>
                                    {routes?.map((route) => (
                                        <option key={route.id} value={route.id} className={"priority-" + route.priority.toString()}>{route.name}</option>
                                    ))}
                                </select>
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
                </>
            ) : (
                <div className="container">
                        {isEditable ? (
                            <select
                                className='dropdown absence'
                                value={reasonOfAbsence}
                                onChange={handleReasonOfAbsenceSelect}
                            >
                                {Object.values(ReasonOfAbsence)
                                    .filter(value => typeof value === "number")
                                    .map((reason) => (
                                        <option key={reason} value={reason}>
                                            {reason === ReasonOfAbsence.None ? "" : ReasonOfAbsence[reason as number]}
                                        </option>
                                    ))}
                            </select>
                        ): (
                                <div>{reasonOfAbsence === ReasonOfAbsence.None ? "" : ReasonOfAbsence[reasonOfAbsence as number]}</div>
                        )}
                    
                </div>
            )}
        </div>
    );
};

export default PlanRecord;