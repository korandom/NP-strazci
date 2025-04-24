import { useContext } from 'react';
import { ScheduleContext } from '../Components/DataProviders/ScheduleDataProvider';


export default function useSchedule() {
    return useContext(ScheduleContext);
}
