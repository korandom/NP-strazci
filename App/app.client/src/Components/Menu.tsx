import  { useState } from 'react';
import { Outlet, Link } from 'react-router-dom';
import useAuth from './Authentication/AuthProvider';
import './Menu.css';

const Menu = () => {
    const [isOpen, setIsOpen] = useState(false);
    const { signout } = useAuth();
    const toggleMenu = () => {
        setIsOpen(!isOpen);
    };
    const closeMenu = () => {
        setIsOpen(false);
    }

    return (
        <>
        <div className={isOpen ? "menu-wrapper whole-screen" : "menu-wrapper"} >
            <div className="toggle-menu" onClick={toggleMenu}>
                {isOpen ? '✕' : '☰'}
            </div>
            <div className={isOpen ? "menu open" : "menu"}>
                <nav className="menu-up">

                        <Link to="/" onClick={closeMenu}>Plán Dne</Link>
                        <Link to="/planovani" onClick={closeMenu}>Plánování</Link>
                        Zdroje
                </nav>
                <nav className="menu-down" onClick={signout}>
                        Odhlásit
                </nav>
            </div>

        </div>

        <Outlet/>
       </>
    );
};

export default Menu;