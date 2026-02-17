import React, { useEffect, useState } from "react";
import ReactDOM from "react-dom";
import styles from "./FiltersDrawer.module.css";
import Chips from "../Chips/Chips";

type Props = {
    open: boolean;
    onClose: () => void;

    location: string;
    onLocationChange: (v: string) => void;

    skills: string[];
    onAddSkill: (v: string) => void;
    onRemoveSkill: (v: string) => void;

    genres: string[];
    onAddGenre: (v: string) => void;
    onRemoveGenre: (v: string) => void;

    onClear: () => void;
};

const FiltersDrawer: React.FC<Props> = ({
                                            open,
                                            onClose,
                                            location,
                                            onLocationChange,
                                            skills,
                                            onAddSkill,
                                            onRemoveSkill,
                                            genres,
                                            onAddGenre,
                                            onRemoveGenre,
                                            onClear,
                                        }) => {
    const [skillInput, setSkillInput] = useState("");
    const [genreInput, setGenreInput] = useState("");

    useEffect(() => {
        if (!open) return;
        const onKey = (e: KeyboardEvent) => {
            if (e.key === "Escape") onClose();
        };
        window.addEventListener("keydown", onKey);
        return () => window.removeEventListener("keydown", onKey);
    }, [open, onClose]);

    if (!open) return null;
    const modalRoot = document.getElementById("modal-root");
    if (!modalRoot) return null;

    const body = (
        <div className={styles.overlay} onMouseDown={onClose}>
            <div className={styles.panel} onMouseDown={(e) => e.stopPropagation()}>
                <div className={styles.header}>
                    <h3 className={styles.title}>Filters</h3>
                    <button className={styles.close} onClick={onClose} aria-label="Close filters">
                        ✕
                    </button>
                </div>

                <div className={styles.section}>
                    <div className={styles.label}>Location:</div>
                    <input
                        className={styles.input}
                        placeholder="City / Country / City, Country"
                        value={location}
                        onChange={(e) => onLocationChange(e.target.value)}
                    />
                </div>

                <div className={styles.section}>
                    <div className={styles.label}>Skills:</div>

                    <div className={styles.addRow}>
                        <input
                            className={styles.input}
                            placeholder="Type..."
                            value={skillInput}
                            onChange={(e) => setSkillInput(e.target.value)}
                            onKeyDown={(e) => {
                                if (e.key === "Enter") {
                                    onAddSkill(skillInput);
                                    setSkillInput("");
                                }
                            }}
                        />
                        <button
                            className={styles.addBtn}
                            onClick={() => {
                                onAddSkill(skillInput);
                                setSkillInput("");
                            }}
                        >
                            Add
                        </button>
                    </div>

                    <Chips items={skills} onRemove={onRemoveSkill} />
                </div>

                <div className={styles.section}>
                    <div className={styles.label}>Genres:</div>

                    <div className={styles.addRow}>
                        <input
                            className={styles.input}
                            placeholder="Type..."
                            value={genreInput}
                            onChange={(e) => setGenreInput(e.target.value)}
                            onKeyDown={(e) => {
                                if (e.key === "Enter") {
                                    onAddGenre(genreInput);
                                    setGenreInput("");
                                }
                            }}
                        />
                        <button
                            className={styles.addBtn}
                            onClick={() => {
                                onAddGenre(genreInput);
                                setGenreInput("");
                            }}
                        >
                            Add
                        </button>
                    </div>

                    <Chips items={genres} onRemove={onRemoveGenre} />
                </div>

                <div className={styles.footer}>
                    <button className={styles.clearBtn} onClick={onClear}>
                        Clear
                    </button>
                </div>
            </div>
        </div>
    );

    return ReactDOM.createPortal(body, modalRoot);
};

export default FiltersDrawer;
