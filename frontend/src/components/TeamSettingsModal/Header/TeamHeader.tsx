import React, {useEffect, useState} from 'react';
import styles from '../TeamSettingsModal.module.css';
import { EditableTeamName } from './EditableTeamName';
import { AvatarUploader } from './AvatarUploader';
import {TrashIcon, ArrowRightStartOnRectangleIcon} from '@heroicons/react/24/outline';

type Props = {
    teamName: string;
    teamPictureUrl?: string | null;

    canRename?: boolean;
    canChangePicture?: boolean;
    canDeleteTeam?: boolean;
    canLeaveTeam?: boolean;

    onRename: (newName: string) => void | Promise<void>;
    onChangePicture: (file: File) => void | Promise<void>;
    onDeleteTeam?: () => void | Promise<void>;
    onLeaveTeam?: () => void | Promise<void>;

    className?: string;
};

export const TeamHeader: React.FC<Props> = ({
                                                teamName,
                                                teamPictureUrl,
                                                canRename = false,
                                                canChangePicture = false,
                                                canDeleteTeam = false,
                                                canLeaveTeam = false,
                                                onRename,
                                                onChangePicture,
                                                onDeleteTeam,
                                                onLeaveTeam,
                                                className,
                                            }) => {
    const [confirm, setConfirm] = useState<null | { type: 'delete' | 'leave' }>(null);

    useEffect(() => {
        if (!confirm) return;
        const onKey = (e: KeyboardEvent) => { if (e.key === 'Escape') setConfirm(null); };
        window.addEventListener('keydown', onKey);
        return () => window.removeEventListener('keydown', onKey);
    }, [confirm]);

    return (
        <header className={className}>
            <EditableTeamName
                name={teamName}
                canEdit={canRename}
                onSave={onRename}
                rightActions={
                    <>
                        {canLeaveTeam && (
                            <button
                                type="button"
                                className={styles.editButton}
                                onClick={() => setConfirm({type: 'leave'})}
                                aria-haspopup="dialog">
                                <ArrowRightStartOnRectangleIcon className={styles.icon}/>Leave team
                            </button>
                        )}
                        {canDeleteTeam && (
                            <button
                                type="button"
                                className={styles.deleteButton}
                                onClick={() => setConfirm({type: 'delete'})}
                                aria-haspopup="dialog">
                                <TrashIcon className={styles.icon}/> Delete team
                            </button>
                        )}
                    </>
                }
            />


            {confirm && (
                <div
                    id="confirm-dialog"
                    role="dialog"
                    aria-modal="true"
                    aria-label={confirm.type === 'delete' ? 'Confirm delete team' : 'Confirm leave team'}
                    className={styles.confirmBackdrop}
                    onClick={() => setConfirm(null)}
                >
                    <div
                        className={styles.confirmBody}
                        onClick={(e) => e.stopPropagation()}
                    >
                        <p className={styles.confirmContent}>
                            {confirm.type === 'delete'
                                ? 'Are you sure you want to delete this team?'
                                : 'Are you sure you want to leave this team?'}
                        </p>

                        <div className={styles.editButtonsRow}>
                            <button
                                type="button"
                                className={styles.nameActionButton}
                                onClick={async () => {
                                    if (confirm.type === 'delete') await onDeleteTeam?.();
                                    if (confirm.type === 'leave') await onLeaveTeam?.();
                                    setConfirm(null);
                                }}
                                aria-label="Confirm action"
                            >
                                Confirm
                            </button>
                            <button
                                type="button"
                                className={styles.nameActionButton}
                                onClick={() => setConfirm(null)}
                                aria-label="Cancel action"
                            >
                                Cancel
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </header>
    );
};
