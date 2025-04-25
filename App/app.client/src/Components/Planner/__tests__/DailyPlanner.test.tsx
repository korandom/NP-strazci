import { fireEvent, render, screen, } from '@testing-library/react';
import { describe, it, vi, expect } from 'vitest';
import DailyPlanner from '../DailyPlanner/DailyPlanner';

// mock useSchedule
const mockWeekForward = vi.fn();
const mockWeekBack = vi.fn();
const mockDateRange = {
    start: new Date("2025-01-06"), 
    end: new Date("2025-01-20")
};

vi.mock('../../../Hooks/useSchedule', () => ({
    default: () => ({
        dateRange: mockDateRange,
        weekForward: mockWeekForward,
        weekBack: mockWeekBack,
        loading: false
    })
}));

// mock PlansForDay component
vi.mock('../PlansForDay/PlanForDay', () => ({
    default: ({ date, onlyWorking }: {date:Date, onlyWorking: boolean}) => (
        <div data-testid="plans-for-day">
            <p>Plans For {date.toString()}</p>
            <p>{onlyWorking ? 'Only Working' : 'All'}</p>
        </div>
    )
}));

describe('DailyPlanner', () => {

    it('renders the DailyPlanner component', () => {
        //act
        render(<DailyPlanner />);
        //assert
        expect(screen.getByTestId('plans-for-day')).toBeInTheDocument();
    });

    it('renders date picker and header', () => {

        //act
        render(<DailyPlanner />);

        //assert
        expect(screen.getByText(/Zobrazit všechny/i)).toBeInTheDocument();
        expect(screen.getByText("◀")).toBeInTheDocument();
        expect(screen.getByText("▶")).toBeInTheDocument();
        expect(screen.getByTestId("plans-for-day")).toBeInTheDocument();
    });
    it('calls weekBack on ◀ click', async () => {
        // act
        render(<DailyPlanner />);
        const backButton = screen.getByText("◀");
        fireEvent.click(backButton);

        // assert
        expect(mockWeekBack).toHaveBeenCalled();
    });

    it('calls weekForward on ▶ click', async () => {
        // act
        render(<DailyPlanner />);
        const forwardButton = screen.getByText("▶");
        fireEvent.click(forwardButton);

        // assert
        expect(mockWeekForward).toHaveBeenCalled();
    });

    it('displays only working', () => {
        //act
        render(<DailyPlanner />);
        //assert
        expect(screen.getByTestId("plans-for-day")).toHaveTextContent("Only Working");

    });

    it('displays all when checkbox is clicked', () => {
        //act
        render(<DailyPlanner />);
        const checkbox = screen.getByRole('checkbox')
        fireEvent.click(checkbox);
        //assert
        expect(screen.getByTestId("plans-for-day")).toHaveTextContent("All");

    });
})