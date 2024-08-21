import { User, signIn, signOut } from '../../Services/UserService';
import { Ranger, getCurrentRanger } from '../../Services/RangerService';
import  { createContext, useContext, ReactNode, useState, useMemo} from 'react';
import { useNavigate} from 'react-router-dom';

interface AppContextType {
    user: User | undefined,
    ranger: Ranger | undefined,
    districtId: number | undefined,
    // routes, vehicles, rangers, somehow plans?
    loading: boolean,
    error: any,
    signin: (email: string, password: string) => void,
    signout: () => void,
}

const AppContext = createContext<AppContextType>({} as AppContextType);

export const AuthProvider = ({ children }: { children: ReactNode }): JSX.Element => {
    const [user, setUser] = useState<User | undefined>(undefined);
    const [ranger, setRanger] = useState<Ranger | undefined>(undefined);
    const [districtId, setDistrictId] = useState<number | undefined>(undefined);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<any>();
    const navigate = useNavigate();

    //TODO clearing errors - manual by calling a function/ when changing routes/ when unmounting?

    //TODO get current user from database if there is an active session/4
    async function signin(email: string, password: string) {
        setLoading(true);
        try {
            const user = await signIn(email, password);
            const ranger = await getCurrentRanger();
            if (ranger != undefined) {
                setDistrictId(ranger.districtId);
            }
            setUser(user);
            setRanger(ranger);
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
                setDistrictId(undefined);
                setRanger(undefined);
            })
    }

    const memoValue = useMemo(
        () => ({
            user,
            ranger,
            districtId,
            loading,
            error,
            signin,
            signout
        }),
        [user, ranger, districtId, loading, error]
    );

    return (
        <AppContext.Provider value={memoValue}>
            {children}
        </AppContext.Provider>
    );
};

export default function useAuth() {
    return useContext(AppContext);
}