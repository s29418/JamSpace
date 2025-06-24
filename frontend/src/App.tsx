import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import NavigationBar from './components/NavigationBar';
import ProfilePage from "./pages/ProfilePage";
import TeamsPage from "./pages/TeamsPage";
import HomePage from "./pages/HomePage";

function App() {
    return (
        <Router>
            <NavigationBar />
            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/profile" element={<ProfilePage />} />
                <Route path="/team" element={<TeamsPage />} />
                {/*<Route path="/chat" element={< />} />*/}
                {/*<Route path="/search" element={< />} />*/}
                {/*<Route path="/notifications" element={< />} />*/}
            </Routes>
        </Router>
    );
}

export default App;
