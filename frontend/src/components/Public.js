import { Link } from "react-router-dom";

const Public = () => {

    const content = (
        <section className="public">
            <header>
                <h1>WordPress Web API</h1>
            </header>
            <main>
                <p>Some information</p>
            </main>
            <footer>
                <Link to="/login">Login</Link>
            </footer>
        </section>
    )

    return content
};

export default Public;