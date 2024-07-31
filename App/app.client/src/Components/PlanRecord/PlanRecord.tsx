import React from 'react';
import { useState } from 'react';
import './PlanRecord.css';
import { Plan, addRoute, removeRoute } from '../../Services/PlanService';
import routeService from '../../Services/RouteService'

// Konkrétní záznam plánu jednoho strážce, bez detailů
const PlanRecord: React.FC<{ plan: Plan, includeRangerName: boolean, isEditable: boolean }> = ({ plan, includeRangerName, isEditable }) => {
    // state managment for showing details
    const [includeDetails, setIncludeDetails] = useState(false);
    const toggleDetails = () => {
        setIncludeDetails(!includeDetails);
    };

    // state managment for editing
    const [editing, setEditing] = useState(false);
    const toggleEdit = () => {
        isEditable ? setEditing(!editing) : null;
    }

    // state managment for routes
    const routes = routeService.getRoutes();
    const [selectedRouteId, setSelectedRouteId] = useState<number|undefined>(undefined);
    const handleRouteSelect = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const selectedId = Number(event.target.value);
        setSelectedRouteId(selectedId);
    };
    const [plannedRoutes, setPlannedRoutes] = useState(plan.routes);
    const addRouteToPlan = () => {
        if (selectedRouteId != undefined) {
            const selectedRoute = routes.find(route => route.id === selectedRouteId);
            if (selectedRoute) {
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
    return (
        <div className='planRecord'>
            {includeRangerName ? <p><strong>{plan.ranger.firstName} {plan.ranger.lastName}</strong></p> : null}
            <div className='container'>
                <div className='vehicles-container'>
                    {plan.vehicles.map((vehicle, index) => (
                        <div className='vehicle' key={index}>
                            {/* change for picture of the vehicle?*/}
                            {vehicle.type}
                            {includeDetails ? vehicle.name : null }
                        </div>
                    ))}
                    {
                        editing ?
                            <div className="add-vehicle">
                                
                            </div>
                        : null
                    }
                </div>
                {/* add not only routes, but other actions as well?*/}
                <div className='routes-container'>
                    {plannedRoutes.map((route, index) => (
                        <div className='route' key={index}>
                            <p>{route.name}</p>
                            {includeDetails && route.controlTime && route.controlTime.trim() !== "" ? (
                                <p>{route.controlTime}  {route.controlPlaceDescription}</p>
                            ) : null}

                            {
                                editing &&
                                <button onClick={() => deleteRouteFromPlan(route.id)}>×</button>
                            }
                        </div>
                    ))}
                    {editing && (
                        <div className="add-route">
                            <select
                                className='route-dropdown'
                                value={selectedRouteId}
                                onChange={handleRouteSelect}
                            >
                                <option value={undefined}>Nová trasa</option>
                                {routes.map((route) => (
                                    <option key={route.id} value={route.id}>{route.name}</option>
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
                    {isEditable && !plan.locked ?
                        editing ? 'x' : '✎'
                    : null}
                </div>
                <div className='expand-details' onClick={toggleDetails}>
                    {includeDetails ? '⇱' : '⇲' }
                </div>
            </div>
        </div>
    );
};

export default PlanRecord;