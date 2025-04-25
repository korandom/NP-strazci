import { cleanup, render, screen } from "@testing-library/react";
import { userEvent } from "@testing-library/user-event";
import { afterEach, describe, expect, it, vi } from "vitest";
import Menu from "./Menu";
import { LinkProps } from "react-router-dom";


// mock router components
vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual<typeof import('react-router-dom')>('react-router-dom');
    return {
        ...actual,
        Link: ({ to, children }: LinkProps) => <a href={typeof to === 'string' ? to : '#'}>{children}</a>,
        Outlet: () => null,
    };
});


// replace fetchAllDistricts with a mock
vi.mock("../../Services/DistrictService", () => ({
    fetchAllDistricts: vi.fn(),
}));


// mock useAuth
const mockSignout = vi.fn();
let mockHasRole: (role: string) => boolean;
vi.mock("../../Hooks/useAuth", () => ({
    default: () => ({
        hasRole: (role: string) => mockHasRole(role),
        signout: mockSignout,
    }),
}));

// mock useDistrict
const mockAssignDistrict = vi.fn();
vi.mock("../../Hooks/useDistrict", () => ({
    default: () => ({
        assignDistrict: mockAssignDistrict,
    }),
}));


describe("MenuComponent", () => {
    afterEach(() => {
        cleanup();
        vi.restoreAllMocks();
    });

    const districts = [
        { id: 1, name: "Name" },
        { id: 2, name: "NoName" },
    ];

    it("renders toggle button on mobile screen", async () => { 
        mockHasRole = (role : string) => role == "Ranger";

        // act
        render(<Menu />);

        // assert 
        const toggleButton = screen.getByText("☰");
        expect(toggleButton).toBeInTheDocument();

        //  clicking the toggle button
        await userEvent.click(toggleButton);
        expect(screen.getByText("✕")).toBeInTheDocument();  

        // clicking reverts back
        await userEvent.click(screen.getByText("✕"));
        expect(screen.getByText("☰")).toBeInTheDocument();
    });
    

    it("shows SourceManagement Page Link if role HeadOfDistrict", async ()  => {
        // arrange
        mockHasRole = (role: string) => role == "HeadOfDistrict";
        //act
        render(<Menu />);
        const toggleButton = screen.getByText("☰");
        expect(toggleButton).toBeInTheDocument();

        // Click the toggle button
        await userEvent.click(toggleButton);

        // assert
        expect(screen.getByText("Správa zdrojů")).toBeInTheDocument();
    });
    
    it("shows District drop down menu if admin", async () => {
        // arrange
        mockHasRole = (role) => role === "Admin";
        vi.mocked(await import("../../Services/DistrictService")).fetchAllDistricts.mockResolvedValue(districts);

        // act
        render(<Menu />);
        const toggleButton = screen.getByText("☰");
        expect(toggleButton).toBeInTheDocument();
        await userEvent.click(toggleButton);

        // assert
        expect(screen.getByText("Obvody")).toBeInTheDocument();
        await userEvent.click(screen.getByText("Obvody"));
        expect(screen.getByText("Name")).toBeInTheDocument();
        expect(screen.getByText("NoName")).toBeInTheDocument();
        await userEvent.click(screen.getByText("Name"));
        expect(mockAssignDistrict).toHaveBeenCalledWith(1);
    });

    it("calls signout when logout is clicked", async () => {
        //arrange
        mockHasRole = () => false;

        //act
        render(<Menu />);
        const toggleButton = screen.getByText("☰");
        expect(toggleButton).toBeInTheDocument();
        await userEvent.click(toggleButton);
        await userEvent.click(screen.getByText("Odhlásit"));

        // assert
        expect(mockSignout).toHaveBeenCalled();
    });
});