import React, { useState, useEffect, useMemo } from 'react';
import { Plan, fetchPlansByDateRange } from '../Services/PlanService';
import { Ranger, fetchRangersByDistrict } from '../Services/RangerService';
import PlanRecord from '../Components/PlanRecord/PlanRecord'
import RangerCell from '../Components/Planner/RangerCell';
import './Planner.css';

const Planner: React.FC = () => {
    const [month, setMonth] = useState<string>(() => {
        const currentDate = new Date();
        const currentMonth = currentDate.getMonth() + 1; 
        return `${currentDate.getFullYear()}-${currentMonth.toString().padStart(2, '0')}`;
    });

    const calculateMonthRange = (month: string): { startDate: string, endDate: string } => {
        const start = new Date(month + '-01');
        const end = new Date(start.getFullYear(), start.getMonth() + 1, 1);

        const startDate = start.toISOString().split('T')[0];
        const endDate = end.toISOString().split('T')[0];
        return { startDate, endDate };
    }

    const [monthRange, setMonthRange] = useState<{ startDate: string, endDate: string }>(calculateMonthRange(month));
    const [plans, setPlans] = useState<Plan[]>([]);
    const [rangers, setRangers] = useState<Ranger[]>([]);
    const nameOfDays: string[] = ["Ne","Po", "Út", "St", "Čt", "Pá", "So"];


    // TODO get district number from user information linked to a ranger instance
    useEffect(() => {
        const fetchRangers = async () => {
            try {
                const rangersData = await fetchRangersByDistrict("2");
                setRangers(rangersData);
            } catch (error) {
                console.error(error);
            }
        };

        fetchRangers();
    }, []);

    const changeMonth = (event: React.ChangeEvent<HTMLInputElement>) => {
        const selectedMonth = event.target.value;
        setMonth(selectedMonth);

        if (selectedMonth) {
            setMonthRange(calculateMonthRange(selectedMonth));
        }
    };

    useEffect(() => {
        if (monthRange.endDate && monthRange.startDate) {
            const fetchPlans = async () => {
                try {
                    const fetchedPlans = await fetchPlansByDateRange(monthRange.startDate, monthRange.endDate);
                    setPlans(fetchedPlans);
                } catch (error) {
                    console.error(error);
                }
            };
            fetchPlans();
        }

    }, [monthRange]);

    const generateDateRange = (start: string, end: string): Date[] => {
        const endDate = new Date(end);
        const dateArray = [];
        for (let date = new Date(start); date <= endDate; date.setDate(date.getDate() + 1)) {
            dateArray.push(new Date(date));
        }
        return dateArray;
    };

    const dateArray = useMemo(() => {
        return monthRange.startDate && monthRange.endDate ? generateDateRange(monthRange.startDate, monthRange.endDate) : [];
    }, [monthRange]);

    const formatDate = (date: Date): string => {
        return date.toISOString().split('T')[0];
    };

    return (
        <div className="planner-container">
            <div className="month-picker">
                <label htmlFor="monthPicker">Vyberte Měsíc: </label>
                <input id="monthPicker" aria-label="Měsíc" type="month" value={month} onChange={changeMonth} lang="cs"/>
            </div> 
            <div className="table-container">
                {dateArray.length > 0 && (
                    <table className="plan-table">
                        <thead>
                            <tr>
                                <th className="sticky"></th>
                                {dateArray.map((date, index) => {
                                    const Weekend = date.getDay() == 0 || date.getDay() == 6;
                                    return (
                                        <th className={Weekend ? "weekend date-header" : "date-header"} key={index}>
                                            <div>
                                                {nameOfDays[date.getDay()] }
                                            </div>
                                            <div>
                                                {date.getDate()}.{(date.getMonth() + 1)}.
                                            </div>
                                        </th>
                                    );
                                })}
                            </tr>
                        </thead>
                        <tbody>
                            {rangers.map(ranger => (
                                <tr key={ranger.id}>
                                    <td className="sticky">
                                        <RangerCell ranger={ranger} />
                                    </td>
                                    {dateArray.map((date, index) => {
                                        const Weekend = date.getDay() == 0 || date.getDay() == 6;
                                        const stringDate = formatDate(date);
                                        const plan = plans.find(plan => (plan.ranger.id === ranger.id && plan.date === stringDate));
                                        return (
                                            
                                            <td className={Weekend ? "weekend plan" : "plan"} key={index}>
                                                {plan ? (
                                                    <PlanRecord
                                                        plan={plan}
                                                        isEditable={true}
                                                        includeRangerName={false}
                                                    />
                                                ) : (
                                                        <div className="add">+</div>
                                                )}
                                            </td>
                                        );
                                    })}
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </div>
        </div>
    );
}

export default Planner;