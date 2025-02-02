import React, { useEffect, useMemo, useState } from 'react';
import usePlans from '../../DataProviders/ScheduleDataProvider';
import PlansForDay from '../PlansForDay/PlanForDay';
import './DailyPlanner.css';
import { generateDateRange, getShiftedDate, nameOfDaysCZ } from '../../../Util/DateUtil';



/**
 *  A React functional component, that displays plans for selected date.
 * 
 *  Is used as the only way to view plans on mobile screens.
 *  Consists of a date picker, for selecting a date and a component PlansForDay takes care of the plans display.
 *  In the date picker, currently selected date is highlighted with a circle and today is marked with red color, weekends are marked with dull grey.
 */
const DailyPlanner: React.FC = () => { 
    const { dateRange, weekForward, weekBack, loading } = usePlans();
    const [selectedDate, setDate] = useState<Date>(new Date());


    useEffect(() => {
        const today = new Date();
        if (today > dateRange.start && today < getShiftedDate(dateRange.start, 6)) {
            setDate(today);
        }
        else  setDate(dateRange.start);
    }, [dateRange])

    const moveRangeBack = () => {
        weekBack();
        setDate(dateRange.start);
    }

    const moveRangeForward = () => {
        weekForward();
        setDate(dateRange.start);
    }

    const getDateClass = (date: Date): string => {
        const today = date.toDateString() == new Date().toDateString();
        const current = date.toDateString() == selectedDate.toDateString();
        if (today && current) {
            return "today current-date date";
        }
        if (today) {
            return "today date";
        }
        if (current) {
            return "current-date date"
        }
        return "date";
    }

    const dateArray = useMemo(() => {
        return dateRange.start && dateRange.end ? generateDateRange(dateRange.start, getShiftedDate(dateRange.start,6)) : [];
    }, [dateRange]);

    return (
        <> 
            <div className='daily-container'>
                <div className="range-movement">
                    <button onClick={moveRangeBack}>◀</button>
                    <button onClick={moveRangeForward}>▶</button>
                </div>
                <div className='date-picker'>
                    
                    {dateArray.map((date, index) => {
                        const Weekend = date.getDay() == 0 || date.getDay() == 6;
                        return (
                            <div className={Weekend ? "date-pick-element weekend" :"date-pick-element"} key={index}>
                                
                                <div className="dayOfWeek">
                                    {nameOfDaysCZ[date.getDay()]}
                                </div>
                                <button className={getDateClass(date)} onClick={() => setDate(date)}>
                                    <div>
                                        {date.getDate()}
                                    </div>
                                </button>
                            </div>
                        );
                    })}
                </div>

                {loading && (
                    <div className="loading-over">
                        <div className="loading-text">Loading...</div>
                    </div>
                )}

                {!loading && (
                    <PlansForDay date={selectedDate} />
                )}
            </div>
        </>
    );
};

export default DailyPlanner;