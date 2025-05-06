import { cleanup, render, screen } from "@testing-library/react";
import { userEvent } from "@testing-library/user-event";
import { afterEach, describe, expect, it, vi } from "vitest";
import Planner from "../Planner";

// mock has role dynamically, so that it can be changed inside tests
let mockHasRole: (role: string) => boolean;
vi.mock('../../Hooks/useAuth', () => ({
    default: () => ({
        hasRole: (role: string) => mockHasRole(role),
    }),
}));

// mock query is (max-width: 560px) --> false == pc, true == mobile
let mockUseMediaQuery: (query: string) => boolean;
vi.mock('../../Hooks/useMediaQuery', () => ({
    useMediaQuery: (query: string) => mockUseMediaQuery(query),
}));

// mock navigate
const mockNavigate = vi.fn()
vi.mock('react-router-dom', async () => {
    // import rest of the model
    const actual = await vi.importActual<typeof import('react-router-dom')>('react-router-dom');

    return {
        ...actual,
        useNavigate: () => mockNavigate
    };
});

// mock useSchedule
vi.mock('../../Hooks/useSchedule', () => ({
    default: () => ({
        error: null,
        dateRange: {
            start: new Date('2025-01-06'),
            end: new Date('2025-01-20'),
        },
    }),
}));

// mock Daily planner component
vi.mock('../../Components/Planner/DailyPlanner/DailyPlanner', () => {
    return {
        default: () => <div data-testid="daily-planner">DailyPlanner</div>
    }
});

// mock PlanTable component
vi.mock('../../Components/Planner/PlanTable/PlanTable', () => {
    return {
        default: () => <div data-testid="plan-table">PlanTable</div>
    }
});


describe("PlannerPage", () => {
    afterEach(() => {
        cleanup();
        vi.restoreAllMocks();
    })



    it("should show DailyPlanner on mobile screen", () => {
        //arrange
        mockUseMediaQuery = () => true;
        mockHasRole = (role: string) => role == "HeadOfDistrict";

        //act
        render(<Planner />);

        // assert
        expect(screen.getByTestId('daily-planner')).toBeInTheDocument();
        expect(screen.queryByTestId('plan-table')).not.toBeInTheDocument();


    });

    it("should NOT show a \"showFortNight\" button on mobile screen", () => {
        //arrange
        mockUseMediaQuery = () => true;
        mockHasRole = (role: string) => role == "Ranger";

        //act
        const { container } = render(<Planner />);

        // assert
        const button = container.querySelector('.showFortNight');
        expect(button).toBeFalsy();
    });

    it("should show PlanTable on larger screens", () => {
        //arrange
        mockUseMediaQuery = () => false;
        mockHasRole = (role: string) => role == "HeadOfDistrict";

        //act
        render(<Planner />);

        // assert
        expect(screen.getByTestId('plan-table')).toBeInTheDocument();
        expect(screen.queryByTestId('daily-planner')).not.toBeInTheDocument();

    });

    it("should show toggle button on larger screens", () => {
        //arrange
        mockUseMediaQuery = () => false;
        mockHasRole = (role: string) => role == "Ranger";

        //act
        const { container } = render(<Planner />);

        // assert
        const button = container.querySelector('.showFortNight');
        expect(button).toBeInTheDocument();
    });

    it("should show DailyPlanner, toggle button back and NO generate button, when user clicks toggle button", async () => {
        //arrange that checks stuff too, but just from the toglle button stuff
        mockUseMediaQuery = () => false;
        mockHasRole = (role: string) => role == "Ranger";
        const { container } = render(<Planner />);
        const button = container.querySelector(".showFortNight")!; 
        expect(button).toBeInTheDocument();

        // act
        await userEvent.click(button);

        // assert
        expect(screen.getByTestId('daily-planner')).toBeInTheDocument();
        const Backbutton = container.querySelector('.showFortNight');
        expect(Backbutton).toBeInTheDocument();
        const generateButton = container.querySelector('.generate');
        expect(generateButton).not.toBeInTheDocument();
        
    });

    it("should show generate button for role HeadOfDistrict", async () => {
        //arrange
        mockUseMediaQuery = () => false;
        mockHasRole = (role: string) => role === "HeadOfDistrict";

        //act + assert button exists
        const { container } = render(<Planner />);
        const button = container.querySelector('.generate');
        expect(button).toBeInTheDocument();

        //act +assert click button
        const generateClick = button as Element;
        await userEvent.click(generateClick);
        expect(mockNavigate).toHaveBeenCalledWith('/generovani/2025-01-13');
    });

    it("should NOT show generate button for role Ranger", () => {
        mockUseMediaQuery = () => false;
        mockHasRole = (role: string) => role === "Ranger";

        const { container } = render(<Planner />);
        const button = container.querySelector('.generate');
        expect(button).not.toBeInTheDocument();
    });
});