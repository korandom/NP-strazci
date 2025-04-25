import { render, screen, fireEvent, cleanup } from '@testing-library/react';
import PlanTable from '../PlanTable/PlanTable';
import { vi } from 'vitest';
import { ReasonOfAbsence } from '../../../Services/AttendenceService';
import { formatDate } from '../../../Util/DateUtil';
import { Locked } from '../../../Services/LockService';

// data
const mockRanger = {
    id: 1,
    firstName: 'Name',
    lastName: 'Surname',
    email: 'abc@email.com',
};

// create a range of mockSchedules
const today = new Date();
const endDate = new Date();
endDate.setDate(today.getDate() + 13);

const schedules = Array.from({ length: 14 }, (_, i) => {
    const date = new Date(today);
    date.setDate(today.getDate() + i);

    return {
        date: formatDate(date),
        ranger: mockRanger,
        routeIds: [],
        vehicleIds: [],
        working: true,
        from: null,
        reasonOfAbsence: ReasonOfAbsence.None,
    };
});

// mock auth
const mockHasRole = vi.fn().mockReturnValue(false);
vi.mock('../../../Hooks/useAuth', () => ({
    default: () => ({
        user: { rangerId: 1 },
        hasRole: mockHasRole
    })
}));

let locks: Locked[] = [];
vi.mock('../../../Hooks/useDistrict', () => ({
    default: () => ({
        rangers: [mockRanger],
        locks: locks,
        addLock: vi.fn(),
        removeLock: vi.fn(),
    })
}));

const mockBack = vi.fn();
const mockForward = vi.fn();

let mockSchedules = schedules;
// mock schedules
vi.mock('../../../Hooks/useSchedule', () => ({
    default: () => ({
        schedules: mockSchedules,
        dateRange: { start: today, end: endDate },
        weekBack: mockBack,
        weekForward: mockForward
    })
}));



describe('<PlanTable />', () => {

    afterEach(() => {
        cleanup();
        vi.restoreAllMocks();
        locks = [];
        mockSchedules = schedules;
    });

    it('Displays headers, buttons', () => {
        const { container } = render(<PlanTable />);
        expect(container.querySelector('.month-label')).toBeInTheDocument(); 
        expect(screen.getByText('Name Surname')).toBeInTheDocument(); 
    });

    it('locks are not displayed to normal rangers', () => {
        render(<PlanTable />);
        expect(screen.queryByText('🔒')).not.toBeInTheDocument();
        expect(screen.queryByText('🔓')).not.toBeInTheDocument();
    });

    it('shows lock/unlock buttons for HeadOfDistrict on future dates', () => {
        mockHasRole.mockImplementation((role) => role === 'HeadOfDistrict')
        render(<PlanTable />);
        expect(screen.getAllByText('🔓').length).toBeGreaterThan(0);
    });

    it('disables editing for past day if not HeadOfDistrict', () => {
        locks = [{ date: '2025-04-01', districtId: 1 }];
        mockSchedules = [{
            date: '2025-04-01',
            ranger: mockRanger,
            routeIds: [],
            vehicleIds: [],
            working: true,
            from: null,
            reasonOfAbsence: ReasonOfAbsence.None,
        }]
        render(<PlanTable />);
        expect(screen.queryByText('✎')).not.toBeInTheDocument();
    });

    it('enables editing for own unlocked day', () => {
        render(<PlanTable />);
        expect(screen.getAllByText('✎').length).toBeGreaterThan(0);
    });

    it('calls weekBack and weekForward when clicked', () => {

        render(<PlanTable />);
        fireEvent.click(screen.getByText('◀'));
        fireEvent.click(screen.getByText('▶'));

        expect(mockBack).toHaveBeenCalled();
        expect(mockForward).toHaveBeenCalled();
    });
});
