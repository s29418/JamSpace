import React, { useState } from 'react';
import { createTeam } from '../services/teams.service';
import styles from '../pages/TeamsPage.module.css';

interface Props {
    onTeamCreated: () => void;
}

const CreateTeamForm: React.FC<Props> = ({ onTeamCreated }) => {
    const [teamName, setTeamName] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!teamName.trim()) return;

        setLoading(true);
        try {
            await createTeam({ name: teamName, teamPictureUrl: null });
            setTeamName('');
            onTeamCreated();
        } catch (error) {
            alert('Error creating team.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div>
            <form onSubmit={handleSubmit}>
                <input
                    type="text"
                    placeholder="Team name"
                    value={teamName}
                    onChange={(e) => setTeamName(e.target.value)}
                    disabled={loading}
                />
            </form>

            <hr className={styles.lineBreak}/>
        </div>
    );
};

export default CreateTeamForm;
