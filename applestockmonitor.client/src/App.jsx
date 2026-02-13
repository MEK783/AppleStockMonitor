import { useEffect, useState } from 'react';
import './App.css';

function App() {
    const [intervals, setIntervals] = useState([]);
    useEffect(() => {
        setIntervals(getIntervals());
    }, []);

    return (
        <div>
            <h1 id="headerLabel">Apple Stock distribution calculator</h1>
            <p>Fill in the date range and interval for the distribution of returns</p>
            <form className="form-inline" id="stockData">
                <label htmlFor="startDate">Start Date:</label>
                <input type="date" id="startDate" name="startDate" className="form-control mx-2" />
                <label htmlFor="endDate">End Date:</label>
                <input type="date" id="endDate" name="endDate" className="form-control mx-2" />
                <label htmlFor="interval">Interval:</label>
                <select id="interval" name="interval" className="form-control mx-2">{intervals.map(
                    interval => <option key={interval} value={interval}>{interval}</option>
                )}</select>
            </form>
        </div>
    );

    async function getIntervals() {
        const response = await fetch('stockdata');
        if (response.ok) {
            return JSON.parse(response.json());
        }

        return [];
    }
}

export default App;