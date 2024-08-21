import { Outlet, Navigate} from 'react-router-dom';
import useAuth from './AuthProvider';

function AuthRoute({ roles }: { roles?: string[] }) {
    const { user } = useAuth();

    if (!user) {
        return <Navigate to="/prihlasit" replace />;
    }

    if (roles && !roles.includes(user.role)) {
        // TODO add some alert unauthorized/ different page
        return <Navigate to="/" replace />;
    }

    return <Outlet/>
}

export default AuthRoute;