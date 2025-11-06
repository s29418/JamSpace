export type CountryDto = {
    code: string;
    name: string
};

const ROOT = 'http://localhost:5072/api/metadata/countries';

export async function getCountries() {
    const res = await fetch(ROOT, { method: 'GET', cache: 'no-store' });
    if (!res.ok)
        throw new Error(`${res.status} ${res.statusText}`);
    return (await res.json()) as {
        code: string;
        name: string
    }[];
}

export function mapByCode(list: CountryDto[]): Record<string, string> {
    const map: Record<string, string> = {};
    for (const c of list) map[c.code.toUpperCase()] = c.name;
    return map;
}