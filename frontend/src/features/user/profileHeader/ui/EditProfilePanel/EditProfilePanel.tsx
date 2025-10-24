import React, {FC, useEffect, useState} from "react";
import { ProfileTab } from "./profileTab/ProfileTab";
import styles from "./EditProfilePanel.module.css";
import {UserProfile} from "../../../../../entities/user/model/types";
import {UpdateUserProfileRequest} from "../../../../../entities/user/api/profile.api";
import {TagsEditorTab} from "./tagsEditorTab/TagsEditorTab";
import {AccountTab} from "./accountTab/AccountTab";

type Props = {
    isOpen: boolean;
    onClose: () => void;
    initialTab?: "profile" | "skills" | "genres" | "account";
    profile: UserProfile;
    onSaveProfile: (draft: UpdateUserProfileRequest, file?: File) => Promise<void> | void;
    addGenre?: (name: string) => Promise<void> | void;
    removeGenre?: (id: string) => Promise<void> | void;
    addSkill?: (name: string) => Promise<void> | void;
    removeSkill?: (id: string) => Promise<void> | void;
    onUpdateEmail?: (email: string) => Promise<void> | void;
    onChangePassword?: (current: string, next: string) => Promise<void> | void;
    onDeleteAccount?: () => Promise<void> | void;
};

export const EditProfilePanel: FC<Props> = ({
                                                      isOpen,
                                                      onClose,
                                                      initialTab = "profile",
                                                      profile,
                                                      onSaveProfile,
                                                      addGenre,
                                                      removeGenre,
                                                      addSkill,
                                                      removeSkill,
                                                      onUpdateEmail,
                                                      onChangePassword,
                                                      onDeleteAccount
                                                  }) => {
    const [tab, setTab] = useState<typeof initialTab>(initialTab);

    useEffect(() => setTab(initialTab), [initialTab]);

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
                        className={tab === "account" ? styles.active : ""}
                        onClick={() => setTab("account")}
                    >
                        Account
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

                    {tab === 'skills' && (
                        <TagsEditorTab
                            title="Skills"
                            items={profile.skills ?? []}
                            onAdd={async (name) => { await addSkill?.(name); }}
                            onRemove={async (id) => { await removeSkill?.(id); }}
                            placeholder="Add skill (e.g., Producer)"
                        />
                    )}

                    {tab === 'genres' && (
                        <TagsEditorTab
                            title="Genres"
                            items={profile.genres ?? []}
                            onAdd={async (name) => { await addGenre?.(name); }}
                            onRemove={async (id) => { await removeGenre?.(id); }}
                            placeholder="Add genre (e.g., Hip-hop)"
                        />
                    )}

                    {tab === 'account' && (
                        <AccountTab
                            initialEmail={profile.email}
                            onUpdateEmail={async (email) => { await onUpdateEmail?.(email); }}
                            onChangePassword={async (cur, nxt) => { await onChangePassword?.(cur, nxt); }}
                            onDeleteAccount={async () => { await onDeleteAccount?.(); }}
                        />
                    )}
                </div>
            </aside>
        </>
    );
};