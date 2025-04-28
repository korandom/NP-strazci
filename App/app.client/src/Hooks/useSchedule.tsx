import { useContext } from 'react';
import { ScheduleContext } from '../Providers/DataProviders/ScheduleDataProvider';

/**
 * Use Schedule data context.
 * @returns Schedule Context.
 */
export default function useSchedule() {
    return useContext(ScheduleContext);
}
