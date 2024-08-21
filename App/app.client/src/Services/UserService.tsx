const BASE_URL = '/api/User';
export interface User {
    email: string,
    role: string,
    rangerId: number | undefined
}

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

export const signOut = async () => {
    const response = await fetch(`${BASE_URL}/signout`, { method: 'POST' });

    if (!response.ok) {
        const errorMessage = await response.text();
        throw new Error(errorMessage);
    }
    // log success?
};