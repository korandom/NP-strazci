import React, { useEffect, useMemo, useState } from 'react';
import UseAuth from '../Components/Authentication/AuthProvider';
import PlanRecord from '../Components/PlanRecord/PlanRecord'
import RangerCell from '../Components/Planner/RangerCell';
import './Style/Planner.css'; 
import useDistrict from '../Components/DataProviders/DistrictDataProvider';
import usePlans from '../Components/DataProviders/PlanDataProvider';
import { formatDate } from '../Util/DateUtil';



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
    const { hasRole} = UseAuth();
    const { rangers } = useDistrict();
    const { plans, dateRange,resetPlans, error, loading, weekBack, weekForward } = usePlans();
    const [daily, setDaily] = useState<boolean>();
    const [isMobile, setIsMobile] = useState<boolean>();

    

    const nameOfDaysCZ: string[] = ["Ne", "Po", "Út", "St", "Čt", "Pá", "So"];

    useEffect(() => {
        resetPlans();
    }, []);

    const isMobileCheck = (): boolean => {
        return window.matchMedia('(max-width: 560px)').matches;
    };

    const generateDateRange = (start: Date, end: Date): Date[] => {
        const dateArray = [];
        for (let date = new Date(start); date <= end; date.setDate(date.getDate() + 1)) {
            dateArray.push(new Date(date));
        }
        return dateArray;
    };

    const dateArray = useMemo(() => {
        return dateRange.start && dateRange.end ? generateDateRange(dateRange.start, dateRange.end) : [];
    }, [dateRange]);


    return (
        <>
            {error &&(
                <div className="error">
                    {error.message}
                </div>
            )}

            {loading && (
                <div className="loading-over">
                   <div className="loading-text">Loading...</div>
                </div>
            )}

            {!loading && (

                <div className="planner-container">
                    <button></button>
                    <div className="range-control">
                        <button onClick={weekBack}>Předešlý</button>

                        <button onClick={weekForward}>Další</button>
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
                                                        {nameOfDaysCZ[date.getDay()]}
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
                                                                isEditable={isheadOfDistrict}
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