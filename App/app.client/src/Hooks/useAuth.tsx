import { useContext } from 'react';
import { AuthContext } from '../Components/Authentication/AuthProvider';


export default function useAuth() {
    return useContext(AuthContext);
}
