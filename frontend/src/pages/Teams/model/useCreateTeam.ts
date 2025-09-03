import { useState } from 'react';
import { createTeam, changeTeamPicture } from 'entities/team/api/teams.api';
import { ApiError, isApiError } from 'shared/lib/api/base';

type Options = {
    onDone?: () => void | Promise<void>;          // np. odświeżenie listy
};

export function useCreateTeam(opts: Options = {}) {
    const { onDone } = opts;
    const [loading, setLoading] = useState(false);
    const [error, setError]   = useState<string | null>(null);

    async function create(name: string, picture?: File | null) {
        setLoading(true);
        setError(null);
        try {
            const created = await createTeam({ name: name.trim(), teamPictureUrl: null });
            if (picture) {
                await changeTeamPicture(created.id, picture);
            }
            if (onDone) await onDone();
            return created;
        } catch (e) {
            const msg = isApiError(e) ? (e as ApiError).message : 'Error creating team';
            setError(msg);
            throw e;
        } finally {
            setLoading(false);
        }
    }

    return { create, loading, error };
}
