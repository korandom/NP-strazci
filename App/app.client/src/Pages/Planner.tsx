import React, { useEffect, useMemo, useState } from 'react';
import './Style/Planner.css'; 
import useSchedule from '../Components/DataProviders/ScheduleDataProvider';
import { useMediaQuery } from '../Util/Hooks';
import PlanTable from '../Components/Planner/PlanTable/PlanTable';
import DailyPlanner from '../Components/Planner/DailyPlanner/DailyPlanner';
import { getShiftedDate, nameOfMonthsCZ } from '../Util/DateUtil';



/**
 *  A React functional component, that displays the page Plans.
 * 
 *  This component allows an authorized user to view plans for the current time period and district.
 *  The user can change the time period viewed.
 *  On a mobile screen, plans are shown per day and changing days is possible.
 *  Otherwise a 14 day period is viewed and paging +/- a week is possible.
 *  User authorized as head of district can generate new plans and manually manage them.
 * 
 */

const Planner: React.FC = () : JSX.Element=> {
    const { resetSchedules, error, weekBack, weekForward, dateRange } = useSchedule();
    const isMobile = useMediaQuery('(max-width: 560px)');
    const [showFortnight, setShowFortnight] = useState<boolean>(()=> isMobile ? false : true)

    useEffect(() => {
        resetSchedules();
    }, []);


    const toggleDaily = () => {
        if (isMobile)
            setShowFortnight(false);
        else setShowFortnight(!showFortnight);
    };

    const decidingDate = useMemo(() => {
        return getShiftedDate(dateRange.start, 6);
    }, [dateRange]);
    return (
        <>
            {error &&(
                <div className="error">
                    {error.message}
                </div>
            )}


            <div className="planner-container">
                {isMobile ? (
                    <DailyPlanner />
                ) : (
                        <>
                            <div className="control-day">
                                <button className="showFortNight" onClick={toggleDaily}>
                                    {!showFortnight ? "Zobrazit 14ti denní plán" : "Zobrazit detail dne"}
                                </button>
                            </div>
                        
                        { showFortnight ? (
                            <>
                                <div className="range-control">
                                        <button onClick={weekBack}>Předešlý</button>
                                        <strong className="month-label">{nameOfMonthsCZ[decidingDate.getMonth()]} {decidingDate.getFullYear()}</strong>
                                    <button onClick={weekForward}>Další</button>
                                </div>

                                <PlanTable />
                            </>
                        ) : (
                            <DailyPlanner />
                        )}
                    </>
                )}
              
            </div>
        </>
    );
}

export default Planner;