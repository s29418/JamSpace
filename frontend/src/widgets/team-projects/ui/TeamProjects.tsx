import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import {
    PlusIcon,
    ArrowTopRightOnSquareIcon,
    PencilSquareIcon
} from '@heroicons/react/24/outline';
import { useTeamProjects } from 'features/team/team-projects/model/useTeamProjects';
import type { TeamProject } from 'entities/team/model/types';
import ProjectEditModal from './ProjectEditModal';
import styles from './TeamProjects.module.css';

type Props = {
    teamId: string;
    maxHeight?: number | null;
};

const getProjectFallback = (name: string) => name.trim().charAt(0).toUpperCase() || '?';

const TeamProjects: React.FC<Props> = ({ teamId, maxHeight = null }) => {
    const {
        projects,
        loading,
        error,
        createProject,
        updateProjectPicture,
        updateProject,
        removeProject,
    } = useTeamProjects(teamId);
    const [isCreateOpen, setIsCreateOpen] = useState(false);
    const [projectToEdit, setProjectToEdit] = useState<TeamProject | null>(null);

    return (
        <aside
            className={styles.panel}
            style={maxHeight ? { height: `${maxHeight}px`, maxHeight: `${maxHeight}px` } : undefined}
        >
            <ProjectEditModal
                isOpen={isCreateOpen}
                project={null}
                mode="create"
                onClose={() => setIsCreateOpen(false)}
                onSave={async ({ name, description, picture }) => {
                    const created = await createProject({ name, description });
                    if (picture) {
                        await updateProjectPicture(created.id, picture);
                    }
                }}
            />

            <ProjectEditModal
                isOpen={!!projectToEdit}
                project={projectToEdit}
                onClose={() => setProjectToEdit(null)}
                onSave={async ({ name, description, picture }) => {
                    if (!projectToEdit) return;

                    await updateProject(projectToEdit.id, { name, description });
                    if (picture) {
                        await updateProjectPicture(projectToEdit.id, picture);
                    }
                }}
                onDelete={async () => {
                    if (!projectToEdit) return;

                    await removeProject(projectToEdit.id);
                }}
            />

            <h2 className={styles.title}>Team Projects</h2>

            <button
                type="button"
                className={styles.createButton}
                onClick={() => setIsCreateOpen(true)}
            >
                <PlusIcon className={styles.buttonIcon} />
                Create new project
            </button>

            <div className={styles.projectsList}>
                {loading && <p className={styles.note}>Loading projects...</p>}
                {!loading && error && <p className={styles.error}>{error}</p>}
                {!loading && !error && projects.length === 0 && (
                    <p className={styles.note}>No projects yet.</p>
                )}

                {!loading && !error && projects.map((project) => (
                    <article
                        key={project.id}
                        className={styles.projectCard}
                    >

                        <div className={styles.projectCardRow}>

                            {project.pictureUrl ? (
                                <img
                                    src={project.pictureUrl}
                                    alt={project.name}
                                    className={styles.projectImage}
                                    loading="lazy"
                                    decoding="async"
                                />
                            ) : (
                                <div className={styles.projectFallback}>
                                    {getProjectFallback(project.name)}
                                </div>
                            )}

                            <div className={styles.projectContent}>
                                <h3 className={styles.projectName} title={project.name}>
                                    {project.name}
                                </h3>

                                <div className={styles.projectButtons}>
                                    <Link
                                        to={`/teams/${teamId}/projects/${project.id}`}
                                        className={styles.openButton}
                                    >
                                        <span className={styles.projectButtonIconBox} aria-hidden="true">
                                            <ArrowTopRightOnSquareIcon className={styles.projectButtonIcon} />
                                        </span>
                                        Open
                                    </Link>

                                    <button
                                        type="button"
                                        className={styles.editButton}
                                        onClick={() => setProjectToEdit(project)}
                                    >
                                        <span className={styles.projectButtonIconBox} aria-hidden="true">
                                            <PencilSquareIcon className={`${styles.projectButtonIcon} ${styles.projectButtonIconEdit}`} />
                                        </span>
                                        Edit
                                    </button>
                                </div>
                            </div>
                        </div>
                    </article>
                ))}
            </div>
        </aside>
    );
};

export default TeamProjects;
