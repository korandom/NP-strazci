import './Home.css';
import React, { useState, useEffect } from 'react';
import PlanForDay from '../Components/DisplayPlan/PlanForDay';
import Calendar from '../Components/DisplayPlan/Calendar';
import { Plan, fetchPlansByDate } from '../Services/PlanService';

const Home: React.FC = () => {
    const [date, setDate] = useState<string>(() => {
        const today = new Date();
        return today.toISOString().split('T')[0];
    });

    const [plans, setPlans] = useState<Plan[]>([]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const fetchedPlans = await fetchPlansByDate(date);
                setPlans(fetchedPlans);
            } catch (error) {
                console.error(error);
            }
        };

        fetchData();
    }, [date]);

    const handleDateChange = (newDate: string) => {
        setDate(newDate);
    };

    return (
        <div className='home-container'>
            <Calendar onDateChange={handleDateChange} />
            <PlanForDay date={date} recordsData={plans} />
        </div>
    );
};

export default Home;