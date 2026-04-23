import React, {useState} from 'react';
import styles from '../TeamSettingsModal.module.css';
import { EditableTeamName } from './EditableTeamName';
import {TrashIcon, ArrowRightStartOnRectangleIcon} from '@heroicons/react/24/outline';
import ConfirmDialog from 'shared/ui/confirm-dialog/ConfirmDialog';

type Props = {
    teamName: string;
    teamPictureUrl?: string | null;

    canRename?: boolean;
    canChangePicture?: boolean;
    canDeleteTeam?: boolean;
    canLeaveTeam?: boolean;

    onRename: (newName: string) => void | Promise<void>;
    onChangePicture: (file: File) => void | Promise<void>;
    onDeleteTeam?: () => Promise<void>;
    onLeaveTeam?: () => Promise<void>;

    className?: string;
};

export const TeamHeader: React.FC<Props> = ({
                                                teamName,
                                                canRename = false,
                                                canDeleteTeam = false,
                                                canLeaveTeam = false,
                                                onRename,
                                                onDeleteTeam,
                                                onLeaveTeam,
                                                className,
                                            }) => {
    const [confirm, setConfirm] = useState<null | { type: 'delete' | 'leave' }>(null);

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


            <ConfirmDialog
                isOpen={!!confirm}
                message={
                    confirm?.type === 'delete'
                        ? 'Are you sure you want to delete this team?'
                        : 'Are you sure you want to leave this team?'
                }
                onConfirm={async () => {
                    if (confirm?.type === 'delete') await onDeleteTeam?.();
                    if (confirm?.type === 'leave') await onLeaveTeam?.();
                    setConfirm(null);
                }}
                onCancel={() => setConfirm(null)}
            />
        </header>
    );
};
