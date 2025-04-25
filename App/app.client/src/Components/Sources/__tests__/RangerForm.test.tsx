import { cleanup, render, screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import RangerForm from "../Rangers/RangerForm";
import userEvent from "@testing-library/user-event";
import { Ranger } from "../../../Services/RangerService";

// mock callback
let mockOnSave: (ranger: Ranger) => void;
let mockOnCancel: () => void;

const initialRanger = {
    id: 1,
    firstName: "AA",
    lastName: "AAAA",
    email: "aaaaaa@aaa.com",
    districtId: 1
};

describe("RangerForm", () => {
    afterEach(() => {
        cleanup();
        vi.restoreAllMocks();
    });

    it("renders form with initial values", () => {
        render(
            <RangerForm
                initialRanger={initialRanger}
                onSave={mockOnSave}
                onCancel={mockOnCancel}
            />
        );

        // asserts
        expect(screen.getByLabelText(/Jméno:/i)).toHaveValue(initialRanger.firstName);
        expect(screen.getByLabelText(/Příjmení:/i)).toHaveValue(initialRanger.lastName);
        expect(screen.getByLabelText(/Email:/i)).toHaveValue(initialRanger.email);
    });

    it("updates form values based on user input", async () => {
        // act
        render(
            <RangerForm
                initialRanger={initialRanger}
                onSave={mockOnSave}
                onCancel={mockOnCancel}
            />
        );
        await userEvent.type(screen.getByLabelText(/Jméno:/i), "B");
        await userEvent.type(screen.getByLabelText(/Příjmení:/i), "B");
        await userEvent.type(screen.getByLabelText(/Email:/i), "B");

        // assert
        expect(screen.getByLabelText(/Jméno:/i)).toHaveValue("AAB");
        expect(screen.getByLabelText(/Příjmení:/i)).toHaveValue("AAAAB");
        expect(screen.getByLabelText(/Email:/i)).toHaveValue("aaaaaa@aaa.comB");
    });

    it("calls onSave with values when the form is submitted", async () => {

        mockOnSave = vi.fn();

        render(
            <RangerForm
                initialRanger={initialRanger}
                onSave={mockOnSave}
                onCancel={mockOnCancel}
            />
        );

        await userEvent.click(screen.getByRole("button", { name: /Uložit/i }));
        expect(mockOnSave).toHaveBeenCalledWith(initialRanger);
    });

    it("calls onCancel when cancel button is clicked", async () => {
        // to make it possible to track
        mockOnCancel = vi.fn();

        render(
            <RangerForm
                initialRanger={initialRanger}
                onSave={mockOnSave}
                onCancel={mockOnCancel}
            />
        );

        await userEvent.click(screen.getByRole("button", { name: /Zrušit/i }));

        expect(mockOnCancel).toHaveBeenCalled();
    });
});