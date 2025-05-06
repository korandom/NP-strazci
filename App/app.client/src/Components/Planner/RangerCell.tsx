import React from 'react';
import { Ranger } from '../../Services/RangerService';

/**
 * A simple component that identifies a ranger in the header column of a PlanTable.
 * Currently only displays first and last name, but could be adapted to include more ranger information, links etc.
 * 
 * @param param0 - props for the component
 * @param param0[ranger] - Ranger
 * @returns {JSX.Element} A rendered ranger identification, used in a header cell of a PlanTable.
 */
const RangerCell: React.FC<{ ranger:Ranger}> = ({ ranger }) => {
    return (
        <div className='rangerCell'>
            <strong>{ranger.firstName} {ranger.lastName}</strong>
        </div>
    );
};

export default RangerCell;