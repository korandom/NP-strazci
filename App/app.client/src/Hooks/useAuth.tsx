import { useContext } from 'react';
import { AuthContext } from '../Providers/Authentication/AuthProvider';

/**
 * Use Authentication Hook.
 * @returns Context of Authentication.
 */
export default function useAuth() {
    return useContext(AuthContext);
}
