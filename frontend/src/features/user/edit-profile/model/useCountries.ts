import { useEffect, useState } from 'react';
import {CountryDto, getCountries, mapByCode} from "../../../../entities/user/api/countries.api";


export function useCountries() {
    const [list, setList] = useState<CountryDto[]>([]);
    const [map, setMap] = useState<Record<string,string>>({});
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        let alive = true;
        (async () => {
            try {
                const data = await getCountries();
                if (!alive) return;
                setList(data);
                setMap(mapByCode(data));
            } catch (e: any) {
                if (!alive) return;
                setError(e?.message ?? String(e));
            } finally {
                if (alive) setLoading(false);
            }
        })();
        return () => { alive = false; };
    }, []);

    return { list, map, loading, error };
}