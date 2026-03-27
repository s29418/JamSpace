import React from "react";
import { BrowserRouter as Router } from "react-router-dom";
import AuthEventsBridge from "./providers/auth/AuthEventsBridge";
import AppBootstrap from "./providers/auth/AppBootstrap";
import AppRouter from "./providers/router/AppRouter";

function App() {
    return (
        <Router>
            <AuthEventsBridge />
            <AppBootstrap />
            <AppRouter />
        </Router>
    );
}

export default App;