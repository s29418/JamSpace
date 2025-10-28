import React, { useEffect, useRef, useState } from 'react';
import { CameraIcon } from '@heroicons/react/24/outline';
import type { UserProfile } from '../../../../../../entities/user/model/types';
import styles from '../EditProfilePanel.module.css';
import { ApiError, isApiError } from '../../../../../../shared/lib/api/base';

export type UpdateUserProfileDraft = {
    setDisplayName?: boolean;
    displayName?: string;
    setBio?: boolean;
    bio?: string | null;
    setProfilePicture?: boolean;
    profilePictureUrl?: string | null;
    setLocation?: boolean;
    location?: {
        city?: string | null;
        country?: string | null
    };
};

type Props = {
    initial: UserProfile;
    onCancel: () => void;
    onSave: (draft: UpdateUserProfileDraft, file?: File) => Promise<void>;
};

const BIO_LIMIT = 170;

type FieldErr = {
    displayName?: string;
    bio?: string;
    location?: string;
    _global?: string
};

export const ProfileTab: React.FC<Props> = ({
                                                initial,
                                                onCancel,
                                                onSave
}) => {
    const [avatarUrl, setAvatarUrl] = useState(initial.profilePictureUrl ?? '');
    const [displayName, setDisplayName] = useState(initial.displayName ?? '');
    const [city, setCity] = useState(initial.location?.city ?? '');
    const [country, setCountry] = useState(initial.location?.country ?? '');
    const [bio, setBio] = useState(initial.bio ?? '');

    const [err, setErr] = useState<FieldErr>({});
    const [submitting, setSubmitting] = useState(false);

    const [file, setFile] = useState<File | null>(null);
    const fileInput = useRef<HTMLInputElement | null>(null);
    const shownAvatar = file
        ? URL.createObjectURL(file)
        : (avatarUrl || initial.profilePictureUrl || '');

    useEffect(() => {
        return () => {
            if (file)
                URL.revokeObjectURL(shownAvatar);
        };
    }, [file, shownAvatar]);

    const remaining = BIO_LIMIT - (bio?.length ?? 0);

    function mapDetailsToFormErrors(details?: Record<string, string[]>): FieldErr {
        if (!details) return {};
        const first = (k: string) => details[k]?.[0];

        const out: FieldErr = {};
        out.displayName = first('DisplayName') ?? out.displayName;
        out.bio        = first('Bio') ?? out.bio;

        const locCity    = first('Location.City');
        const locCountry = first('Location.Country');
        if (locCity || locCountry) out.location = locCity || locCountry;

        if (!out.displayName && !out.bio && !out.location) {
            const anyFirst = Object.values(details).flat()[0];
            if (anyFirst) out._global = anyFirst;
        }
        return out;
    }

    async function handleSave() {
        const e: FieldErr = {};
        if (!displayName.trim()) e.displayName = 'Display name cannot be empty.';
        if ((bio?.length ?? 0) > BIO_LIMIT) e.bio = `Bio has a limit of ${BIO_LIMIT} characters.`;
        setErr(e);
        if (Object.keys(e).length) return;

        const draft: UpdateUserProfileDraft = {};
        const norm = (s: string) => (s ?? '').trim();

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

        try {
            setSubmitting(true);
            setErr({});
            await onSave(draft, file || undefined);
        } catch (ex) {
            if (isApiError(ex)) {
                const api = ex as ApiError;
                const mapped = mapDetailsToFormErrors(api.details);
                setErr(prev => ({ ...prev, ...mapped, _global: mapped._global || api.message || prev._global }));
            } else {
                setErr(prev => ({ ...prev, _global: (ex as any)?.message || 'Failed to update profile.' }));
            }
        } finally {
            setSubmitting(false);
        }
    }

    return (
        <form
            className={styles.form}
            onSubmit={(e) => { e.preventDefault(); void handleSave(); }}
            noValidate
        >

            {/* Profile picture */}
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
                        <img src={shownAvatar} alt="avatar preview" className={styles.avatarModal} />
                    ) : (
                        <div className={styles.avatarModal} />
                    )}
                    <CameraIcon className={styles.cameraIcon} aria-hidden="true" />
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
                <input
                    className={`${styles.input} ${err.displayName ? styles.inputError : ''}`}
                    value={displayName}
                    onChange={(e) => {
                        setDisplayName(e.target.value);
                        if (err.displayName)
                            setErr(prev => ({ ...prev, displayName: undefined }));
                    }}
                    aria-invalid={!!err.displayName}
                />
                {err.displayName &&
                    <p className={`${styles.help} ${styles.error}`}>
                        {err.displayName}
                    </p>
                }
            </div>

            {/* Location */}
            <div className={styles.formRow}>
                <label className={styles.label}>Location</label>
                <div className={styles.twoCol}>

                    <input
                        className={styles.input}
                        value={city}
                        onChange={(e) => setCity(e.target.value)}
                        placeholder="City (np. Warsaw)"
                    />

                    <input
                        className={styles.input}
                        value={country}
                        onChange={(e) => setCountry(e.target.value)}
                        placeholder="Country (np. Poland)"
                    />

                </div>
                {err.location &&
                    <p className={`${styles.help} ${styles.error}`}>
                        {err.location}
                    </p>
                }
            </div>

            {/* Bio */}
            <div className={styles.formRow}>
                <label className={styles.label}>Bio</label>

                <textarea
                    className={`${styles.textarea} ${err.bio ? styles.inputError : ''}`}
                    rows={4}
                    maxLength={BIO_LIMIT}
                    value={bio}
                    onChange={(e) => {
                        setBio(e.target.value);
                        if (err.bio)
                            setErr(prev => ({ ...prev, bio: undefined }));
                    }}
                    aria-invalid={!!err.bio}
                />

                <div className={styles.rowBetween}>
                    <p className={`${styles.help} ${err.bio ? styles.error : ''}`}>
                        {err.bio ?? `Max ${BIO_LIMIT} characters.`}
                    </p>
                    <span className={styles.counter}>
                        {remaining}
                    </span>
                </div>
            </div>

            {/* Save / Cancel */}
            <div className={styles.saveBar}>
                <button
                    type="button"
                    className={`${styles.button} ${styles.buttonGhost}`}
                    onClick={onCancel}
                    disabled={submitting}
                >
                    Cancel
                </button>

                <button
                    type="submit"
                    className={`${styles.button} ${styles.buttonPrimary}`}
                    disabled={submitting}
                >
                    {submitting ? 'Saving…' : 'Save'}
                </button>
            </div>
        </form>
    );
};
