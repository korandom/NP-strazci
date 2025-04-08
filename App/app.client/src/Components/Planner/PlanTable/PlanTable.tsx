import { useMemo } from "react";
import useAuth from "../../Authentication/AuthProvider";
import useDistrict from "../../DataProviders/DistrictDataProvider";
import useSchedule from "../../DataProviders/ScheduleDataProvider";
import { formatDate, generateDateRange, nameOfDaysCZ } from "../../../Util/DateUtil";
import RangerCell from "../RangerCell";
import PlanRecord from "../PlanRecord/PlanRecord";
import { ReasonOfAbsence } from "../../../Services/AttendenceService";


/**
 *  A React functional component, that displays a table of all plans within active range for all rangers in current district.
 * 
 *  This component uses RangerCell components to display ranger headers and PlanRecord Components to display plans.
 *  User with a HeadOfDistrict Role can make edits to the via the PlanRecord components.
 * 
 */
const PlanTable: React.FC = () : JSX.Element => {
    const { user, hasRole } = useAuth();
    const { rangers, locks, removeLock, addLock  } = useDistrict();
    const { schedules, dateRange } = useSchedule();

    const dateArray = useMemo(() => {
        return dateRange.start && dateRange.end ? generateDateRange(dateRange.start, dateRange.end) : [];
    }, [dateRange]);

    const generateLocksArray = (): boolean[] => {
        const locksArray = dateArray.map((date) => { return date < new Date || locks.some(l => l.date == formatDate(date)) });
        return locksArray;
    };

    const isLockedArray = useMemo(() => {
        return generateLocksArray()
    }, [locks, dateArray]);

    return (
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
                                                {isLockedArray[index] ?
                                                    <button onClick={() => removeLock(formatDate(date))}>🔒</button>
                                                    :
                                                    <button onClick={() => addLock(formatDate(date))}>🔓</button>
                                                }
                                            </div>
                                        }
                                        <div className="dayOfWeek">
                                            {nameOfDaysCZ[date.getDay()]}
                                        </div>
                                        <div className="date">
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
                                    {
                                        dateArray.map((date, index) => {
                                            const Weekend = date.getDay() == 0 || date.getDay() == 6;
                                            const stringDate = formatDate(date);
                                            const schedule = schedules.find(p => (p.ranger.id === ranger.id && p.date === stringDate));
                                            return (

                                                <td className={Weekend ? "weekend plan" : "plan"} key={index}>
                                                    <PlanRecord
                                                        schedule={schedule ? schedule : { date: stringDate, ranger: ranger, routeIds: [], vehicleIds: [], working: false, from: null, reasonOfAbsence: ReasonOfAbsence.None }}
                                                        isEditable={isheadOfDistrict || (isOwner && !isLockedArray[index])}
                                                        includeRangerName={false}
                                                    />
                                                </td>
                                            );
                                        })
                                    }
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
            )}
        </div>
    );
}
export default PlanTable;