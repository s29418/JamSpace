import React from "react";
import { Routes, Route } from "react-router-dom";
import NavigationBar from "../../../widgets/navigation/ui/NavigationBar";
import HomePage from "../../../pages/home/ui/HomePage";
import ProfilePage from "../../../pages/profile/ui/ProfilePage";
import FollowsListPage from "../../../pages/follows-list/ui/FollowsListPage";
import TeamsPage from "../../../pages/teams/ui/TeamsPage";
import TeamDetailsPage from "../../../pages/team-details/ui/TeamDetailsPage";
import SearchPage from "../../../pages/search/ui/SearchPage";
import ChatPage from "../../../pages/chat/ui/ChatPage";
import PostDetailsPage from "../../../pages/post-details/ui/PostDetailsPage";

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
                <Route path="/posts/:id" element={<PostDetailsPage />} />

                {/* <Route path="/notifications" element={< />} /> */}
            </Routes>
        </>
    );
}

export default AppRouter;
