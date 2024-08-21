import useAuth from '../Components/Authentication/AuthProvider';
import React, { useState } from 'react';

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
            <form onSubmit={handleSubmit}>
                <div>
                    <label className="forminput" htmlFor="email">Email:</label>
                </div>
                <div>
                    <input
                        type="email"
                        id="email"
                        name="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)} 
                        required
                    />
                </div>
                <div>
                    <label className="forminput" htmlFor="password">Heslo:</label>
                </div>
                <div>
                    <input
                        type="password"
                        id="password"
                        name="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <button type="submit" disabled={loading}>
                        {loading ? '...' : 'Pøihlásit'}
                    </button>
                </div>
            </form>
            {error && <p className="error">{error.message}</p>}
        </div>
    );
}


export default Login;