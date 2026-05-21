import React from 'react';
import { PencilSquareIcon } from '@heroicons/react/24/outline';
import type { TeamProject } from 'entities/team/model/types';
import { formatDate, getProjectFallback } from 'entities/team/lib/teamProjectFormatters';
import styles from './ProjectHeader.module.css';

type ProjectHeaderProps = {
    project: TeamProject;
    onEdit: () => void;
};

const ProjectHeader: React.FC<ProjectHeaderProps> = ({ project, onEdit }) => (
    <header className={styles.header}>
        <div className={styles.projectArtwork}>
            {project.pictureUrl ? (
                <img src={project.pictureUrl} alt={project.name} />
            ) : (
                <span>{getProjectFallback(project.name)}</span>
            )}
        </div>

        <div className={styles.headerText}>
            <h1 className={styles.title}>{project.name}</h1>
            {project.description && <p className={styles.description}>{project.description}</p>}
            <p className={styles.meta}>Updated {formatDate(project.updatedAt)}</p>

            <button
                type="button"
                className={styles.editProjectButton}
                onClick={onEdit}
            >
                <PencilSquareIcon className={styles.icon} />
                Edit
            </button>
        </div>
    </header>
);

export default ProjectHeader;
