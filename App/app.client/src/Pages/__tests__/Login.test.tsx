import { cleanup, render, screen } from "@testing-library/react";

import { afterEach, describe, expect, it, vi } from "vitest";
import Login from "../Login";
import userEvent from "@testing-library/user-event";


// mock useAuth
let mockSignIn: (email: string, password: string) => void;
let mockLoading: boolean;
let mockError: Error | null;

vi.mock("../../Hooks/useAuth", () => ({
    default: () => ({
        signin: mockSignIn,
        loading: mockLoading,
        error: mockError,
    }),
}));


describe("LoginPage", () => {
    afterEach(() => {
        cleanup();
        vi.restoreAllMocks();
    })

    it("renders login form", () => {
        // arrange
        mockSignIn = vi.fn();
        mockLoading = false;
        mockError = null;

        // act
        render(<Login />);

        // assert
        expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/heslo/i)).toBeInTheDocument();
        expect(screen.getByRole("button", { name: /přihlásit/i })).toBeInTheDocument();
    })

    it("updates based on inputs", async() => {
        // arrange
        mockSignIn = vi.fn();
        mockLoading = false;
        mockError = null;
        render(<Login />);

        // act
        await userEvent.type(screen.getByLabelText(/email/i), "test@example.com");
        await userEvent.type(screen.getByLabelText(/heslo/i), "password123");

        // assert
        expect(screen.getByLabelText(/email/i)).toHaveValue("test@example.com");
        expect(screen.getByLabelText(/heslo/i)).toHaveValue("password123");
    })

    it("calls signIn upon submit", () => {

    })

    it("displays dots when loading", () => {

    })

    it("displays error message when error", () => {

    })
});