import React from "react";
import { ProfileTab } from "./profileTab/ProfileTab";
import styles from "./EditProfilePanel.module.css";
import {UserProfile} from "../../../../../entities/user/model/types";
import {UpdateUserProfileRequest} from "../../../../../entities/user/api/profile.api";

type Props = {
    isOpen: boolean;
    onClose: () => void;
    initialTab?: "profile" | "skills" | "genres" | "security";
    profile: UserProfile;
    onSaveProfile: (draft: UpdateUserProfileRequest, file?: File) => Promise<void> | void; // ← dodany file
};

export const EditProfilePanel: React.FC<Props> = ({
                                                      isOpen,
                                                      onClose,
                                                      initialTab = "profile",
                                                      profile,
                                                      onSaveProfile
                                                  }) => {
    const [tab, setTab] = React.useState<typeof initialTab>(initialTab);

    React.useEffect(() => setTab(initialTab), [initialTab]);

    return (
        <>
            <div
                className={`${styles["edit-panel-backdrop"]} ${isOpen ? styles.open : ""}`}
                onClick={onClose}
            />
            <aside
                className={`${styles["edit-panel"]} ${isOpen ? styles.open : ""}`}
                aria-hidden={!isOpen}
            >
                <header className={styles["edit-panel__header"]}>
                    <h2>Edit profile</h2>
                    <button className={styles["icon-btn"]} onClick={onClose} aria-label="Close">
                        ✕
                    </button>
                </header>

                <nav className={styles["edit-panel__tabs"]}>
                    <button
                        className={tab === "profile" ? styles.active : ""}
                        onClick={() => setTab("profile")}
                    >
                        Profile
                    </button>
                    <button
                        className={tab === "skills" ? styles.active : ""}
                        onClick={() => setTab("skills")}
                    >
                        Skills
                    </button>
                    <button
                        className={tab === "genres" ? styles.active : ""}
                        onClick={() => setTab("genres")}
                    >
                        Genres
                    </button>
                    <button
                        className={tab === "security" ? styles.active : ""}
                        onClick={() => setTab("security")}
                    >
                        Security
                    </button>
                </nav>

                <div className={styles["edit-panel__content"]}>
                    {tab === "profile" && (
                        <ProfileTab
                            initial={profile}
                            onCancel={onClose}
                            onSave={async (draft, file) => {
                                await onSaveProfile(draft, file);
                                onClose();
                            }}
                        />
                    )}
                    {tab === "skills" && (
                        <div className={styles.placeholder}>

                        </div>
                    )}
                    {tab === "genres" && (
                        <div className={styles.placeholder}>

                        </div>
                    )}
                    {tab === "security" && (
                        <div className={styles.placeholder}>
                            1234
                        </div>
                    )}
                </div>
            </aside>
        </>
    );
};