import React, { useState } from 'react';
import Calendar from 'react-calendar';
import 'react-calendar/dist/Calendar.css';
import usePlans from '../../DataProviders/PlanDataProvider';
import PlansForDay from '../DisplayPlan/PlanForDay';
import './DailyPlanner.css';
import { useMediaQuery } from '../../../Util/Hooks';

type ValuePiece = Date | null;
type Value = ValuePiece | [ValuePiece, ValuePiece];

const DailyPlanner: React.FC = () => { 
    const { dateRange, changeDateOfPlans } = usePlans();
    const [date, setDate] = useState<Date>(new Date());
    const isMobile = useMediaQuery('(max-width: 560px)');
    


    const handleDateChange = (date : Value)  => {
        if (date instanceof Date) {
            if (date < dateRange.start || date > dateRange.end) {
                changeDateOfPlans(date);
            }
            setDate(date);
        }
    };

    return (
        <> 
            <div className='daily-container'>
                {isMobile ? (
                    <div>Mobile calendar</div>
                ):(
                    <div className = 'calendar'>
                        <Calendar
                            onChange = { handleDateChange }
                            value = { date }
                            locale = 'cs'
                            maxDetail = "month"
                            minDetail = "month"
                            next2Label = { null }
                            prev2Label = { null }
                        />
                    </div>
                )}
              
                <PlansForDay date={date} />
            </div>
        </>
    );
};

export default DailyPlanner;