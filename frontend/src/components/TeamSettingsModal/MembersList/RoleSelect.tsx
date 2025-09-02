import React, { useState } from 'react';
import styles from '../TeamSettingsModal.module.css';
import { TeamRoleCode } from '../../../types/team';

type Props = {
    onPick: (role: TeamRoleCode) => void | Promise<void>;
    onClose: () => void;
    initialValue?: TeamRoleCode;
};

export const RoleSelect: React.FC<Props> = ({ onPick, onClose, initialValue }) => {
    const [value, setValue] = useState<TeamRoleCode | ''>(initialValue ?? '');

    return (
        <div>
            <select
                className={styles.roleSelect}
                value={value}
                onChange={(e) => setValue(Number(e.target.value) as TeamRoleCode)}
                aria-label="Select new team role"
            >
                <option value="" disabled>
                    Select role
                </option>
                <option value={TeamRoleCode.Member}>Member</option>
                <option value={TeamRoleCode.Admin}>Admin</option>
                <option value={TeamRoleCode.Leader}>Leader</option>
            </select>

            <div className={styles.editButtonsRow} style={{ marginTop: '8px' }}>
                <button
                    className={styles.userActionButton}
                    type="button"
                    onClick={() => value !== '' && onPick(value as TeamRoleCode)}
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
