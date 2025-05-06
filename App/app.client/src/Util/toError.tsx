/**
 * Converts unknown throw into proper error for type safety.
 * @param err error with unknown type.
 * @returns Error with error type.
 */
export const toError = (err: unknown): Error => {
    return err instanceof Error ? err : new Error(String(err));
};