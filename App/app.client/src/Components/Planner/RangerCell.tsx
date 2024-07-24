import React from 'react';
import { Ranger } from '../../Services/RangerService';

// Identifikace strážce v tabulce plánovaèe
const RangerCell: React.FC<{ ranger:Ranger}> = ({ ranger }) => {
    return (
        <div className='rangerCell'>
            <strong>{ranger.firstName} {ranger.lastName}</strong>
        </div>
    );
};

export default RangerCell;