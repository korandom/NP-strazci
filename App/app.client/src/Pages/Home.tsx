import './Style/Home.css';
import React, { useState, useEffect, useMemo } from 'react';
import PlanForDay from '../Components/DisplayPlan/PlanForDay';
import Calendar from 'react-calendar';
import 'react-calendar/dist/Calendar.css';
import usePlans from '../Components/DataProviders/PlanDataProvider';

type ValuePiece = Date | null;
type Value = ValuePiece | [ValuePiece, ValuePiece];

const Home: React.FC = () => {
    const { plans, month, changeMonth, resetToCurrentMonth, error } = usePlans();
    const [date, setDate] = useState<Date>(new Date());

    useEffect(() => {
        resetToCurrentMonth();
    }, []);

    const dayPlans = useMemo(() => {
        const dateString = date.getFullYear() + "-" + (date.getMonth() + 1).toString().padStart(2, '0') + "-" + date.getDate().toString().padStart(2, '0');
        return plans.filter(plan => plan.date === dateString);
    }, [plans, date]);

    const handleDateChange = (date : Value)  => {
        if (date instanceof Date) {
            // if new date is from different month, change the month in context
            const selectedMonth = `${date.getFullYear()}-${(date.getMonth() + 1).toString().padStart(2, '0')}`;
            if (selectedMonth !== month) {
                changeMonth(selectedMonth);
            }
            
            setDate(date);
        }
    };

    return (
        <>
            {error ? (
                <div className="error">
                    {error.message}
                </div>
            ) : (
                <div className='home-container'>
                    <div className='calendar'>
                        <Calendar
                            onChange={handleDateChange}
                            value={date}
                            locale='cs'
                            maxDetail="month"
                            minDetail="month"
                            next2Label={null}
                            prev2Label={null}
                        />
                        </div>

                    <PlanForDay recordsData={dayPlans} />
                </div>
            )}
        </>
    );
};

export default Home;