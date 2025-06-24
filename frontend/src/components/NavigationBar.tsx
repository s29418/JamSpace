import React from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import styles from "./NavigationBar.module.css";

import {
    HomeIcon as HomeOutline,
    UserIcon as UserOutline,
    UsersIcon as UsersOutline,
    ChatBubbleOvalLeftEllipsisIcon as ChatOutline,
    MagnifyingGlassIcon as SearchOutline,
    BellIcon as BellOutline
} from '@heroicons/react/24/outline';

import {
    HomeIcon as HomeSolid,
    UserIcon as UserSolid,
    UsersIcon as UsersSolid,
    ChatBubbleOvalLeftEllipsisIcon as ChatSolid,
    MagnifyingGlassIcon as SearchSolid,
    BellIcon as BellSolid
} from '@heroicons/react/24/solid';

import logo from '../assets/logo.png';

const NavigationBar = () => {
    const location = useLocation();
    const navigate = useNavigate();

    const isActive = (path: string) => location.pathname === path;

    return (
        <nav className={styles.navbar}>
            {/* LEWA STRONA */}
            <NavButton
                iconSolid={HomeSolid}
                iconOutline={HomeOutline}
                path="/"
                active={isActive('/')}
                navigate={navigate}
            />
            <NavButton
                iconSolid={UserSolid}
                iconOutline={UserOutline}
                path="/profile"
                active={isActive('/profile')}
                navigate={navigate}
            />
            <NavButton
                iconSolid={UsersSolid}
                iconOutline={UsersOutline}
                path="/team"
                active={isActive('/team')}
                navigate={navigate}
            />

            {/* LOGO */}
            <div className={styles.logoWrapper} onClick={() => navigate('/')}>
                <img src={logo} alt="JamSpace logo" className={styles.logo}/>
            </div>

            {/* PRAWA STRONA */}
            <NavButton
                iconSolid={ChatSolid}
                iconOutline={ChatOutline}
                path="/chat"
                active={isActive('/chat')}
                navigate={navigate}
            />
            <NavButton
                iconSolid={SearchSolid}
                iconOutline={SearchOutline}
                path="/search"
                active={isActive('/search')}
                navigate={navigate}
            />
            <NavButton
                iconSolid={BellSolid}
                iconOutline={BellOutline}
                path="/notifications"
                active={isActive('/notifications')}
                navigate={navigate}
            />
        </nav>
    );
};

const NavButton = ({
                       iconSolid: Solid,
                       iconOutline: Outline,
                       path,
                       active,
                       navigate
                   }: {
    iconSolid: React.ElementType;
    iconOutline: React.ElementType;
    path: string;
    active: boolean;
    navigate: (path: string) => void;
}) => {
    const Icon = active ? Solid : Outline;
    return (
        <button onClick={() => navigate(path)} className={styles.navIcon}>
            <Icon className={styles.navIcon} />
        </button>
    );
};

export default NavigationBar;
