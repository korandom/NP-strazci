import { cleanup, render, screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import RouteForm from "../Routes/RouteForm";  
import userEvent from "@testing-library/user-event";
import { Route } from "../../../Services/RouteService";

// mock callback
let mockOnSave: (route: Route) => void;
let mockOnCancel: () => void;

const initialRoute = {
    id: 1,
    name: "Route",
    priority: 0,
    districtId: 1,
    controlPlace: {
        controlPlaceDescription: "KK",
        controlTime: "10:00",
    }
};

describe("RouteForm", () => {
    afterEach(() => {
        cleanup();
        vi.restoreAllMocks();
    });

    it("renders form with initial values", () => {
        render(
            <RouteForm
                initialRoute={initialRoute}
                onSave={mockOnSave}
                onCancel={mockOnCancel}
            />
        );

        // asserts
        expect(screen.getByLabelText(/Jméno:/i)).toHaveValue(initialRoute.name);
        expect(screen.getByLabelText(/Priorita:/i)).toHaveValue(initialRoute.priority.toString());
        expect(screen.getByLabelText(/Místo kontroly:/i)).toHaveValue(initialRoute.controlPlace.controlPlaceDescription);
        expect(screen.getByLabelText(/Čas kontroly:/i)).toHaveValue(initialRoute.controlPlace.controlTime);
    });

    it("updates form values based on user input", async () => {
        // act
        render(
            <RouteForm
                initialRoute={initialRoute}
                onSave={mockOnSave}
                onCancel={mockOnCancel}
            />
        );

        await userEvent.type(screen.getByLabelText(/Jméno:/i), "1");
        await userEvent.selectOptions(screen.getByLabelText(/Priorita:/i), ["1"]); 
        await userEvent.type(screen.getByLabelText(/Místo kontroly:/i), "K");
        await userEvent.clear(screen.getByLabelText(/Čas kontroly:/i));
        await userEvent.type(screen.getByLabelText(/Čas kontroly:/i), "12:00");

        // assert
        expect(screen.getByLabelText(/Jméno:/i)).toHaveValue("Route1");
        expect(screen.getByLabelText(/Priorita:/i)).toHaveValue("1");
        expect(screen.getByLabelText(/Místo kontroly:/i)).toHaveValue("KKK");
        expect(screen.getByLabelText(/Čas kontroly:/i)).toHaveValue("12:00");
    });

    it("calls onSave with updated values when the form is submitted", async () => {
        mockOnSave = vi.fn();

        render(
            <RouteForm
                initialRoute={initialRoute}
                onSave={mockOnSave}
                onCancel={mockOnCancel}
            />
        );

        await userEvent.type(screen.getByLabelText(/Jméno:/i), "1");
        await userEvent.selectOptions(screen.getByLabelText(/Priorita:/i), ["2"]); // Týdenní
        await userEvent.type(screen.getByLabelText(/Místo kontroly:/i), "K");
        await userEvent.clear(screen.getByLabelText(/Čas kontroly:/i));
        await userEvent.type(screen.getByLabelText(/Čas kontroly:/i), "14:00");

        await userEvent.click(screen.getByRole("button", { name: /Uložit/i }));

        expect(mockOnSave).toHaveBeenCalledWith({
            id: 1,
            name: "Route1",
            districtId: 1,
            priority: "2", // Týdenní
            controlPlace: {
                controlPlaceDescription: "KKK",
                controlTime: "14:00",
            },
        });
    });

    it("calls onCancel when the cancel button is clicked", async () => {
        mockOnCancel = vi.fn();

        render(
            <RouteForm
                initialRoute={initialRoute}
                onSave={mockOnSave}
                onCancel={mockOnCancel}
            />
        );

        await userEvent.click(screen.getByRole("button", { name: "Zrušit" }));
        expect(mockOnCancel).toHaveBeenCalled();
    });

    it("adds and removes control points", async () => {
        render(
            <RouteForm
                initialRoute={initialRoute}
                onSave={mockOnSave}
                onCancel={mockOnCancel}
            />
        );

        // Initially, the "Odebrat kontrolu" button is visible
        expect(screen.getByRole("button", { name: /Odebrat kontrolu/i })).toBeInTheDocument();
        await userEvent.click(screen.getByRole("button", { name: "Odebrat kontrolu" }));

        // After adding control, the "Odebrat kontrolu" button should be visible
        expect(screen.getByRole("button", { name: /Přidat kontrolu/i })).toBeInTheDocument();
    });
});