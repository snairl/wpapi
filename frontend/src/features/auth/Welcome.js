import { useSelector } from "react-redux";
import { selectCurrentUser, selectCurrentToken } from "./authSlice";
import { Link } from "react-router-dom";

const Welcome = () => {
    const user = useSelector(selectCurrentUser);
    const token = useSelector(selectCurrentToken);
    
    const content = (
        <section className="public">
        <header>
            <h1>Welcome</h1>
        </header>
        <main>
            <p>Welcome, {user?.name}!</p>
            <p>Your token is: {token}</p>
        </main>
        <footer>
            <Link to="/dashboard">Dashboard</Link>
        </footer>
        </section>
    );
    
    return content;
}

export default Welcome;