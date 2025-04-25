import { cleanup, render, screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { District } from "../../Services/DistrictService";
import SourceManagement from "../SourceManagement";

let mockUseDistrict: () => { district: District| undefined, error: Error | null };
// mock useSchedule
vi.mock('../../Hooks/useDistrict', () => ({
    default: () => (
        mockUseDistrict()
    ),
}));

// mock RoutesManager component
vi.mock('../../Components/Sources/Routes/RoutesManager', () => {
    return {
        default: ({ districtId }: { districtId: string }) =>
            <div data-testid="routes-manager">RoutesManager for {districtId}</div>
    }
});

// mock RangerManager component
vi.mock('../../Components/Sources/Rangers/RangerManager', () => {
    return {
        default: ({ districtId }: { districtId: string }) =>
            <div data-testid="rangers-manager">RangersManager for {districtId}</div>
    }
});

// mock VehiclesManager component
vi.mock('../../Components/Sources/Vehicles/VehicleManager', () => {
    return {
        default: ({ districtId }: { districtId: string }) =>
            <div data-testid="vehicles-manager">VehiclesManager for {districtId}</div>
    }
});


describe("SourceManagementPage", () => {
    afterEach(() => {
        cleanup();
        vi.restoreAllMocks();
    })

    it("displays error message if error fetching district data", () => {
        // arrange
        mockUseDistrict = () => ({
            district: undefined,
            error: new Error("Something went wrong"),
        });

        //act
        render(<SourceManagement/>);

        // assert
        expect(screen.getByText("Something went wrong")).toBeInTheDocument();
    })

    it("displays warning if district is undefined", () => {
        // arrange
        mockUseDistrict = () => ({
            district: undefined,
            error: null,
        });

        // act
        render(<SourceManagement />);

        // assert
        expect(screen.getByText("Není vybrán žádný obvod, vyberte obvod z menu.")).toBeInTheDocument();

    })

    it("displays managers for correct district", () => {

        // arrange
        mockUseDistrict = () => ({
            district: {
                id: 2,
                name: "Name",
            },
            error: null,
        });

        // act
        render(<SourceManagement />);

        // assert
        expect(screen.getByText("Obvod Name")).toBeInTheDocument();
        expect(screen.getByTestId("routes-manager")).toHaveTextContent("2");
        expect(screen.getByTestId("vehicles-manager")).toHaveTextContent("2");
        expect(screen.getByTestId("rangers-manager")).toHaveTextContent("2");
    })

});