import { vi , describe, it, expect, afterEach} from 'vitest';
import { updateAttendence, Attendence, ReasonOfAbsence } from '../AttendenceService';

describe('updateAttendence', () => {
    afterEach(() => {
        vi.restoreAllMocks()
    })

    const mockAttendence: Attendence = {
        date: '2024-04-01',
        ranger: { id: 1, firstName: "name", lastName: "surname", districtId:1, email:"test@mail.com"}, 
        working: true,
        from: '8:00',
        reasonOfAbsence: ReasonOfAbsence.None
    };

    it('should update attendence and return the updated attendence', async () => {

       // arrange
        const mockResponse = {
            date: '2024-04-01',
            ranger: { id: 1, firstName: "name", lastName: "surname", districtId: 1, email: "test@mail.com" },
            working: true,
            from: '8:00',
            reasonOfAbsence: 0
        };

        // mocking fetch
        vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
            ok: true,
            json: async () => mockAttendence,
        }));

        //act
        const result = await updateAttendence(mockAttendence);

        // assert
        expect(result).toEqual(mockResponse); 
        expect(fetch).toHaveBeenCalledWith(
            '/api/Attendence/update',
            expect.objectContaining({
                method: 'PUT',
                headers: expect.objectContaining({
                    'Content-Type': 'application/json'
                }),
                body: JSON.stringify({
                    ...mockAttendence,
                    reasonOfAbsence: 0 // enum converted to number
                })
            })
        );
    });

    it('should throw an error if the response is not ok', async () => {
        // arrange
        const mockErrorMessage = 'Error updating attendence';
        vi.stubGlobal('fetch', vi.fn().mockResolvedValueOnce({
            ok: false,
            text: async () => mockErrorMessage,
        }));

        // expecting throwing an error
        await expect(updateAttendence(mockAttendence)).rejects.toThrow(mockErrorMessage);

        // checking correct call
        expect(fetch).toHaveBeenCalledWith(
            '/api/Attendence/update',
            expect.objectContaining({
                method: 'PUT',
                headers: expect.objectContaining({
                    'Content-Type': 'application/json'
                }),
                body: JSON.stringify({
                    ...mockAttendence,
                    reasonOfAbsence: 0 // enum converted to number
                })
            })
        );
    });
});