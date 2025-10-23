import React, { useRef } from 'react';
import { CameraIcon } from '@heroicons/react/24/outline';
import type { UserProfile } from '../../../../../../entities/user/model/types';
import styles from '../EditProfilePanel.module.css';

export type UpdateUserProfileDraft = {
    setDisplayName?: boolean; displayName?: string;
    setBio?: boolean; bio?: string | null;
    setProfilePicture?: boolean; profilePictureUrl?: string | null;
    setLocation?: boolean; location?: { city?: string | null; country?: string | null };
};

type Props = {
    initial: UserProfile;
    onCancel: () => void;
    onSave: (draft: UpdateUserProfileDraft, file?: File) => void;
};

const BIO_LIMIT = 170;

export const ProfileTab: React.FC<Props> = ({ initial, onCancel, onSave }) => {
    const [avatarUrl, setAvatarUrl]   = React.useState(initial.profilePictureUrl ?? '');
    const [displayName, setDisplayName] = React.useState(initial.displayName ?? '');
    const [city, setCity]             = React.useState(initial.location?.city ?? '');
    const [country, setCountry]       = React.useState(initial.location?.country ?? '');
    const [bio, setBio]               = React.useState(initial.bio ?? '');
    const [err, setErr] = React.useState<{displayName?: string; bio?: string}>({});

    const [file, setFile] = React.useState<File | null>(null);
    const fileInput = useRef<HTMLInputElement | null>(null);
    const shownAvatar = file ? URL.createObjectURL(file) : (avatarUrl || initial.profilePictureUrl || '');
    React.useEffect(() => () => { if (file) URL.revokeObjectURL(shownAvatar); }, [file, shownAvatar]);

    const remaining = BIO_LIMIT - bio.length;

    function handleSave() {
        const e: typeof err = {};
        if (!displayName.trim()) e.displayName = 'Display name cannot be empty.';
        if (bio.length > BIO_LIMIT) e.bio = `Bio has a limit of ${BIO_LIMIT} characters.`;
        setErr(e);
        if (Object.keys(e).length) return;

        const draft: UpdateUserProfileDraft = {};
        const norm = (s: string) => s.trim();

        if (norm(avatarUrl) !== (initial.profilePictureUrl ?? '')) {
            draft.setProfilePicture = true; draft.profilePictureUrl = norm(avatarUrl) || '';
        }
        if (norm(displayName) !== (initial.displayName ?? '')) {
            draft.setDisplayName = true; draft.displayName = norm(displayName);
        }
        if (norm(city) !== (initial.location?.city ?? '') || norm(country) !== (initial.location?.country ?? '')) {
            draft.setLocation = true; draft.location = { city: norm(city) || '', country: norm(country) || '' };
        }
        if (norm(bio) !== (initial.bio ?? '')) {
            draft.setBio = true; draft.bio = norm(bio) || '';
        }

        onSave(draft, file || undefined);
    }

    return (
        <form className={styles.form} onSubmit={(e) => {
            e.preventDefault();
            handleSave();
        }}>
            {/* Profile picture (URL) + preview */}
            <div className={styles.formRow}>
                <label className={styles.labelCenter}>Profile picture</label>

                <div
                    className={styles.avatarWrapper}
                    role="button"
                    tabIndex={0}
                    aria-label="Change profile picture"
                    onClick={() => fileInput.current?.click()}
                    onKeyDown={(e) => {
                        if (e.key === 'Enter' || e.key === ' ') {
                            e.preventDefault();
                            fileInput.current?.click();
                        }
                    }}
                >
                    {shownAvatar ? (
                        <img src={shownAvatar} alt="avatar preview" className={styles.avatarModal}/>
                    ) : (
                        <div className={styles.avatarModal}/>
                    )}
                    <CameraIcon className={styles.cameraIcon} aria-hidden="true"/>
                </div>

                <input
                    ref={fileInput}
                    type="file"
                    accept="image/*"
                    onChange={(e) => {
                        const f = e.target.files?.[0] ?? null;
                        setFile(f);
                        setAvatarUrl('');
                    }}
                    className={styles.hiddenInput}
                    aria-hidden="true"
                />
            </div>

            {/* Display name */}
            <div className={styles.formRow}>
                <label className={styles.label}>Display name</label>
                <input className={`${styles.input} ${err.displayName ? styles.inputError : ''}`} value={displayName}
                       onChange={(e) => setDisplayName(e.target.value)}/>
                {err.displayName && <p className={`${styles.help} ${styles.error}`}>{err.displayName}</p>}
            </div>

            {/* Location */}
            <div className={styles.formRow}>
                <label className={styles.label}>Location</label>
                <div className={styles.twoCol}>
                    <input className={styles.input} value={city} onChange={(e) => setCity(e.target.value)}
                           placeholder="City (np. Warsaw)"/>
                    <input className={styles.input} value={country} onChange={(e) => setCountry(e.target.value)}
                           placeholder="Country (np. Poland)"/>
                </div>
            </div>

            {/* Bio */}
            <div className={styles.formRow}>
                <label className={styles.label}>Bio</label>
                <textarea className={`${styles.textarea} ${err.bio ? styles.inputError : ''}`} rows={4}
                          maxLength={BIO_LIMIT} value={bio} onChange={(e) => setBio(e.target.value)}/>
                <div className={styles.rowBetween}>
                    <p className={`${styles.help} ${err.bio ? styles.error : ''}`}>{err.bio ?? `Max ${BIO_LIMIT} characters.`}</p>
                    <span className={styles.counter}>{remaining}</span>
                </div>
            </div>

            {/* Save / Cancel buttons */}
            <div className={styles.saveBar}>
                <button type="button" className={`${styles.button} ${styles.buttonGhost}`} onClick={onCancel}>Cancel
                </button>
                <button type="submit" className={`${styles.button} ${styles.buttonPrimary}`}>Save</button>
            </div>
        </form>
    );
};
