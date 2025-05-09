﻿import React, { useMemo } from 'react';
import './PlanForDay.css';
import PlanRecord from '../PlanRecord/PlanRecord';
import useSchedule from '../../../Hooks/useSchedule';
import { formatDate } from '../../../Util/DateUtil';
import useDistrict from '../../../Hooks/useDistrict';
import { ReasonOfAbsence } from '../../../Services/AttendenceService';
import useAuth from '../../../Hooks/useAuth';
import { Ranger } from '../../../Services/RangerService';
import { RangerSchedule } from '../../../Services/RangerScheduleService';

/**
 * A React functional component that displays a list of schedules for a given date.
 * 
 * Plans are rendered using PlanRecord components.
 * 
 * @param date - Date of plans.
 * @param onlyWorking - When true, displays only schedules of rangers that are working the given date.
 * @returns {JSX.Element} A rendered list of PlanRecord components.
 */
const PlansForDay: React.FC<{ date: Date, onlyWorking: boolean}> = ({ date, onlyWorking }) : JSX.Element => {
    const { schedules } = useSchedule();
    const { rangers, locks } = useDistrict();
    const { user , hasRole } = useAuth();

    const dayPlans = useMemo(() => {
        const dateString = formatDate(date);
        return schedules.filter(plan => plan.date === dateString);
    }, [schedules, date]);

    const filteredDayPlans = useMemo(() => {
        if (onlyWorking) {
            return dayPlans.filter(plan => plan.working); 
        }
        return dayPlans; 
    }, [dayPlans, onlyWorking]);


    const getRangersSchedule = (ranger: Ranger): RangerSchedule | undefined => {
        const schedule = filteredDayPlans.find(schedule => schedule.ranger.id === ranger.id);
        if (schedule == undefined && !onlyWorking) {
            return { date: formatDate(date), ranger: ranger, routeIds: [], vehicleIds: [], working: false, from: null, reasonOfAbsence: ReasonOfAbsence.None }
        }
        return schedule;
    }

    const getUserRangerSchedule = (ranger: Ranger): RangerSchedule => {
        const schedule = dayPlans.find(schedule => schedule.ranger.id === ranger.id);
        if (schedule == undefined) {
            return { date: formatDate(date), ranger: ranger, routeIds: [], vehicleIds: [], working: false, from: null, reasonOfAbsence: ReasonOfAbsence.None }
        }
        return schedule;
    }

    const isLocked = useMemo(() => {
        return date < new Date || locks.some(l => l.date == formatDate(date))
    },[locks, date])

    return (
        <div className="plan-for-day">
            <div className="records">
                {rangers.map((ranger, index) => {
                    const isOwner = user?.rangerId == ranger.id;
                    const schedule = isOwner ? getUserRangerSchedule(ranger) : getRangersSchedule(ranger);
                    const isheadOfDistrict = hasRole("HeadOfDistrict");
                    if (schedule != undefined) {
                        return (
                            <PlanRecord
                                key={index}
                                schedule={schedule}
                                isEditable={isheadOfDistrict||(isOwner && !isLocked)}
                                includeRangerName={true}
                            />
                        )
                    }
                })}
            </div>
        </div>
    );
};

export default PlansForDay;
