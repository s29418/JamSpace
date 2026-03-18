import React, { useState } from "react";
import styles from "./FiltersDrawer.module.css";
import Chips from "../Chips/Chips";

import {
    XMarkIcon as ClearIcon,
    PlusIcon as AddIcon
} from '@heroicons/react/24/outline';

type Props = {
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

    const handleAddSkill = () => {
        onAddSkill(skillInput);
        setSkillInput("");
    };

    const handleAddGenre = () => {
        onAddGenre(genreInput);
        setGenreInput("");
    };

    return (
        <div className={styles.panel}>
            <div className={styles.header}>
                <h3 className={styles.title}>Filters</h3>
            </div>

            <div className={styles.body}>
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
                                    handleAddSkill();
                                }
                            }}
                        />
                        <button className={styles.addBtn} onClick={handleAddSkill} type="button">
                            <AddIcon className={styles.icon}/>
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
                                    handleAddGenre();
                                }
                            }}
                        />
                        <button className={styles.addBtn} onClick={handleAddGenre} type="button">
                            <AddIcon className={styles.icon}/>
                            Add
                        </button>
                    </div>

                    <Chips items={genres} onRemove={onRemoveGenre} />
                </div>
            </div>

            <div className={styles.footer}>
                <button className={styles.clearBtn} onClick={onClear} type="button">
                    <ClearIcon className={styles.icon} />
                    Clear
                </button>
            </div>
        </div>
    );
};

export default FiltersDrawer;
