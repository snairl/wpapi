import { useLocation, Navigate, Outlet } from "react-router-dom";
import { useSelector } from "react-redux";
import { selectCurrentToken } from "./authSlice";

const RequireAuth = () => {
  const token = useSelector(selectCurrentToken);
  const location = useLocation();

  if (!token) {
    return <Navigate to="/login" state={{from: location}} replace />;
  }

  return <Outlet />;
};

export default RequireAuth;