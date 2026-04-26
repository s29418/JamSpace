import React, { useEffect, useRef, useState } from 'react';
import { PlusIcon } from '@heroicons/react/24/outline';
import { useTeamProjects } from 'features/team/team-projects/model/useTeamProjects';
import styles from './TeamProjects.module.css';

type Props = {
    teamId: string;
};

const getProjectFallback = (name: string) => name.trim().charAt(0).toUpperCase() || '?';

const TeamProjects = ({ teamId }: Props) => {
    const { projects, loading, error } = useTeamProjects(teamId);
    const panelRef = useRef<HTMLElement | null>(null);
    const titleRef = useRef<HTMLHeadingElement | null>(null);
    const createButtonRef = useRef<HTMLButtonElement | null>(null);
    const [listHeight, setListHeight] = useState<number | null>(null);

    useEffect(() => {
        const panelElement = panelRef.current;
        const titleElement = titleRef.current;
        const buttonElement = createButtonRef.current;

        if (!panelElement || !titleElement || !buttonElement) {
            setListHeight(null);
            return;
        }

        const updateListHeight = () => {
            if (window.innerWidth <= 900) {
                setListHeight(null);
                return;
            }

            const styles = window.getComputedStyle(panelElement);
            const paddingTop = Number.parseFloat(styles.paddingTop) || 0;
            const paddingBottom = Number.parseFloat(styles.paddingBottom) || 0;
            const titleBottomMargin = Number.parseFloat(window.getComputedStyle(titleElement).marginBottom) || 0;
            const buttonBottomMargin = Number.parseFloat(window.getComputedStyle(buttonElement).marginBottom) || 0;

            const reservedSpace =
                paddingTop +
                paddingBottom +
                titleElement.offsetHeight +
                titleBottomMargin +
                buttonElement.offsetHeight +
                buttonBottomMargin;

            setListHeight(Math.max(panelElement.clientHeight - reservedSpace, 0));
        };

        updateListHeight();

        const observer = new ResizeObserver(() => {
            updateListHeight();
        });

        observer.observe(panelElement);
        observer.observe(titleElement);
        observer.observe(buttonElement);
        window.addEventListener('resize', updateListHeight);

        return () => {
            observer.disconnect();
            window.removeEventListener('resize', updateListHeight);
        };
    }, []);

    return (
        <aside ref={panelRef} className={styles.panel}>
            <h2 ref={titleRef} className={styles.title}>Team Projects</h2>

            <button
                ref={createButtonRef}
                type="button"
                className={styles.createButton}
                disabled
                aria-disabled="true"
            >
                <PlusIcon className={styles.buttonIcon} />
                Create new project
            </button>

            <div
                className={styles.projectsList}
                style={listHeight ? { height: `${listHeight}px`, maxHeight: `${listHeight}px` } : undefined}
            >
                {loading && <p className={styles.note}>Loading projects...</p>}
                {!loading && error && <p className={styles.error}>{error}</p>}
                {!loading && !error && projects.length === 0 && (
                    <p className={styles.note}>No projects yet.</p>
                )}

                {!loading && !error && projects.map((project) => (
                    <article key={project.id} className={styles.projectCard}>
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
                                <h3 className={styles.projectName}>{project.name}</h3>
                                <button
                                    type="button"
                                    className={styles.openButton}
                                    disabled
                                    aria-disabled="true"
                                >
                                    Open
                                </button>
                            </div>
                        </div>
                    </article>
                ))}
            </div>
        </aside>
    );
};

export default TeamProjects;
