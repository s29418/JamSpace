import React, {FC, useState} from 'react';
import styles from '../EditProfilePanel.module.css';

type Props = {
    initialEmail?: string | null;
    onUpdateEmail: (email: string) => Promise<void> | void;
    onChangePassword: (current: string, next: string) => Promise<void> | void;
    onDeleteAccount: () => Promise<void> | void;
    onLogoutAll: () => Promise<void> | void;
};

export const AccountTab: FC<Props> = ({
                                                initialEmail,
                                                onUpdateEmail,
                                                onChangePassword,
                                                onDeleteAccount,
                                                onLogoutAll
                                            }) => {

    const [showEmail, setShowEmail] = useState(false);
    const [email, setEmail] = useState(initialEmail ?? '');
    const [emailBusy, setEmailBusy] = useState(false);
    const [emailError, setEmailError] = useState<string | null>(null);

    const [showPw, setShowPw] = useState(false);
    const [current, setCurrent] = useState('');
    const [next, setNext] = useState('');
    const [confirm, setConfirm] = useState('');
    const [pwBusy, setPwBusy] = useState(false);
    const [pwError, setPwError] = useState<string | null>(null);

    const [confirmOpen, setConfirmOpen] = useState(false);

    async function saveEmail() {
        if (!email.trim()) return;
        setEmailBusy(true);
        try {
            await onUpdateEmail(email.trim());
            setShowEmail(false);
            setEmailError('');
        } catch (e: any) {
            setEmailError(e?.message || 'Failed to update email.');
        }
        finally {
            setEmailBusy(false);
        }
    }

    async function savePassword() {
        setPwError(null);
        if (!current || !next || !confirm) {
            setPwError('All fields are required.');
            return;
        }
        if (next !== confirm) {
            setPwError('Passwords are not the same.');
            return;
        }
        setPwBusy(true);
        try {
            await onChangePassword(current, next);
            setCurrent('');
            setNext('');
            setConfirm('');
            setShowPw(false);
        } catch (e: any) {
            setPwError(e?.message || 'Failed to change password.');
        }
        finally {
            setPwBusy(false);
        }
    }

    return (
        <div className={styles.accountTab}>

            {/* Email */}
            <div className={styles.section}>
                <div className={styles.sectionHeader}>

                    <h3>Email</h3>
                    <button
                        className={`${styles.button} ${styles.buttonGhost}`}
                        onClick={() => setShowEmail(v => !v)}
                        type="button"
                    >{showEmail ? 'Cancel' : 'Change email'}</button>
                </div>

                {showEmail && (
                    <div className={styles.inlineRow}>

                        <input
                            className={styles.input}
                            type="email"
                            value={email}
                            onChange={e => setEmail(e.target.value)}
                            placeholder="name@example.com"
                        />

                        <button
                            className={`${styles.button} ${styles.buttonPrimary}`}
                            onClick={() => void saveEmail()}
                            disabled={emailBusy || !email.trim()}
                            type="button"
                        >Save
                        </button>

                        {emailError &&
                            <p className={`${styles.help} ${styles.error}`}>
                                {emailError}
                            </p>
                        }
                    </div>
                )}
            </div>


            {/* Password */}
            <div className={styles.section}>
                <div className={styles.sectionHeader}>

                    <h3>Password</h3>
                    <button
                        className={`${styles.button} ${styles.buttonGhost}`}
                        onClick={() => setShowPw(v => !v)}
                        type="button"
                    >{showPw ? 'Cancel' : 'Change password'}</button>

                </div>

                {showPw && (
                    <div className={styles.stack}>

                        <input
                            className={styles.input}
                            type="password"
                            placeholder="Current password"
                            value={current}
                            onChange={e => setCurrent(e.target.value)}
                        />
                        <input
                            className={styles.input}
                            type="password"
                            placeholder="New password"
                            value={next}
                            onChange={e => setNext(e.target.value)}
                        />
                        <input
                            className={styles.input}
                            type="password"
                            placeholder="Confirm new password"
                            value={confirm}
                            onChange={e => setConfirm(e.target.value)}
                        />

                        {pwError && <p className={`${styles.help} ${styles.error}`}>{pwError}</p>}

                        <div className={styles.alignRight}>
                            <button
                                className={`${styles.button} ${styles.buttonPrimary}`}
                                onClick={() => void savePassword()}
                                disabled={pwBusy}
                                type="button"
                            >Save password</button>
                        </div>

                    </div>
                )}
            </div>

            {/* Logout all */}
            <div className={styles.section}>
                {/*<div className={styles.sectionHeader}>*/}
                {/*    <h3>Sessions</h3>*/}
                {/*</div>*/}

                <button
                    className={`${styles.button} ${styles.buttonGhost}`}
                    type="button"
                    onClick={() => void onLogoutAll?.()}
                >
                    Log out from all devices
                </button>

                {/*<p className={styles.help}>*/}
                {/*    This will sign you out on all browsers and devices.*/}
                {/*</p>*/}
            </div>

            {/* Delete account */}
            <div className={styles.section}>

                {/*<div className={styles.sectionHeader}>*/}
                {/*    <h3>Delete account</h3>*/}
                {/*</div>*/}

                <button
                    className={`${styles.button} ${styles.buttonDanger}`}
                    onClick={() => setConfirmOpen(true)}
                    type="button"
                >Delete my account</button>

                {confirmOpen && (
                    <div
                        className={styles.confirmBackdrop}
                        onClick={() => setConfirmOpen(false)}
                        role="dialog"
                        aria-modal="true"
                        aria-label="Confirm delete account"
                    >

                        <div className={styles.confirmBody} onClick={e => e.stopPropagation()}>

                            <p className={styles.confirmContent}>
                                Are you sure you want to delete your account? This action cannot be undone.
                            </p>

                            <div className={styles.editButtonsRow}>

                                <button
                                    type="button"
                                    className={`${styles.button} ${styles.buttonDanger}`}
                                    onClick={async () => { await onDeleteAccount(); }}
                                >Confirm</button>
                                <button
                                    type="button"
                                    className={`${styles.button} ${styles.buttonGhost}`}
                                    onClick={() => setConfirmOpen(false)}
                                >Cancel</button>
                            </div>
                        </div>
                    </div>
                )}

                {/*<p className={styles.help}>*/}
                {/*    This will permanently delete your account and remove your data from our servers.*/}
                {/*</p>*/}
            </div>
        </div>
    );
};
