import React, { useState } from 'react';
import styles from '../TeamSettingsModal.module.css';
import type { TeamRoleLabel } from 'entities/team/model/types';

type Props = {
    onPick: (role: TeamRoleLabel) => void | Promise<void>;
    onClose: () => void;
    initialValue?: TeamRoleLabel;
};

export const RoleSelect: React.FC<Props> = ({ onPick, onClose, initialValue }) => {
    const [value, setValue] = useState<TeamRoleLabel | ''>(initialValue ?? '');

    const canSave = value !== '';

    return (
        <div>
            <select
                className={styles.roleSelect}
                value={value}
                onChange={(e) => setValue(e.target.value as TeamRoleLabel)}
                aria-label="Select new team role"
            >
                <option value="" disabled>
                    Select role
                </option>
                <option value="Member">Member</option>
                <option value="Admin">Admin</option>
                <option value="Leader">Leader</option>
            </select>

            <div className={styles.editButtonsRow}>
                <button
                    className={styles.userActionButton}
                    type="button"
                    onClick={() => canSave && onPick(value as TeamRoleLabel)}
                    disabled={!canSave}
                    aria-label="Save new role"
                >
                    Save
                </button>
                <button
                    className={styles.userActionButton}
                    type="button"
                    onClick={onClose}
                    aria-label="Cancel"
                >
                    Cancel
                </button>
            </div>
        </div>
    );
};
