import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import NavigationBar from '../shared/ui/navigation/NavigationBar';
import ProfilePage from "../pages/Profile/ProfilePage";
import TeamsPage from "../pages/Teams/TeamsPage";
import HomePage from "../pages/Home/HomePage";
import TeamDetailsPage from "../pages/TeamDetails/TeamDetailsPage";
import AuthEventsBridge from "./AuthEventsBridge";
import FollowsListPage from "../pages/Profile/FollowsListPage/FollowsListPage";
import AppBootstrap from "./AppBootstrap";

function App() {
    return (
        <Router>
            <AuthEventsBridge />
            <AppBootstrap />
            <NavigationBar />

            <Routes>
                <Route path="/" element={<HomePage />} />

                <Route path="/profile" element={<ProfilePage />} />
                <Route path="/profile/:id" element={<ProfilePage />} />
                    <Route path="/profile/:id/followers" element={<FollowsListPage mode="followers" />} />
                    <Route path="/profile/:id/following" element={<FollowsListPage mode="following" />} />

                <Route path="/teams" element={<TeamsPage />} />
                    <Route path="/teams/:id" element={<TeamDetailsPage />} />
                {/*<Route path="/chat" element={< />} />*/}
                {/*<Route path="/search" element={< />} />*/}
                {/*<Route path="/notifications" element={< />} />*/}
            </Routes>
        </Router>
    );
}

export default App;
