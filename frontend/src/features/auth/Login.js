import { useRef, useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

import { useDispatch } from "react-redux";
import { setCredentials } from "./authSlice";
import { useLoginMutation } from "./authApiSlice";




const Login = () => {
    const userRef = useRef();
    const errorRef = useRef();
    const [user,setUser] = useState("");
    const [password,setPassword] = useState("");
    const [errMsg, setErrMsg] = useState("");
    const navigate = useNavigate();
    
    const [login, { isLoading, error }] = useLoginMutation();
    const dispatch = useDispatch();

    useEffect(() => {
        userRef.current.focus();
    }, []);

    useEffect(() => {
        setErrMsg("");
    }, [user, password]);

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (user && password) {
            try {
                const result = await login({ user, password }).unwrap();
                dispatch(setCredentials(result.payload));
                if (result.payload.token) {
                    dispatch(setCredentials({ ...result, user }));
                    setUser("");
                    setPassword("");
                    navigate("/dashboard");
                }
            } catch (err) {
                setErrMsg("Invalid user or password");
                // errorRef.current.focus();
            }
        } else {
            setErrMsg("Please enter user and password");
            // errorRef.current.focus();    
        }
    };

    const handleUserChange = (e) => setUser(e.target.value);
    const handlePasswordChange = (e) => setPassword(e.target.value);

    const content = isLoading ? <h1>Loading ...</h1> :(
        <section className="login">
            <p ref={errorRef} className={errMsg? "errMsg":"offscreen"}>{errMsg}</p>
            <h1>Login</h1>
            <form onSubmit={handleSubmit}>
                <label htmlFor="username">Username</label>
                <input
                    type="text"
                    id="username"
                    ref={userRef}
                    value={user}
                    onChange={handleUserChange} 
                    autoComplete="off"
                    required
                    />
                <label htmlFor="password">Password</label>
                <input
                    type="password"
                    id="password"
                    value={password}
                    onChange={handlePasswordChange}
                    autoComplete="off"
                    required
                    />
                <button type="submit">Login</button>
            </form>

        </section>
    );
    
    return content
};

export default Login;