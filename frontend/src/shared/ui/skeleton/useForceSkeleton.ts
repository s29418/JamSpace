//helper for forcing skeleton loading state

import { useSearchParams } from 'react-router-dom';
export function useForceSkeleton() {
    const [q] = useSearchParams();
    return q.get('skeleton') === '1';
}
