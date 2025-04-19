import React, { useMemo, useState } from 'react';
import './Style/Planner.css'; 
import useSchedule from '../Components/DataProviders/ScheduleDataProvider';
import { useMediaQuery } from '../Util/Hooks';
import PlanTable from '../Components/Planner/PlanTable/PlanTable';
import DailyPlanner from '../Components/Planner/DailyPlanner/DailyPlanner';
import { formatDate, getShiftedDate } from '../Util/DateUtil';
import { useNavigate } from 'react-router-dom';
import useAuth from '../Components/Authentication/AuthProvider';



/**
 *  A React functional component, that displays the page Plans.
 * 
 *  This component allows an authorized user to view plans for the current time period and district.
 *
 *  On a mobile screen, plans are shown per day and changing days is possible.
 *  Otherwise a 14 day period is viewed and redirection to generating is possible for the head of the district
 * 
 */

const Planner: React.FC = () : JSX.Element=> {
    const { error, dateRange } = useSchedule();
    const { hasRole } = useAuth();
    const isMobile = useMediaQuery('(max-width: 560px)');
    const navigate = useNavigate();
    const [showFortnight, setShowFortnight] = useState<boolean>(()=> isMobile ? false : true)

    const toggleDaily = () => {
        if (isMobile)
            setShowFortnight(false);
        else setShowFortnight(!showFortnight);
    };
    const startOfGeneratingDate = useMemo(() => {
        return getShiftedDate(dateRange.start, 7);
    }, [dateRange]);

    const handleGenerate = () => {
        navigate(`/generovani/${formatDate(startOfGeneratingDate)}`);
    }
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
                        {showFortnight ? (
                            <>
                            <div className="range-control">
                                <button className="showFortNight" onClick={toggleDaily}>Denní plán</button>
                                {hasRole("HeadOfDistrict") &&
                                    <button className="generate" onClick={handleGenerate}>
                                        Generovat trasy na týden od {startOfGeneratingDate.getDate()}. {startOfGeneratingDate.getMonth() + 1}.
                                    </button>
                                }

                            </div>

                            <PlanTable />
                            </>
                        ) : (
                            <>
                                <div className="range-control">
                                    <button className="showFortNight" onClick={toggleDaily}>Zpět</button>
                                </div>

                                <DailyPlanner />
                            </>
                        )}
                    </>
                )}
              
            </div>
        </>
    );
}

export default Planner;