import React, { useEffect, useMemo } from 'react';
import UseAuth from '../Components/Authentication/AuthProvider';
import PlanRecord from '../Components/PlanRecord/PlanRecord'
import RangerCell from '../Components/Planner/RangerCell';
import './Style/Planner.css'; 
import useDistrict from '../Components/DataProviders/DistrictDataProvider';
import usePlans from '../Components/DataProviders/PlanDataProvider';

const Planner: React.FC = () => {
    const { hasRole, user } = UseAuth();
    const { rangers, locks, addLock, removeLock } = useDistrict();
    const { plans, month, monthRange, resetToCurrentMonth, changeMonth, error } = usePlans();

    const nameOfDays: string[] = ["Ne", "Po", "Út", "St", "Čt", "Pá", "So"];

    useEffect(() => {
        resetToCurrentMonth();
    }, []);

    const pickMonth = (event: React.ChangeEvent<HTMLInputElement>) => {
        const selectedMonth = event.target.value;
        changeMonth(selectedMonth);
    };

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

    const generateLocksArray = (): boolean[] => {
        const locksArray = dateArray.map((date) => { return date < new Date || locks.some(l => l.date == formatDate(date)) });
        return locksArray;
    };

    const isLockedArray = useMemo(() => {
        return generateLocksArray()
    }, [locks, dateArray]);

    return (
        <>
            {error ? (
                <div className="error">
                    {error.message}
                </div>
            ) : (

                <div className="planner-container">
                    <div className="month-picker">
                        <label htmlFor="monthPicker">Vyberte Měsíc: </label>
                        <input id="monthPicker" aria-label="Měsíc" type="month" value={month} onChange={pickMonth} lang="cs" />
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
                                                    {hasRole("HeadOfDistrict") && (date > new Date) &&
                                                        <div className="lock" >
                                                            { isLockedArray[index] ?
                                                                <button onClick={() => removeLock(formatDate(date))}>🔒</button>
                                                             :
                                                                <button onClick={() => addLock(formatDate(date))}>🔓</button>
                                                            }
                                                        </div>
                                                    }
                                                    <div>
                                                        {nameOfDays[date.getDay()]}
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

                                        const isheadOfDistrict = hasRole("HeadOfDistrict");
                                        const isOwner = user?.rangerId == ranger.id;

                                        return (
                                            <tr key={ranger.id}>
                                                <td className="sticky">
                                                    <RangerCell ranger={ranger} />
                                                </td>

                                                {dateArray.map((date, index) => {
                                                    const Weekend = date.getDay() == 0 || date.getDay() == 6;
                                                    const stringDate = formatDate(date);
                                                    const plan = plans.find(p => (p.ranger.id === ranger.id && p.date === stringDate));
                                                    return (

                                                        <td className={Weekend ? "weekend plan" : "plan"} key={index}>
                                                            <PlanRecord
                                                                plan={plan ? plan : { date: stringDate, ranger: ranger, routeIds: [], vehicleIds: [] }}
                                                                isEditable={isheadOfDistrict||(isOwner && !isLockedArray[index])}
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
            )}
        </>
    );
}

export default Planner;