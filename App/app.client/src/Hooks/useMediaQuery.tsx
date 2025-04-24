import { useState, useEffect } from 'react';

export const useMediaQuery = (query: string): boolean => {
    const [isMatch, setIsMatch] = useState<boolean>(() =>
        typeof window !== 'undefined' &&
        window.matchMedia(query).matches);

    useEffect(() => {
        if (typeof window === 'undefined') {
            return;
        }
        const mediaQuery = window.matchMedia(query);
        const handleChange = (e: MediaQueryListEvent): void => {
            setIsMatch(e.matches);
        };

        setIsMatch(mediaQuery.matches);

        mediaQuery.addEventListener('change', handleChange);
        return () => {
            mediaQuery.removeEventListener('change', handleChange);
        };
    }, [query]);

    return isMatch;
}
