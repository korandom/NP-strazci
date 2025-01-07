import React from 'react';
import './PlanForDay.css';
import PlanRecord from '../PlanRecord/PlanRecord';
import { Plan } from '../../Services/PlanService';

interface PlanForDayProps {
    recordsData: Plan[];
}

/**
 * A React functional component that displays the plans given in props.
 * 
 * Plans are rendered using PlanRecord components.
 * 
 * @component
 * @param {PlanForDayProps} props - The props for the PlanForDay component, containing plans.
 * @returns {JSX.Element} A rendered list of PlanRecord components.
 */
const PlanForDay: React.FC<PlanForDayProps> = ({ recordsData }): JSX.Element => {
    return (
        <div className="plan-for-day">
            <div className="records">
                {recordsData.map((item, index) => (
                    <PlanRecord
                        key={index}
                        plan={item}
                        isEditable={false}
                        includeRangerName={true}
                    />
                ))}
            </div>
        </div>
    );
};

export default PlanForDay;
