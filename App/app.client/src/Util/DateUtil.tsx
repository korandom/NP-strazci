/**
 *  Abbr. of days of week in czech.
 */
export const nameOfDaysCZ: string[] = ["Ne", "Po", "Út", "St", "Čt", "Pá", "So"];

/**
 * Names of months in czech in chronological order.
 */
export const nameOfMonthsCZ: string[] = ["Leden", "Únor", "Březen", "Duben", "Květen", "Červen", "Červenec", "Srpen", "Září", "Říjen", "Listopad", "Prosinec"];

export const nameOfMonthsGenetiveCZ: string[] = ["ledna", "února", "března", "dubna", "května", "června", "července", "srpna", "září", "října", "listopadu", "prosince"];

/**
 * Function to format date to YYYY-MM-DD string.
 * 
 * @param date Date.
 * @returns The date in a string format.
 */
export const formatDate = (date: Date): string => {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0'); 
    const day = String(date.getDate()).padStart(2, '0');

    return `${year}-${month}-${day}`;
};

/**
 * Sets a date to the start of the day (00:00:00.000).
 */
const setHoursToMin = (date: Date): Date => {
    return new Date(date.setHours(0, 0, 0, 0));
};

/**
 * Sets a date to the end of the day (23:59:59.999).
 */
const setHoursToMax = (date: Date): Date => {
    return new Date(date.setHours(23, 59, 59, 999));
};

/**
 * Calculates a range of two weeks, containing the given date.
 * The given date is from the first of the two weeks.
 * The range starts with a Monday midnight and ends with a Sunday before midnight.
 * 
 * @param date Given Date
 * @returns an object with start and end Date.
 */
export const calculateTwoWeeksRange = (date: Date): { start: Date, end: Date } => { 
    const daysToMonday = date.getDay() === 0 ? 6 : date.getDay() - 1;
    
    const startDate = setHoursToMin(getShiftedDate(date, - daysToMonday));

    const endDate = setHoursToMax(getShiftedDate(startDate, 13));

    return { start: startDate, end: endDate };
};

/**
 * Returns a new date, that is shifted from the given date by a given number of days.
 * (Shift by one means that the returned date is the next day from the given date.)
 * @param date Original date
 * @param shift number of days to shift the original date
 * @returns A new shifted date
 */
export const getShiftedDate = (date: Date, shift: number): Date => {
    const newDate = new Date(date);
    newDate.setDate(date.getDate() + shift);
    return newDate;
}

/**
 * Generates an array of consecutive dates, from start to end, inclusive.
 * @param start first date in array
 * @param end last date in array
 * @returns An array of dates 
 */
export const generateDateRange = (start: Date, end: Date): Date[] => {
    const dateArray = [];
    for (let date = new Date(start); date <= end; date.setDate(date.getDate() + 1)) {
        dateArray.push(new Date(date));
    }
    return dateArray;
};