import { BrowserRouter, Routes, Route } from 'react-router-dom';
import './App.css';
import Login from './Pages/Login';
import Home from './Pages/Home';
import Menu from './Components/Menu';
import Planner from './Pages/Planner';
import SourceManagement from './Pages/SourceManagement'
import { AuthProvider } from './Components/Authentication/AuthProvider';
import AuthRoute from './Components/Authentication/AuthRoutes';
import { DistrictDataProvider } from './Components/DataProviders/DistrictDataProvider';
import { MonthlyPlansProvider } from './Components/DataProviders/PlanDataProvider';


function App() {
    return (
        <BrowserRouter>
            <DistrictDataProvider>
                <AuthProvider>
                    <MonthlyPlansProvider>
                        <Routes>
                            <Route path="/prihlasit" element={<Login />} />
                            <Route element={<AuthRoute />}>
                                <Route element={<Menu />}>
                                        <Route path="/" element={<Home />} />
                                        <Route path="/planovani" element={<Planner />} />
                                    <Route element={<AuthRoute roles={["Admin", "HeadOfDistrict"]} />}>
                                        <Route path="/sprava" element={<SourceManagement />} />
                                    </Route>
                                </Route>
                            </Route>
                        </Routes>
                    </MonthlyPlansProvider>
                </AuthProvider>
            </DistrictDataProvider>
        </BrowserRouter>
    );
}

export default App;