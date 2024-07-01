import React from 'react';

interface CalendarProps {
    onDateChange: (date: string) => void;
}

const Calendar: React.FC<CalendarProps> = ({ onDateChange }) => {
    const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        onDateChange(event.target.value);
    };

    return (
        <div className="calendar">
            <input type="date" onChange={handleChange} aria-label="date" />
        </div>
    );
};

export default Calendar;