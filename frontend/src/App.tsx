import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import NavigationBar from './components/NavigationBar';
import ProfilePage from "./pages/ProfilePage";

const Placeholder = ({ label }: { label: string }) => (
    <div style={{ backgroundColor: '#121212', height: '80vh', color: 'white', padding: '2rem' }}>
        <h1>{label}</h1>
    </div>
);

function App() {
    return (
        <Router>
            <NavigationBar />
            <Routes>
                <Route path="/" element={<Placeholder label="Home" />} />
                <Route path="/profile" element={<ProfilePage />} />
                <Route path="/team" element={<Placeholder label="Team" />} />
                <Route path="/chat" element={<Placeholder label="Chat" />} />
                <Route path="/search" element={<Placeholder label="Search" />} />
                <Route path="/notifications" element={<Placeholder label="Notifications" />} />
            </Routes>
        </Router>
    );
}

export default App;
