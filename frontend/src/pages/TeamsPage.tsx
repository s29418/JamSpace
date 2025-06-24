import React, { useEffect, useState } from 'react';
import {
    getMyTeams,
    createTeam,

} from '../services/teamService';
import TeamCard from '../components/TeamCard';
import styles from './TeamsPage.module.css';

interface Member {
    userId: string;
    username: string;
    role: string;
}

interface Team {
    id: string;
    name: string;
    teamPictureUrl?: string;
    members: Member[];
}

const TeamsPage = () => {
    const [teams, setTeams] = useState<Team[]>([]);
    const [showForm, setShowForm] = useState(false);
    const [teamName, setTeamName] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await createTeam({ name: teamName, teamPictureUrl: null });
            setTeamName('');
            setShowForm(false);
            const data = await getMyTeams();
            setTeams(data);
        } catch (err) {
            console.error('Failed to create team:', err);
        }
    };

    useEffect(() => {
        const fetchTeams = async () => {
            try {
                const data = await getMyTeams();
                setTeams(data);
            } catch (err) {
                console.error('Failed to fetch teams:', err);
            }
        };

        fetchTeams();
    }, []);

    return (
        <div className={styles.wrapper}>
            <div className={styles.header}>
                <h1 className={styles.title}>Teams</h1>

                <button className={styles.createButton} onClick={() => setShowForm(true)}>
                    + Create new team
                </button>
            </div>

            {showForm && (
                <form className={styles.form} onSubmit={handleSubmit}>
                    <input
                        type="text"
                        value={teamName}
                        onChange={(e) => setTeamName(e.target.value)}
                        placeholder="Team name"
                        required
                    />
                    <button type="submit">Create</button>
                    <button type="button" onClick={() => setShowForm(false)}>Cancel</button>
                </form>
            )}

            <hr className={styles.lineBreak} />

            {teams.map(team => (
                <TeamCard key={team.id} name={team.name} teamPictureUrl={team.teamPictureUrl} members={team.members}/>
            ))}
        </div>
    );
};

export default TeamsPage;
