import { describe, it, expect, afterEach, vi } from 'vitest';
import { signIn, signOut, getCurrentUser, User } from '../UserService';

describe('UserService', () => {
    afterEach(() => {
        vi.restoreAllMocks();
    });

    const mockUser: User = {
        email: 'test@gmail.com',
        role: 'HeadOfDistrict',
        rangerId: 1
    };

    describe('signIn', () => {
        it('should sign in a user and return the user', async () => {
            // arrange mocking successful signin fetch
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: true,
                json: async () => mockUser
            }));

            // act
            const result = await signIn('test@gmail.com', 'password123');

            // assert
            expect(result).toEqual(mockUser);
            expect(fetch).toHaveBeenCalledWith(
                `/api/User/signin`,
                expect.objectContaining({
                    method: 'POST',
                    headers: expect.objectContaining({
                        'Content-Type': 'application/json'
                    }),
                    body: JSON.stringify({ email: 'test@gmail.com', password: 'password123' })
                })
            );
        });

        it('should throw an error if response is not ok', async () => {
            // arrange
            const errorMessage = 'Sign in failed.';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => errorMessage
            }));

            // act + assert
            await expect(signIn('test@gmail.com', 'password123')).rejects.toThrow(errorMessage);
        });
    });

    describe('signOut', () => {
        it('should sign out a use', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({ ok: true }));

            // act
            await signOut();

            // assert
            expect(fetch).toHaveBeenCalledWith(
                `/api/User/signout`,
                { method: 'POST' }
            );
        });

        it('should throw an error if response is not ok', async () => {
            // arrange
            const errorMessage = 'Sign out failed';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => errorMessage
            }));

            // act + assert
            await expect(signOut()).rejects.toThrow(errorMessage);
        });
    });

    describe('getCurrentUser', () => {
        it('should return the current signed-in user', async () => {
            // arrange
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: true,
                json: async () => mockUser
            }));

            // act
            const result = await getCurrentUser();

            // assert
            expect(result).toEqual(mockUser);
            expect(fetch).toHaveBeenCalledWith(`/api/User`);
        });

        it('should throw an error if response is not ok', async () => {
            // arrange
            const errorMessage = 'No user is signed in';
            vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
                ok: false,
                text: async () => errorMessage
            }));

            // act + assert
            await expect(getCurrentUser()).rejects.toThrow(errorMessage);
        });
    });
});