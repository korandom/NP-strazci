import React from 'react';
import { Ranger } from '../../Services/RangerService';

// Identifikace str�ce v tabulce pl�nova�e
const RangerCell: React.FC<{ ranger:Ranger}> = ({ ranger }) => {
    return (
        <div className='rangerCell'>
            <strong>{ranger.firstName} {ranger.lastName}</strong>
        </div>
    );
};

export default RangerCell;