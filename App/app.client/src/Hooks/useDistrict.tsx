import { useContext } from 'react';
import { DistrictContext } from '../Components/DataProviders/DistrictDataProvider';


export default function useDistrict() {
    return useContext(DistrictContext);
}
