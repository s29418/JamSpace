import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import NavigationBar from './components/NavigationBar';

const Placeholder = ({ label }: { label: string }) => (
    <div style={{ backgroundColor: '#121212', height: '100vh', color: 'white', padding: '2rem' }}>
        <h1>{label}</h1>
    </div>
);

function App() {
    return (
        <Router>
            <NavigationBar />
            <Routes>
                <Route path="/" element={<Placeholder label="Strona główna" />} />
                <Route path="/profile" element={<Placeholder label="Profil" />} />
                <Route path="/team" element={<Placeholder label="Zespół" />} />
                <Route path="/chat" element={<Placeholder label="Czat" />} />
                <Route path="/search" element={<Placeholder label="Wyszukaj" />} />
                <Route path="/notifications" element={<Placeholder label="Powiadomienia" />} />
            </Routes>
        </Router>
    );
}

export default App;
