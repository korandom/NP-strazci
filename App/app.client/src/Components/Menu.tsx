import  { useState, useEffect } from 'react';
import { Outlet, Link } from 'react-router-dom';
import useAuth from './Authentication/AuthProvider';
import './Menu.css';
import { District, fetchAllDistricts } from '../Services/DistrictService';
import useDistrict from './DistrictContext/DistrictDataProvider';

const Menu = () => {
    const [isOpen, setIsOpen] = useState(false);
    const { signout, hasRole } = useAuth();
    const { assignDistrict } = useDistrict();
    const [districts, setDistricts] = useState<District[]>([]);
    const [ isDistrictDropdownOpen, setIsDistrictDropdownOpen ] = useState(false);

    useEffect(() => {
        const fetchDistricts = async () => {
            try {
                const districts = await fetchAllDistricts();
                setDistricts(districts);
            } catch (error) {
                console.error(error);
            }
        };
        if (hasRole("Admin")) {
            fetchDistricts();
        }
    }, []);

    const toggleMenu = () => {
        setIsOpen(!isOpen);
    };

    const closeMenu = () => {
        setIsOpen(false);
        setIsDistrictDropdownOpen(false);
    }

    const toggleDistrictDropdown = () => {
        setIsDistrictDropdownOpen(!isDistrictDropdownOpen);
    }




    return (
        <>
        <div className={isOpen ? "menu-wrapper whole-screen" : "menu-wrapper"} >
            <div className="toggle-menu" onClick={toggleMenu}>
                {isOpen ? '✕' : '☰'}
            </div>
            <div className={isOpen ? "menu open" : "menu"}>
                <nav className="menu-up">
                        
                        <Link to="/" onClick={closeMenu} className="menu-item">🏠</Link>
                        <Link to="/planovani" onClick={closeMenu} className="menu-item">Plánování</Link>
                        {
                            (hasRole("HeadOfDistrict") || hasRole("Admin")) &&
                            <Link to="/sprava" onClick={closeMenu} className="menu-item">Správa objektů</Link>

                        }
                        {
                            hasRole("Admin") &&
                            <div>
                                <div className="menu-item" onClick={toggleDistrictDropdown}>
                                    Obvody
                                </div>

                                {isDistrictDropdownOpen && (
                                    <div className="dropdown-content">
                                        {districts.map((district, index) => (
                                            <div key={index} className="dropdown-item" onClick={ async () => {await assignDistrict(district.id); closeMenu(); }}>
                                                {district.name}
                                            </div>
                                        ))}
                                    </div>
                                )}
                            </div>
                        }
                </nav>
                <nav className="menu-down">
                        <div onClick={signout} className="menu-item">Odhlásit</div>
                </nav>
            </div>

        </div>

        <Outlet/>
       </>
    );
};

export default Menu;