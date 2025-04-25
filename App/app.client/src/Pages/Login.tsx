import useAuth from '../Hooks/useAuth';
import React, { useState } from 'react';
import './Style/Login.css';

/**
 * Login Page, just a form for email and password. If Successful redirects to /planovani.
 * @returns A JSX.Element of the login form.
 */
 const Login = () : JSX.Element => {
     const { signin, loading, error } = useAuth();
     const [email, setEmail] = useState("");
     const [password, setPassword] = useState("");

     const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
         event.preventDefault();
         signin(email, password);
     };

    return (
        <div className="login-container">
            <form className="login-form" onSubmit={handleSubmit}>
                <div className="form-group">
                    <label className="forminput" htmlFor="email">Email</label>
                    <input
                        type="email"
                        id="email"
                        name="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)} 
                        required
                    />
                </div>
                <div className="form-group">
                    <label className="forminput" htmlFor="password">Heslo</label>
                    <input
                        type="password"
                        id="password"
                        name="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>
                <div className="submit-button">
                    <button type="submit" disabled={loading}>
                        {loading ? '...' : 'Přihlásit'}
                    </button>
                </div>
            </form>
            {error && <p className="error">{error.message}</p>}
        </div>
    );
}


export default Login;