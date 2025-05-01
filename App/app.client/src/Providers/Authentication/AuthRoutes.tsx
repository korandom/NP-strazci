import { Outlet, Navigate} from 'react-router-dom';
import useAuth from '../../Hooks/useAuth';

/** 
 * A route protection component that restricts access based on user authentication and roles.
 * 
 * @param param0 - The component props.
 * @param param0.roles - Optional array of roles that are allowed to access the route.
 * @returns 
 * - Redirects to the login page ("/prihlasit") if the user is not authenticated.  
 * - Redirects to the planner page ("/planovani") if the user does not have the required role.  
 * - Renders the nested routes (`<Outlet />`) if the user is authenticated and authorized.
 */
function AuthRoute({ roles }: { roles?: string[] }) {
    const { user } = useAuth();

    if (!user) {
        return <Navigate to="/prihlasit" replace />;
    }

    if (roles && !roles.includes(user.role)) {
        return <Navigate to="/planovani" replace />;
    }

    return <Outlet/>
}

export default AuthRoute;