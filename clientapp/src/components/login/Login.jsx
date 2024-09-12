import React, { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { loginStart, loginSuccess, loginFailure } from "../../features/auth/authSlice";
import { useNavigate } from "react-router-dom";
import './Login.css';

const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  // const [error, setError] = useState("");
  // const [loading, setLoading] = useState(false);
  // const [success, setSuccess] = useState(false);

  const dispatch = useDispatch();
  const { loading, error, user } = useSelector((state) => state.auth);

  // Function to handle login API call
  const authenticateUser = async () => {
    dispatch(loginStart());  // Dispatch the loginStart action

    try {
      const response = await fetch("/api/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ username, password }),
      });

      const data = await response.json();

      if (response.ok) {
        // Dispatch loginSuccess if API call succeeds
        dispatch(loginSuccess({ user: data.user, token: data.token }));
        console.log("Login successful:", data);
        navigate("/dashboard");
      } else {
        // Dispatch loginFailure if login fails
        dispatch(loginFailure(data.message || "Invalid credentials"));
      }
    } catch (err) {
      dispatch(loginFailure("Something went wrong. Please try again."));
    }
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    // Basic validation
    if (username === "" || password === "") {
      dispatch(loginFailure("Both fields are required."));
      return;
    }

    // Call the authenticate function
    //TODO move in a separate function ???
    authenticateUser();
  };

  return (
    <div className="container">
      <h2>Login</h2>
      <form onSubmit={handleSubmit} className="form">
        <div className="input-container">
          <label className="label">Username</label>
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            className="input"
            placeholder="Enter username"
            disabled={loading}
          />
        </div>

        <div className="input-container">
          <label className="label">Password</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="input"
            placeholder="Enter password"
            disabled={loading}
          />
        </div>

        {error && <p className="error">{error}</p>}

        <button type="submit" className="button" disabled={loading}>
          {loading ? "Logging in..." : "Login"}
        </button>
      </form>
    </div>
  );
};

export default Login;
