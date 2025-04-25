import { render, screen, fireEvent } from '@testing-library/react';
import { afterEach, describe, expect, it, vi } from 'vitest';
import PlanRecord from '../PlanRecord/PlanRecord';
import { RangerSchedule } from '../../../Services/RangerScheduleService';
import { ReasonOfAbsence } from '../../../Services/AttendenceService';


// mock useSchedule
const mockAddPlannedRoute = vi.fn();
const mockRemovePlannedRoute = vi.fn();
const mockAddPlannedVehicle = vi.fn();
const mockRemovePlannedVehicle = vi.fn();
const mockUpdateWorking = vi.fn();
const mockUpdateReasonOfAbsence = vi.fn();

vi.mock('../../../Hooks/useSchedule', () => ({
    default: () => ({
        addPlannedRoute: mockAddPlannedRoute,
        addPlannedVehicle: mockAddPlannedVehicle,
        removePlannedRoute: mockRemovePlannedRoute,
        removePlannedVehicle: mockRemovePlannedVehicle,
        updateWorking: mockUpdateWorking,
        updateReasonOfAbsence: mockUpdateReasonOfAbsence,
    }),
}));

// mock useAuth
let mockHasRole: (role: string) => boolean;
vi.mock('../../../Hooks/useAuth', () => ({
    default: () => ({
        hasRole: (role: string) => mockHasRole(role),
    }),
}));

// mock useDistrict
vi.mock('../../../Hooks/useDistrict', () => ({
    default: () => ({
        routes: [{ id: 1, name: 'route', priority: 2 }],
        vehicles: [{ id: 10, name: 'aa', type: 'vehicle' }],
    }),
}));

describe('PlanRecord Component', () => {
    afterEach(() => {
        vi.clearAllMocks();
    });
    const baseSchedule : RangerSchedule = {
        ranger: { id: 1, firstName: 'Name', lastName: 'Surname', email: 'abc@email.com', districtId: 1 },
        date: '2025-04-01',
        routeIds: [1],
        vehicleIds: [],
        from: null,
        working: true,
        reasonOfAbsence: ReasonOfAbsence.None,
    };
    mockHasRole = () => false;

    it('should include ranger name if includeRangerName', () => {
        render(<PlanRecord schedule={baseSchedule} includeRangerName={true} isEditable={false} />);
        expect(screen.getByText('Name Surname')).toBeInTheDocument();
    });

    it('shows yes button if working', () => {
        render(<PlanRecord schedule={baseSchedule} includeRangerName={false} isEditable={true} />);
        expect(screen.getByRole('button')).toHaveTextContent('✔');
    });

    it('disables working toggle button if not editable', () => {
        const { container } = render(<PlanRecord schedule={baseSchedule} includeRangerName={false} isEditable={false} />);
        expect(container.querySelector('.positive')!).toBeDisabled();
    });

    it('calls updateWorking when switching working', () => {
        render(<PlanRecord schedule={baseSchedule} includeRangerName={false} isEditable={true} />);
        const button = screen.getByRole("button", { name: /✔/ });
        fireEvent.click(button);

        //assert
        expect(screen.getByRole("button", { name: /×/ })).toBeInTheDocument();
        expect(mockUpdateWorking).toHaveBeenCalledWith('2025-04-01', baseSchedule.ranger, false);
    });

    it('shows absence select when not working and editable', () => {
        //arrange
        const notWorking = { ...baseSchedule, working: false, reasonOfAbsence: ReasonOfAbsence.N };

        //act
        const { container } = render(<PlanRecord schedule={notWorking} includeRangerName={false} isEditable={true} />);
        const select = container.querySelector('.dropdown') as Element;
        expect(select).toBeInTheDocument();
    });

    it('does not show edit button if not editable', () => {
        render(<PlanRecord schedule={baseSchedule} includeRangerName={false} isEditable={false} />);
        expect(screen.queryByText('✎')).not.toBeInTheDocument();
    });

    it('trying to add new route from dropdown when its already scheduled', () => {
        
        const { container } = render(<PlanRecord schedule={baseSchedule} includeRangerName={false} isEditable={true} />);
        fireEvent.click(screen.getByText('✎'));
        const dropdown = container.querySelector('select.routes');
        fireEvent.change(dropdown!, { target: { value: '1' } });
        expect(mockAddPlannedRoute).not.toHaveBeenCalled(); 
    });

    it('adds a new vehicle from dropdown', () => {
        // must be HeadOfDistrict
        mockHasRole = () => true;
        const schedule = { ...baseSchedule };
        const { container } = render(<PlanRecord schedule={schedule} includeRangerName={false} isEditable={true} />);
        // start edit
        fireEvent.click(screen.getByText('✎'));
        const dropdowns = container.querySelectorAll('select');
        // add vehicle
        fireEvent.change(dropdowns[0], { target: { value: '10' } });

        // assert called add vehicle to plan
        expect(mockAddPlannedVehicle).toHaveBeenCalledWith('2025-04-01', schedule.ranger.id, 10);
    });
});