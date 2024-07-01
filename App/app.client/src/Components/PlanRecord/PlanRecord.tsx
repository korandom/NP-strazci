import React from 'react';
import './PlanRecord.css';
import { Plan } from '../../Services/PlanService';

// Konkr�tn� z�znam pl�nu jednoho str�ce, bez detail�
const PlanRecord: React.FC<{ plan : Plan }> = ({ plan }) => {
    return (
        <div className='planRecord'>
            <p ><strong>{plan.ranger.firstName} {plan.ranger.lastName}</strong></p>
            <div className='container'>
                <div className='vehicles-container'>
                    {plan.vehicles.map((vehicle, index) => (
                        <div className='vehicle' key={index}>
                            {/* change for picture of the vehicle?*/}
                            {vehicle.name}
                        </div>
                    ))}
                </div>
                {/* add not only routes, but other actions as well?*/}
                <div className='routes-container'>
                    {plan.routes.map((route, index) => (
                        <div className='route' key={index}>
                            {route.name}
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default PlanRecord;