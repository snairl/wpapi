import "./App.css"
import Login from "./components/login/Login"
import Dashboard from "./components/dashboard/Dashboard";
import { BrowserRouter as Router, Route, Routes, Navigate } from "react-router-dom";
import { useSelector } from 'react-redux';

const PrivateRoute = ({ children }) => {
  const token = useSelector((state) => state.auth.token);
  return token ? children : <Navigate to="/login" />;
};


function App() {
  const token = useSelector((state) => state.auth.token);

  return (
    <Router>
    <Routes>
      {/* Public route for login */}
      <Route
        path="/login"
        element={token ? <Navigate to="/dashboard" /> : <Login />}
      />

      {/* Protected route for dashboard */}
      <Route
        path="/dashboard"
        element={
          <PrivateRoute>
            <Dashboard />
          </PrivateRoute>
        }
      />

      {/* Default route */}
      <Route path="/" element={<Navigate to="/dashboard" />} />
    </Routes>
  </Router>
  )
}

export default App
