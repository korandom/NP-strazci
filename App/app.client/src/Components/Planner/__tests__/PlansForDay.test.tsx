import { cleanup, render, screen } from '@testing-library/react';
import PlansForDay from '../PlansForDay/PlanForDay';
import { vi } from 'vitest';

import { ReasonOfAbsence } from '../../../Services/AttendenceService';
import { Locked } from '../../../Services/LockService';


// data
const mockRanger = {
    id: 1,
    firstName: 'Name',
    lastName: 'Surname',
    email: 'test@email.com',
};

const schedule = {
    date: '2025-04-01',
    ranger: mockRanger,
    routeIds: [],
    vehicleIds: [],
    working: true,
    from: null,
    reasonOfAbsence: ReasonOfAbsence.None,
};

// mock schedules
vi.mock('../../../Hooks/useSchedule', () => ({
    default: () => ({
        schedules: [schedule]
    })
}));

let locks : Locked[] = [];
vi.mock('../../../Hooks/useDistrict', () => ({
    default: () => ({
        rangers: [mockRanger],
        locks: locks,
    })
}));

// mock auth
const mockHasRole = vi.fn().mockReturnValue(false);
vi.mock('../../../Hooks/useAuth', () => ({
    default: () => ({
        user: { rangerId: 1 },
        hasRole: mockHasRole
    })
}));



describe('<PlansForDay />', () => {
    afterEach(() => {
        cleanup();
        vi.restoreAllMocks();
    });


    it('displays working if only working', () => {
        schedule.working = true;
        render(<PlansForDay date={new Date('2025-04-01')} onlyWorking={true} />);
        expect(screen.getByText(/Name Surname/)).toBeInTheDocument();
    });

    it('does not display non working when only working', () => {
        schedule.working = false;
        render(<PlansForDay date={new Date('2025-04-01')} onlyWorking={true} />);
        expect(screen.queryByText(/Name Surname/)).not.toBeInTheDocument();
    });

    it('displays non-working schedule if onlyWorking=false', () => {
        schedule.working = false;
        render(<PlansForDay date={new Date('2025-04-01')} onlyWorking={false} />);
        expect(screen.getByText(/Name Surname/)).toBeInTheDocument();
    });

    it('displays editable if user is HeadOfDistrict', () => {
        schedule.working = true;
        mockHasRole.mockReturnValue(true);

        render(<PlansForDay date={new Date('2025-04-01')} onlyWorking={true} />);
        const button = screen.getByRole('button');
        expect(button).not.toBeDisabled();
    });

    it('disables editing if user is locked', () => {
        schedule.working = true;
        mockHasRole.mockReturnValue(false);
        locks = [{ date: '2025-04-01', districtId: 1 }];

        render(<PlansForDay date={new Date('2025-04-01')} onlyWorking={true} />);
        const button = screen.getByRole('button');
        expect(button).toBeDisabled();
    });

    it('disables editing if in the past', () => {
        schedule.working = true;
        mockHasRole.mockReturnValue(false);

        render(<PlansForDay date={new Date('2025-04-01')} onlyWorking={true} />);
        const button = screen.getByRole('button');
        expect(button).toBeDisabled();
    });
});