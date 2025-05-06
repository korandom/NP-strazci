import { User, getCurrentUser, signIn, signOut } from '../../Services/UserService';
import { getCurrentRanger } from '../../Services/RangerService';
import  { createContext, ReactNode, useState, useMemo, useEffect, useRef, useCallback} from 'react';
import { useNavigate } from 'react-router-dom';
import useDistrict from '../../Hooks/useDistrict';

interface AuthContextType {
    user: User | undefined,
    loading: boolean,
    error: Error | null,
    signin: (email: string, password: string) => void,
    signout: () => void,
    hasRole: (role: string) => boolean
 }

export const AuthContext = createContext<AuthContextType>({} as AuthContextType);

/**
 * AuthProvider manages the user state and provides functions to sign up and sign out.
 *
 * @param children - The child components that will have access to the authentication context.
 * @returns A JSX.Element that provides the context to its children.
 * 
 */
export const AuthProvider = ({ children }: { children: ReactNode }): JSX.Element => {
    const [user, setUser] = useState<User | undefined>(undefined);
    const [loading, setLoading] = useState<boolean>(false);
    const [initialLoading, setInitialLoading] = useState<boolean>(true);
    const isInitializing = useRef(false);
    const [error, setError] = useState<Error|null>(null);
    const navigate = useNavigate();
    const { assignDistrict, clearDistrict } = useDistrict();

    // initial load of all data + assigning a district if user is a ranger (therefor has a district)
    useEffect(() => {
        const fetchInitialData = async () => {
            if (isInitializing.current) return;
            isInitializing.current = true;

            setInitialLoading(true);
            try {
                const user = await getCurrentUser();
                const ranger = await getCurrentRanger();
                if (ranger != undefined) {
                    assignDistrict(ranger.districtId);
                }
                setUser(user);
            } catch (error) {
                console.error('Error during initialization:', error);
            } finally {
                setInitialLoading(false);
                isInitializing.current = false;
            }
        };

        fetchInitialData();
    },[assignDistrict]);

    // sign in with email and password, after is redirect to /planovani
    async function signin(email: string, password: string) {
        setLoading(true);
        try {
            const user = await signIn(email, password);
            const ranger = await getCurrentRanger();
            if (ranger != undefined) {
                assignDistrict(ranger.districtId);
            }
            setError(null);
            setUser(user);
            navigate("/planovani");
        }
        catch (error) {
            if (error instanceof Error) {
                console.error('Error during sign-in:', error.message);
                setError(error);
            } else {
                console.error('Unknown error during sign-in:', error);
                setError(new Error('Neznámý error při přihlašování'));
            }
        }
        finally {
            setLoading(false);
        }
    }

    // signs out and clears data
    async function signout() {
        signOut()
            .then( () =>{
                setUser(undefined);
                clearDistrict();
            })
    }

    // check if user has specific role
    const hasRole = useCallback((role: string): boolean => {
        return user?.role === role;
    }, [user]);

    const memoValue = useMemo(
        () => ({
            user,
            loading,
            error,
            signin,
            signout,
            hasRole
        }),
        [user, loading, error, hasRole]
    );

    return (
        <AuthContext.Provider value={memoValue}>
            {!initialLoading && children}
        </AuthContext.Provider>
    );
};

