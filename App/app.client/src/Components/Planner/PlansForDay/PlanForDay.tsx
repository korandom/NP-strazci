import React, { useMemo } from 'react';
import './PlanForDay.css';
import PlanRecord from '../PlanRecord/PlanRecord';
import usePlans from '../../DataProviders/ScheduleDataProvider';

/**
 * A React functional component that displays the schedule for a given day.
 * 
 * Plans are rendered using PlanRecord components.
 * 
 * @param date - Date of plans.
 * @returns {JSX.Element} A rendered list of PlanRecord components.
 */
const PlansForDay: React.FC<{ date: Date }> = ({ date }) : JSX.Element => {
    const { schedules } = usePlans();

    const dayPlans = useMemo(() => {
        const dateString = date.getFullYear() + "-" + (date.getMonth() + 1).toString().padStart(2, '0') + "-" + date.getDate().toString().padStart(2, '0');
        return schedules.filter(plan => plan.date === dateString);
    }, [schedules, date]);

    return (
        <div className="plan-for-day">
            <div className="records">
                {dayPlans.map((item, index) => (
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

export default PlansForDay;
