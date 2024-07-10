import React from 'react';
import './PlanRecord.css';
import { Plan } from '../../Services/PlanService';

// Konkrétní záznam plánu jednoho strážce, bez detailù
const PlanRecord: React.FC<{ plan : Plan, includeRangerName: boolean, includeDetails: boolean }> = ({ plan, includeRangerName, includeDetails}) => {
    return (
        <div className='planRecord'>
            {includeRangerName ? <p ><strong>{plan.ranger.firstName} {plan.ranger.lastName}</strong></p> : null}
            <div className='container'>
                <div className='vehicles-container'>
                    {plan.vehicles.map((vehicle, index) => (
                        <div className='vehicle' key={index}>
                            {/* change for picture of the vehicle?*/}
                            {vehicle.type}
                            {includeDetails ? vehicle.name : null }
                        </div>
                    ))}
                </div>
                {/* add not only routes, but other actions as well?*/}
                <div className='routes-container'>
                    {plan.routes.map((route, index) => (
                        <div className='route' key={index}>
                            {route.name}
                            {includeDetails && route.controlTime && route.controlTime.trim() !== "" ? (
                                <p>{route.controlTime} - {route.controlPlaceDescription}</p>
                            ) : null}
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default PlanRecord;