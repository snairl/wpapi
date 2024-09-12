import React from "react";
import { useDispatch } from "react-redux";
import { logout } from "../../features/auth/authSlice";
import { useNavigate  } from "react-router-dom";
import "./Dashboard.css";


const Dashboard = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const handleLogout = () => {
    dispatch(logout());  
  };

  const handleBrowse = () => {
    navigate("/category");
  }

  return (
    <div className="container">
      <h2>Dashboard</h2>
      <p>Browse categories here</p>
      <button onClick={handleBrowse} className="button">
        Browse
      </button>
      <p>or</p>
      <button onClick={handleLogout} className="button">
        Logout
      </button>
    </div>
  );
};

export default Dashboard;
