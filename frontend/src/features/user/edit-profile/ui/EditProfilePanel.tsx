import React, {FC, useEffect, useState} from "react";
import { ProfileTab } from "./profile-tab/ProfileTab";
import styles from "./EditProfilePanel.module.css";
import {UserProfile} from "../../../../entities/user/model/types";
import {UpdateUserProfileRequest} from "../../../../entities/user/api/profile.api";
import {TagsEditorTab} from "./tags-editor-tab/TagsEditorTab";
import {AccountTab} from "./account-tab/AccountTab";
import {verifyPassword} from "../../../../entities/user/api/auth.api";
import {ApiError} from "../../../../shared/api/base";

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
    onLogoutAll?: () => Promise<void> | void;
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
                                                      onDeleteAccount,
                                                      onLogoutAll,
                                                  }) => {
    const [tab, setTab] = useState<typeof initialTab>(initialTab);
    const [isAccountUnlocked, setIsAccountUnlocked] = useState(false);
    const [verifyLoading, setVerifyLoading] = useState(false);
    const [verifyError, setVerifyError] = useState<string | null>(null);
    const [verifyPasswordValue, setVerifyPasswordValue] = useState('');

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
                        isAccountUnlocked ? (

                            <AccountTab
                                initialEmail={profile.email}
                                onUpdateEmail={async (email) => { await onUpdateEmail?.(email); }}
                                onChangePassword={async (cur, nxt) => { await onChangePassword?.(cur, nxt); }}
                                onDeleteAccount={async () => { await onDeleteAccount?.(); }}
                                onLogoutAll={async () => { await onLogoutAll?.(); }}
                            />

                        ) : (

                            <form
                                className={styles.verifyWrapper}
                                onSubmit={async (e) => {
                                    e.preventDefault();
                                    setVerifyError(null);
                                    setVerifyLoading(true);
                                    try {
                                        await verifyPassword(verifyPasswordValue);
                                        setIsAccountUnlocked(true);
                                        setVerifyPasswordValue('');
                                    } catch (err) {
                                        setVerifyError(
                                            err instanceof ApiError ? err.message : 'Invalid password.'
                                        );
                                    } finally {
                                        setVerifyLoading(false);
                                    }
                                }}
                            >

                                <h3>Confirm your password</h3>
                                <p className={styles.help}>
                                    Please enter your password to manage account settings.
                                </p>

                                <input
                                    className={styles.input}
                                    style={{ marginTop: '16px', marginBottom: '16px' }}
                                    type="password"
                                    value={verifyPasswordValue}
                                    onChange={(e) => setVerifyPasswordValue(e.target.value)}
                                    placeholder="Password"
                                    required
                                />

                                {verifyError && <p className={styles.error}>{verifyError}</p>}

                                <button
                                    type="submit"
                                    disabled={verifyLoading}
                                    className={`${styles.button} ${styles.buttonPrimary} ${styles.fullWidth}` }
                                >
                                    {verifyLoading ? 'Checking...' : 'Continue'}
                                </button>

                            </form>
                        )
                    )}

                </div>
            </aside>
        </>
    );
};