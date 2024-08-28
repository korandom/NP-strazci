import React, { useState, useEffect, useMemo } from 'react';
import UseAuth from '../Components/Authentication/AuthProvider';
import { Plan, fetchPlansByDateRange } from '../Services/PlanService';
import PlanRecord from '../Components/PlanRecord/PlanRecord'
import RangerCell from '../Components/Planner/RangerCell';
import './Style/Planner.css';
import useDistrict from '../Components/DistrictContext/DistrictDataProvider';

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
    const nameOfDays: string[] = ["Ne","Po", "Út", "St", "Čt", "Pá", "So"];


    const { authorizedEdit } = UseAuth();
    const { district, rangers } = useDistrict();

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
                    if (!district) {
                        throw new Error("District is not set.");
                    }

                    const fetchedPlans = await fetchPlansByDateRange(district.id, monthRange.startDate, monthRange.endDate);
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
                            {rangers?.map(ranger => {

                                const editable = authorizedEdit(ranger);
                                return (
                                    <tr key={ranger.id}>
                                        <td className="sticky">
                                            <RangerCell ranger={ranger} />
                                        </td>

                                        {dateArray.map((date, index) => {
                                            const Weekend = date.getDay() == 0 || date.getDay() == 6;
                                            const stringDate = formatDate(date);
                                            const plan = plans.find(p => (p.ranger.id === ranger.id && p.date === stringDate));
                                            const locked = plans.find(p => p.date === stringDate)?.locked ?? false;
                                            return (

                                                <td className={Weekend ? "weekend plan" : "plan"} key={index}>
                                                    <PlanRecord
                                                        plan={plan ? plan : { date: stringDate, ranger: ranger, routes: [], vehicles: [], locked: locked }}
                                                        isEditable= {editable} 
                                                        includeRangerName={false}
                                                    />
                                                </td>
                                            );
                                        })}
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                )}
            </div>
        </div>
    );
}

export default Planner;