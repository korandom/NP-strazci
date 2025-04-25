import { useContext } from 'react';
import { ScheduleContext } from '../Components/DataProviders/ScheduleDataProvider';

/**
 * Use Schedule data context.
 * @returns Schedule Context.
 */
export default function useSchedule() {
    return useContext(ScheduleContext);
}
