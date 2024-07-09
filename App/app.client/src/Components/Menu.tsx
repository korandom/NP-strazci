import  { useState } from 'react';
import { Link } from 'react-router-dom';
import './Menu.css';

const Menu = () => {
    const [isOpen, setIsOpen] = useState(false);

    const toggleMenu = () => {
        setIsOpen(!isOpen);
    };
    const closeMenu = () => {
        setIsOpen(false);
    }

    return (
        <div className="menu-wrapper" >
            <div className="toggle-menu" onClick={toggleMenu}>&#9776;</div>
            <div className={isOpen ? "menu-open" : "menu"}>
                <nav className="menu-up">

                        <Link to="/" onClick={closeMenu}>Plán Dne</Link>
                        <Link to="/planovani" onClick={closeMenu}>Plánování</Link>
                        Zdroje
                </nav>
                <nav className="menu-down">
                        Odhlásit
                </nav>
            </div>

        </div>
       
    );
};

export default Menu;