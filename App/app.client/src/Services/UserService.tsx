const BASE_URL = '/api/User';
export interface User {
    email: string,
    role: string,
    rangerId: number | undefined
}

/**
 * Sign in user with email and password.
 * @param email Email of the user.
 * @param password Password of the user
 * @returns A Promise of signed in User.
 */
export const signIn = async (email: string, password: string): Promise<User> => {
    const response = await fetch(`${BASE_URL}/signin`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ email, password })
    });

    if (!response.ok) {
        const errorMessage = await response.text();
        throw new Error(errorMessage);
    }
    const result = await response.json();
    return result;
};

/** Sign out user.*/
export const signOut = async () => {
    const response = await fetch(`${BASE_URL}/signout`, { method: 'POST' });

    if (!response.ok) {
        const errorMessage = await response.text();
        throw new Error(errorMessage);
    }
};

/**
 * Get currently signed in user.
 * @returns A promise of user.
 */
export const getCurrentUser = async (): Promise<User> => {
    const response = await fetch(`${BASE_URL}`);

    if (!response.ok) {
        const errorMessage = await response.text();
        throw new Error(errorMessage);
    }

    const result = await response.json();
    return result;
}