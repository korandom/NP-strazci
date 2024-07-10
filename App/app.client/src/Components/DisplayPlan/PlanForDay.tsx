import React from 'react';
import './PlanForDay.css';
import PlanRecord from '../PlanRecord/PlanRecord';
import { Plan } from '../../Services/PlanService';

interface PlanForDayProps {
    recordsData: Plan[];
}

// Zobrazení plánu strážcù na urèitý den
const PlanForDay: React.FC<PlanForDayProps> = ({ recordsData }) => {
    return (
        <div className="plan-for-day">
            <div className="records">
                {recordsData.map((item, index) => (
                    <PlanRecord
                        key={index}
                        plan={item}
                        includeDetails={false}
                        includeRangerName={true}
                    />
                ))}
            </div>
        </div>
    );
};

export default PlanForDay;
