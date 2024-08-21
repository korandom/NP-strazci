import './Home.css';
import UseAuth from '../Components/Authentication/AuthProvider';
import React, { useState, useEffect } from 'react';
import PlanForDay from '../Components/DisplayPlan/PlanForDay';
import Calendar from 'react-calendar';
import { Plan, fetchPlansByDate } from '../Services/PlanService';
import 'react-calendar/dist/Calendar.css';

type ValuePiece = Date | null;
type Value = ValuePiece | [ValuePiece, ValuePiece];

const Home: React.FC = () => {
    const { districtId } = UseAuth();
    const [date, setDate] = useState<Date>(new Date());

    const [plans, setPlans] = useState<Plan[]>([]);

    useEffect(() => {
        const fetchData = async () => {
            try { 
                if (!districtId) {
                    throw Error("District is not set.");
                }
                const fetchedPlans = await fetchPlansByDate(districtId, date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate());
                setPlans(fetchedPlans);
            } catch (error) {
                console.error(error);
            }
        };

        fetchData();
    }, [date]);

    const handleDateChange = (date : Value)  => {
        if (date instanceof Date) {
            setDate(date);
        }
    };

    return (
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
            
            <PlanForDay recordsData={plans} />
        </div>
    );
};

export default Home;