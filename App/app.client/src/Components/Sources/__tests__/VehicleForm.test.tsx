import { cleanup, render, screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import VehicleForm from "../Vehicles/VehicleForm"; 
import userEvent from "@testing-library/user-event";
import { Vehicle } from "../../../Services/VehicleService";

// Mock functions
let mockOnSave: (vehicle: Vehicle) => void;
let mockOnCancel: () => void;

const initialVehicle = {
    id: 1,
    name: "",
    type: "",
    districtId: 1
};

describe("VehicleForm", () => {

    afterEach(() => {
        cleanup();
        vi.restoreAllMocks();
    });

    it("renders form with initial values", () => {
        render(
            <VehicleForm
                initialVehicle={initialVehicle}
                onSave={mockOnSave}
                onCancel={mockOnCancel}
            />
        );

        // Asserts that the fields are rendered with the initial values
        expect(screen.getByLabelText(/Identifikační jméno:/i)).toHaveValue(initialVehicle.name);
        expect(screen.getByLabelText(/Typ:/i)).toHaveValue(initialVehicle.type);
    });

    it("updates form values based on user input", async () => {
        render(
            <VehicleForm
                initialVehicle={initialVehicle}
                onSave={mockOnSave}
                onCancel={mockOnCancel}
            />
        );


        await userEvent.type(screen.getByLabelText(/Identifikační jméno:/i), "Name");
        await userEvent.type(screen.getByLabelText(/Typ:/i), "Type");

        expect(screen.getByLabelText(/Identifikační jméno:/i)).toHaveValue("Name");
        expect(screen.getByLabelText(/Typ:/i)).toHaveValue("Type");
    });

    it("calls onSave with updated values when the form is submitted", async () => {
        // Arrange
        mockOnSave = vi.fn();

        render(
            <VehicleForm
                initialVehicle={initialVehicle}
                onSave={mockOnSave}
                onCancel={mockOnCancel}
            />
        );


        await userEvent.type(screen.getByLabelText(/Identifikační jméno:/i), "Name");
        await userEvent.type(screen.getByLabelText(/Typ:/i), "Type");
        await userEvent.click(screen.getByRole("button", { name: /Uložit/i }));

        // Assert
        expect(mockOnSave).toHaveBeenCalledWith({
            id: 1,
            name: "Name", 
            type: "Type",
            districtId: 1
        });
    });

    it("calls onCancel when cancel button is clicked", async () => {
        // Arrange
        mockOnCancel = vi.fn();

        render(
            <VehicleForm
                initialVehicle={initialVehicle}
                onSave={mockOnSave}
                onCancel={mockOnCancel}
            />
        );

        // Act
        await userEvent.click(screen.getByRole("button", { name: /Zrušit/i }));

        // Assert
        expect(mockOnCancel).toHaveBeenCalled();
    });
});