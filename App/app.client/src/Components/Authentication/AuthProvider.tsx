import { User, signIn, signOut } from '../../Services/UserService';
import { Ranger, getCurrentRanger } from '../../Services/RangerService';
import  { createContext, useContext, ReactNode, useState, useMemo} from 'react';
import { useNavigate } from 'react-router-dom';
import useDistrict from '../DistrictContext/DistrictDataProvider';

interface AuthContextType {
    user: User | undefined,
    loading: boolean,
    error: any,
    signin: (email: string, password: string) => void,
    signout: () => void,
    authorizedEdit: (planOwner: Ranger) => boolean,
    hasRole: (role: string) => boolean
 }

const AuthContext = createContext<AuthContextType>({} as AuthContextType);

export const AuthProvider = ({ children }: { children: ReactNode }): JSX.Element => {
    const [user, setUser] = useState<User | undefined>(undefined);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<any>();
    const navigate = useNavigate();
    const { assignDistrict, clearDistrict, district } = useDistrict();

    //TODO clearing errors - manual by calling a function/ when changing routes/ when unmounting?

    //TODO get current user from database if there is an active session/4
    async function signin(email: string, password: string) {
        setLoading(true);
        try {
            const user = await signIn(email, password);
            const ranger = await getCurrentRanger();
            if (ranger != undefined) {
                await assignDistrict(ranger.districtId);
            }
            setError(null);
            setUser(user);
            navigate("/");
        }
        catch (error) {
            setError(error);
        }
        finally {
            setLoading(false);
        }
    }
    async function signout() {
        signOut()
            .then( () =>{
                setUser(undefined);
                clearDistrict();
            })
    }
    function authorizedEdit(planOwner : Ranger) : boolean{
        if (planOwner.id === user?.rangerId) {
            return true;
        }
        if (user?.role === "HeadOfDistrict" && district?.id === planOwner.districtId) {
            return true;
        }
        return false;
    }

    function hasRole(role: string): boolean {
        if (user?.role == role) {
            return true;
        }
        else return false;
    }

    const memoValue = useMemo(
        () => ({
            user,
            loading,
            error,
            signin,
            signout,
            authorizedEdit,
            hasRole
        }),
        [user, loading, error]
    );

    return (
        <AuthContext.Provider value={memoValue}>
            {children}
        </AuthContext.Provider>
    );
};

export default function useAuth() {
    return useContext(AuthContext);
}