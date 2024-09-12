import React, { useState } from "react";
import './Login.css';

const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);

  // Function to handle login API call
  const authenticateUser = async () => {
    setLoading(true);
    setError("");
    setSuccess(false);

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
        // Handle successful authentication (e.g., store token, redirect)
        setSuccess(true);
        console.log("Login successful:", data);
        // For example: localStorage.setItem("token", data.token);
      } else {
        // Handle errors (e.g., invalid credentials)
        setError(data.message || "Invalid username or password");
      }
    } catch (error) {
      // Handle network errors
      setError("Something went wrong. Please try again later.");
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    // Basic validation
    if (username === "" || password === "") {
      setError("Both fields are required.");
      return;
    }

    // Call the authenticate function
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
        {success && <p className="success">Login successful!</p>}

        <button type="submit" className="button" disabled={loading}>
          {loading ? "Logging in..." : "Login"}
        </button>
      </form>
    </div>
  );
};

export default Login;
