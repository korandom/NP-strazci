import { BrowserRouter, Routes, Route } from 'react-router-dom';
import './App.css';
import Home from './Pages/Home';
import Menu from './Components/Menu';
import Planner from './Pages/Planner';

function App() {
    return (
        <BrowserRouter>
            <Menu/>
            <Routes>
                <Route path="/" element={<Home />} />
                <Route path="/planovani" element={<Planner />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;