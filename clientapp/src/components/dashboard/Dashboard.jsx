import React from "react";
import { useDispatch } from "react-redux";
import { logout } from "../../features/auth/authSlice";
import "./Dashboard.css";

const Dashboard = () => {
  const dispatch = useDispatch();

  const handleLogout = () => {
    dispatch(logout());  
  };

  return (
    <div className="container">
      <h2>Dashboard</h2>
      <p>Welcome to your dashboard!</p>
      <button onClick={handleLogout} className="button">
        Logout
      </button>
    </div>
  );
};

export default Dashboard;
