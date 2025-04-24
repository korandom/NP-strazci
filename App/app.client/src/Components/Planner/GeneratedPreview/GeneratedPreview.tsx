import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Plan, fetchGeneratedRoutePlan,  updatePlans } from "../../../Services/PlanService";
import { formatDate, generateDateRange, getShiftedDate, nameOfDaysCZ } from "../../../Util/DateUtil";
import useDistrict from '../../../Hooks/useDistrict';
import RangerCell from "../RangerCell";
import useSchedule from '../../../Hooks/useSchedule';


/**
 * React component for generating a route plan and displaying a preview before saving changes.
 * The generating itself is done on the server.
 */
const GeneratedPreview: React.FC = (): JSX.Element => {
    const { date } = useParams();
    const { rangers, routes, district } = useDistrict();
    const { triggerReload } = useSchedule();
    const parsedDate = new Date(date!);
    const navigate = useNavigate();
    const [generating, setGenerating] = useState<boolean>(true);
    const [generatedPlan, setGeneratedPlan] = useState<Plan[]>([]);
    const [message, setMessage] = useState<string>("");
    const [success, setSuccess] = useState<boolean>(false);

    useEffect(() => {
        const fetchRoutePlan = async () => {
            setGenerating(true);
            if (district !== undefined && date !== null) {
                try {
                    const fetchedResult = await fetchGeneratedRoutePlan(district?.id, date!);
                    setMessage(fetchedResult.message);
                    setSuccess(fetchedResult.success);
                    setGeneratedPlan(fetchedResult.plans);
                } catch (error) {
                    console.error("Error fetching generated route plan", error);
                    setSuccess(false);
                    setMessage("Neznámý error při generování plánu tras.")
                }
            }
            setGenerating(false);
        };

        fetchRoutePlan(); 
    }, [district, date]);

    const save = async () => {
        await updatePlans(generatedPlan);
        triggerReload();
        navigate("/planovani");
    }

    const dateArray = generateDateRange(parsedDate, getShiftedDate(parsedDate, 6));

    return (
        <div className="generating-container">
            {generating ? (
                <div className="generating-loading">
                    <div>Probíhá Generování ...</div>
                    <button onClick={() => navigate("/")}>Zrušit</button>
                </div>

            ) : (<>
                    {!success &&
                        <div className="message">
                            <div>{message}</div>
                            <button onClick={() => navigate("/planovani")}>Zpět</button>
                        </div>
                    }

                    {success && 
                        <>
                        <div className="message">
                            <div>{message}</div>
                        </div>
                        <div className="generate-buttons">
                            <button onClick={() => navigate("/planovani")}>Zahodit</button>
                            <button onClick={save}>Uložit</button>
                        </div>
                    <div className="table-container">
                        {dateArray.length > 0 && (
                            <table className="plan-table">
                                <thead>
                                    <tr>
                                        <th></th>
                                        {dateArray.map((date, index) => {
                                            const Weekend = date.getDay() == 0 || date.getDay() == 6;
                                            return (
                                                <th className={Weekend ? "weekend date-header sticky" : "date-header sticky"} key={index}>
                                                    <div className="dayOfWeek">
                                                        {nameOfDaysCZ[date.getDay()]}
                                                    </div>
                                                    <div className="date">
                                                        {date.getDate()}.{(date.getMonth() + 1)}.
                                                    </div>
                                                </th>
                                            );
                                        })}
                                    </tr>
                                </thead>
                                <tbody>
                                    {rangers?.map((ranger) => (
                                        <tr key={ranger.id}>
                                            <td className="sticky">
                                                <RangerCell ranger={ranger} />
                                            </td>

                                            {dateArray.map((date, dateIndex) => {
                                                const isWeekend = date.getDay() === 0 || date.getDay() === 6;
                                                const stringDate = formatDate(date);

                                                const routesIds = generatedPlan.find((p) => p.ranger.id === ranger.id && p.date === stringDate)?.routeIds;

                                                return (
                                                    <td key={dateIndex} className={isWeekend ? "weekend" : ""}>
                                                        <div className="planned-items-container">
                                                        {routesIds && routesIds.length > 0 ? (
                                                            routesIds.map((routeId, routeIndex) => {
                                                                const route = routes.find((r) => r.id === routeId);
                                                                if (!route) return null;

                                                                return (
                                                                    <div
                                                                        key={routeIndex}
                                                                        className={`route priority-${route.priority}`}
                                                                    >
                                                                        <div className="identification">
                                                                            <p>{route.name}</p>
                                                                        </div>
                                                                    </div>
                                                                );
                                                            })
                                                        ) : (
                                                            <div className="no-routes">×</div>
                                                        )}
                                                        </div>
                                                    </td>
                                                );
                                            })}
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        )}
                        </div>
                    </>}
                </>
            )}
        </div>
    )
}

export default GeneratedPreview;