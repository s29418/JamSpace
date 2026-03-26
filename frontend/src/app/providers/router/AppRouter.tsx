import React from "react";
import { Routes, Route } from "react-router-dom";
import NavigationBar from "../../../widgets/navigation/ui/NavigationBar";
import HomePage from "../../../pages/Home/ui/HomePage";
import ProfilePage from "../../../pages/Profile/ui/ProfilePage";
import FollowsListPage from "../../../pages/Follows-list/ui/FollowsListPage";
import TeamsPage from "../../../pages/Teams/ui/TeamsPage";
import TeamDetailsPage from "../../../pages/TeamDetails/ui/TeamDetailsPage";
import SearchPage from "../../../pages/Search/ui/SearchPage";
import ChatPage from "../../../pages/Chat/ui/ChatPage";

function AppRouter() {
    return (
        <>
            <NavigationBar />

            <Routes>
                <Route path="/" element={<HomePage />} />

                <Route path="/profile" element={<ProfilePage />} />
                <Route path="/profile/:id" element={<ProfilePage />} />
                <Route
                    path="/profile/:id/followers"
                    element={<FollowsListPage mode="followers" />}
                />
                <Route
                    path="/profile/:id/following"
                    element={<FollowsListPage mode="following" />}
                />

                <Route path="/teams" element={<TeamsPage />} />
                <Route path="/teams/:id" element={<TeamDetailsPage />} />

                <Route path="/search" element={<SearchPage />} />

                <Route path="/chat" element={<ChatPage />} />

                {/* <Route path="/notifications" element={< />} /> */}
            </Routes>
        </>
    );
}

export default AppRouter;