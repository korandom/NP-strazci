import { User, getCurrentUser, signIn, signOut } from '../../Services/UserService';
import { getCurrentRanger } from '../../Services/RangerService';
import  { createContext, useContext, ReactNode, useState, useMemo, useEffect, useRef} from 'react';
import { useNavigate } from 'react-router-dom';
import useDistrict from '../DataProviders/DistrictDataProvider';

interface AuthContextType {
    user: User | undefined,
    loading: boolean,
    error: any,
    signin: (email: string, password: string) => void,
    signout: () => void,
    hasRole: (role: string) => boolean
 }

const AuthContext = createContext<AuthContextType>({} as AuthContextType);

export const AuthProvider = ({ children }: { children: ReactNode }): JSX.Element => {
    const [user, setUser] = useState<User | undefined>(undefined);
    const [loading, setLoading] = useState<boolean>(false);
    const [initialLoading, setInitialLoading] = useState<boolean>(true);
    const isInitializing = useRef(false);
    const [error, setError] = useState<any>();
    const navigate = useNavigate();
    const { assignDistrict, clearDistrict } = useDistrict();

    useEffect(() => {
        const fetchInitialData = async () => {
            if (isInitializing.current) return;
            isInitializing.current = true;

            setInitialLoading(true);
            try {
                const user = await getCurrentUser();
                const ranger = await getCurrentRanger();
                if (ranger != undefined) {
                    await assignDistrict(ranger.districtId);
                }
                setUser(user);
            } catch (error) {
                {}
            } finally {
                setInitialLoading(false);
                isInitializing.current = false;
            }
        };

        fetchInitialData();
    }, []);

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

    function hasRole(role: string): boolean {
        return user?.role == role;
        
    }

    const memoValue = useMemo(
        () => ({
            user,
            loading,
            error,
            signin,
            signout,
            hasRole
        }),
        [user, loading, error]
    );

    return (
        <AuthContext.Provider value={memoValue}>
            {!initialLoading && children}
        </AuthContext.Provider>
    );
};

export default function useAuth() {
    return useContext(AuthContext);
}