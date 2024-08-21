import { BrowserRouter, Routes, Route } from 'react-router-dom';
import './App.css';
import Login from './Pages/Login';
import Home from './Pages/Home';
import Menu from './Components/Menu';
import Planner from './Pages/Planner';
import { AuthProvider } from './Components/Authentication/AuthProvider';
import AuthRoute from './Components/Authentication/AuthRoutes';


function App() {
    return (
        <BrowserRouter>
            <AuthProvider>
                <Routes>
                    <Route path="/prihlasit" element={<Login />} />
                    <Route element={<AuthRoute/> }>
                        <Route element={<Menu />}>
                            <Route path="/" element={<Home />} />
                            <Route path="/planovani" element={<Planner />} />
                        </Route>
                    </Route>
                </Routes>
            </AuthProvider>
        </BrowserRouter>
    );
}

export default App;