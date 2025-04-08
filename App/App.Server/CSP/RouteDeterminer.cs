using App.Server.DTOs;

namespace App.Server.CSP
{
    public interface IRouteTypeDeterminer
    {
        public RouteType DetermineRouteType(RouteDto route);
        public bool IsPreplanned(int routeId, int dayNumber);
    }

    /// <summary>
    /// Class for determining type of routes based on how often it should be planned/was planned.
    /// </summary>
    /// <param name="start">Start of the planning period.</param>
    /// <param name="plans">Plans that are taken into consideration.</param>
    public class RouteDeterminer(DateOnly start, List<PlanDto> plans) : IRouteTypeDeterminer
    {
        private readonly DateOnly _start = start;
        private readonly List<PlanDto> _plans = plans;

        /// <summary>
        /// Determines what type of route the given route is in the context of recent plans.
        /// This implementation is based on a dayCount of 7 - one week! 
        /// If the planning period would change, this method does not work as intended and must be refactored.
        /// Routes with a Daily priority have a RouteType Daily.
        /// Routes with a Weekly priority have a RouteType Once.
        /// Routes with a Forthnighly priority have a RouteType of Once if not planned the previous week, else MaxOnce.
        /// Routes with a Monthly priority have a RouteType of Once if not planned the previous 3 weeks,
        /// if they were planned the previous week, they should not be planned at all, else Meh.
        /// </summary>
        /// <param name="route">Given Route</param>
        /// <returns>RouteType of the route</returns>
        public RouteType DetermineRouteType(RouteDto route)
        {
            switch (route.Priority)
            {
                //Monthly
                case 0:
                    if (WasRoutePlanned(route.Id, _start, _start.AddDays(7)))
                    {
                        return RouteType.None;
                    }
                    if (WasRoutePlanned(route.Id, _start.AddDays(-7), _start))
                    {
                        return RouteType.None;
                    }
                    if (WasRoutePlanned(route.Id, _start.AddDays(-21), _start))
                    {
                        return RouteType.MaxOnce;
                    }
                    return RouteType.Once;

                //Fortnightly
                case 1:
                    if (WasRoutePlanned(route.Id, _start, _start.AddDays(7)))
                    {
                        return RouteType.None;
                    }
                    if (WasRoutePlanned(route.Id, _start.AddDays(-7), _start))
                    {
                        return RouteType.MaxOnce;
                    }
                    return RouteType.Once;

                //Weekly
                case 2:
                    if (WasRoutePlanned(route.Id, _start, _start.AddDays(7)))
                    {
                        return RouteType.MaxOnce;
                    }
                    return RouteType.MinOnce;

                //Daily
                case 3:
                    return RouteType.Daily;

                //Unknown
                default:
                    return RouteType.None;

            }
        }

        /// <summary>
        /// Checks if route was planned on a day start + daynumber.
        /// </summary>
        /// <param name="routeId">Id of route.</param>
        /// <param name="dayNumber">Number of days from start.</param>
        /// <returns></returns>
        public bool IsPreplanned(int routeId, int dayNumber)
        {
            return _plans.Any(
                    plan => plan.Date == _start.AddDays(dayNumber) &&
                    plan.RouteIds.Any(route => route == routeId)
            );
        }

        /// <summary>
        /// Checks if given route was planned somewhere in the days from start to end.
        /// Including start date and not including end day.
        /// </summary>
        /// <param name="routeId">Id of given route</param>
        /// <param name="start">Start of the range.</param>
        /// <param name="end">End of the range.</param>
        /// <returns></returns>
        private bool WasRoutePlanned(int routeId, DateOnly start, DateOnly end)
        {
            return _plans.Any(
                    plan => plan.Date >= start &&
                    plan.Date < end &&
                    plan.RouteIds.Any(route => route == routeId)
            );
        }
    }
}
