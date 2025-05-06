import { useContext } from 'react';
import { DistrictContext } from '../Providers/DataProviders/DistrictDataProvider';

/**
 * Use District data context.
 * @returns District context.
 */
export default function useDistrict() {
    return useContext(DistrictContext);
}
