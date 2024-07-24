import React from 'react';
import { useState } from 'react';
import './PlanRecord.css';
import { Plan } from '../../Services/PlanService';

// Konkrétní záznam plánu jednoho strážce, bez detailů
const PlanRecord: React.FC<{ plan: Plan, includeRangerName: boolean, isEditable: boolean }> = ({ plan, includeRangerName, isEditable }) => {
    const [includeDetails, setIncludeDetails] = useState(false);
    const toggleDetails = () => {
        setIncludeDetails(!includeDetails);
    };
    const [editing, setEditing] = useState(isEditable ? true : false);

    const toggleEdit = () => {
        isEditable ? setEditing(!editing) : null;
    }
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
                    {
                        editing ?
                            <div className="add-vehicle">
                                
                            </div>
                        : null
                    }
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
            <div className='tools'>
                <div className='edit' onClick={toggleEdit}>
                    {/*more advanced when authorization is implemented - if its the rangers or if user is the head*/}
                    {isEditable && !plan.locked ?
                         editing ? '✎' : 'x'
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