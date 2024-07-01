import React from 'react';
import './PlanForDay.css';
import PlanRecord from '../PlanRecord/PlanRecord';
import { Plan } from '../../Services/PlanService';

interface PlanForDayProps {
    date: string;
    recordsData: Plan[];
}

// Zobrazen� pl�nu str�c� na ur�it� den
const PlanForDay: React.FC<PlanForDayProps> = ({ date, recordsData }) => {
    return (
        <div className="plan-for-day">
            <div className="date-header">
                {date}
            </div>
            <div className="records">
                {recordsData.map((item, index) => (
                    <PlanRecord
                        key={index}
                        plan={item}
                    />
                ))}
            </div>
        </div>
    );
};

export default PlanForDay;
