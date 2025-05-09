﻿import { BrowserRouter, Routes, Route} from 'react-router-dom';
import './App.css';
import Login from './Pages/Login';
import Menu from './Components/Menu/Menu';
import Planner from './Pages/Planner';
import SourceManagement from './Pages/SourceManagement'
import { AuthProvider } from './Providers/Authentication/AuthProvider';
import AuthRoute from './Providers/Authentication/AuthRoutes';
import { DistrictDataProvider } from './Providers/DataProviders/DistrictDataProvider';
import { SchedulesProvider } from './Providers/DataProviders/ScheduleDataProvider';
import GeneratedPreview from './Components/Planner/GeneratedPreview/GeneratedPreview';


function App() {
    return (
        <BrowserRouter>
            <DistrictDataProvider>
                <AuthProvider>
                    <SchedulesProvider>
                        <Routes>
                            <Route path="/prihlasit" element={<Login />} />
                            <Route element={<AuthRoute />}>
                                <Route path="/" element={<Menu />}>
                                    <Route path="/planovani" element={<Planner />} />
                                    <Route element={<AuthRoute roles={["Admin", "HeadOfDistrict"]} />}>
                                        <Route path="/sprava" element={<SourceManagement />} />
                                        <Route path="/generovani/:date" element={<GeneratedPreview />} />
                                    </Route>
                                </Route>
                            </Route>
                        </Routes>
                    </SchedulesProvider>
                </AuthProvider>
            </DistrictDataProvider>
        </BrowserRouter>
    );
}

export default App;