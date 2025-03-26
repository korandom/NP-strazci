using App.Server.DTOs;
using System.Collections.Immutable;

namespace App.Server.CSP
{

    using RouteDistribution = Dictionary<int, int>;
    using Rangers = List<int>;
    public interface IRouteTypeDeterminer
    {
        public RouteType DetermineRouteType(RouteDto route);
    }
    /// <summary>
    /// DataProcessor is a class for extracting information - like route distribution, from past plans to use for generating new route plan.
    /// It takes into account rangers and routes, that should be included into the processing.
    /// </summary>
    public class DataProcessor : IRouteTypeDeterminer
    {
        private readonly List<PlanDto> previousPlans;
        private readonly List<RangerDto> rangers;
        private readonly List<RouteDto> routes;
        private readonly DateOnly start;

        /// <summary>
        /// Contructor of DataProcessor.
        /// </summary>
        /// <param name="previousPlans">Previous plans, should be all plans in a 3 week span before start of generated.</param>
        /// <param name="rangers">Rangers for who the information will be created.</param>
        /// <param name="routes">Routes for which the information will be created.</param>
        public DataProcessor(List<PlanDto> previousPlans, List<RangerDto> rangers, List<RouteDto> routes, DateOnly start)
        {
            this.previousPlans = previousPlans;
            this.rangers = rangers;
            this.routes = routes;
            this.start = start;
        }

        /// <summary>
        /// Calculates distribution for all rangers. viz <see cref="CalculateRouteDistribution"/>
        /// </summary>
        /// <returns>A Dictionary<int, RouteDistribution> where key is rangerId. </returns>
        public Dictionary<int, RouteDistribution> GetRouteDistributions()
        {
            var rangersPlansDic = previousPlans
                .GroupBy(plan => plan.Ranger.Id)
                .ToDictionary(group => group.Key, group => group.ToList());

            var distributionDic = new Dictionary<int, RouteDistribution>();

            foreach (var ranger in rangers)
            {
                distributionDic[ranger.Id] = CalculateRouteDistribution(rangersPlansDic.TryGetValue(ranger.Id, out var plans) ? plans : new List<PlanDto>());
            }

            return distributionDic;
        }

        /// <summary>
        ///  Calculates from the given plans a percentage distribution of routes.
        ///  If plans of one ranger are given, the result can be compared to results of other rangers
        ///  to assess data not by absolute numbers ("ranger went to this route 5 times this month") 
        ///  but by relative numbers ("ranger went to this route 80% of the time")
        ///  This is helpful, because some rangers can go to work much less than others and absolute numbers woudlnt be telling.
        ///  
        ///  The distribution is represented as an int between 0 and 1000 which is coresponding to percentage * 10.  
        /// </summary>
        /// <param name="plans">Given Plans</param>
        /// <returns>
        /// A Dictionary<int,int>, where the key is a routeId and the value is distribution of that route in the given plans.
        /// </returns>
        private RouteDistribution CalculateRouteDistribution(List<PlanDto> plans)
        {
            var distributionDic = routes.ToDictionary(route => route.Id, route => 0);
            var allPlannedRoutes = plans.SelectMany(plan => plan.RouteIds).ToList();
            var count = allPlannedRoutes.Count;
            if (count == 0) return distributionDic;

            var calculatedDistributionDic = allPlannedRoutes
                .GroupBy(route => route)
                .ToDictionary(
                    group => group.Key,
                    group => (int)(group.Count() / (double)count * 1000) // distribution is percentage * 10
                );

            foreach (var (routeId, distribution) in calculatedDistributionDic)
            {
                distributionDic[routeId] = distribution;
            }

            return distributionDic;
        }

        /// <summary>
        /// Orders all rangers for each route from best suited to least -- according to the routeDistribution.
        /// Rangers with least route distribution come first.
        /// </summary>
        /// <param name="routeDistributions">Route distribution of all rangers.</param>
        /// <returns>A Dictionary with key being RouteId and value are Rangers - a list of rangerIDs, ordered by best to worst for route.</returns>
        public Dictionary<int, Rangers> GetBestRangersForRoutes(Dictionary<int, RouteDistribution> routeDistributions)
        {
            return routes.ToDictionary(route => route.Id, route => DetermineBestRangers(route.Id, routeDistributions));
        }

        /// <summary>
        /// Determines best rangers for a given route according to routeDistributions.
        /// Route distribution should iclude even routes with 0 distribution value.
        /// </summary>
        /// <param name="routeId">Id of route</param>
        /// <param name="routeDistributions">All route distributions for all rangers</param>
        /// <returns>A list of rangers ordered from best to worst.</returns>
        private static Rangers DetermineBestRangers(int routeId, Dictionary<int, RouteDistribution> routeDistributions)
        {
            return routeDistributions
                .Where(distribution => distribution.Value.ContainsKey(routeId))
                .OrderBy(distribution => distribution.Value[routeId])
                .Select(distribution => distribution.Key)
                .ToList();
        }

        /// <summary>
        /// Gets rangers that are working per days in interval <start, start+daycount).
        /// Days are represented by an integer - number of days since start of range.
        /// 
        /// </summary>
        /// <param name="attendences">Attendence</param>
        /// <param name="startDate">Start date of the range</param>
        /// <param name="dayCount">Number of days total.</param>
        /// <returns>A dictionary, where key is a day represented as integer and values are rangers working that day - lists of rangerIds.</returns>
        public static Dictionary<int, Rangers> GetWorkingRangers(List<AttendenceDto> attendences, int dayCount, DateOnly startDate)
        {
            var attendenceDic = attendences
                .Where(attendence => attendence.Working == true && attendence.Date >= startDate && attendence.Date < startDate.AddDays(dayCount))
                .GroupBy(attendece => attendece.Date.DayNumber - startDate.DayNumber)
                .ToDictionary(group => group.Key, group => group.Select(att => att.Ranger.Id).ToList());

            // add empty Lists for no available rangers
            var finalDic = Enumerable.Range(0, dayCount).ToDictionary(seq => seq, seq => attendenceDic.TryGetValue(seq, out var value) ? value : []);
            return finalDic;
        }

        /// <summary>
        /// Determines what type of route the given route is in the context of recent plans.
        /// This implementation is based on a dayCount of 7 - one week! 
        /// If the planning period would change, this method does not work as intended and must be refactored.
        /// Routes with a Daily priority have a RouteType Daily.
        /// Routes with a Weekly priority have a RouteType Once.
        /// Routes with a Forthnighly priority have a RouteType of Once if not planned the previous week, else Meh.
        /// Routes with a Monthly priority have a RouteType of Once if not planned the previous 3 weeks, else Meh.
        /// </summary>
        /// <param name="route">Given Route</param>
        /// <returns>RouteType of the route</returns>
        public RouteType DetermineRouteType(RouteDto route)
        {
            switch (route.Priority)
            {
                //Monthly
                case 0:
                    if (WasRoutePlanned(route.Id, 28))
                    {
                        return RouteType.Meh;
                    }
                    return RouteType.Once;

                //Fortnightly
                case 1:
                    if (WasRoutePlanned(route.Id, 7))
                    {
                        return RouteType.Meh;
                    }
                    return RouteType.Once;

                //Weekly
                case 2:
                    return RouteType.Once;

                //Daily
                case 3:
                    return RouteType.Daily;

                //Unknown
                default:
                    return RouteType.Meh;

            }
        }

        /// <summary>
        /// Checks if given route was planned in the recent past with at most shift number of days before start.
        /// </summary>
        /// <param name="routeId">Id of given route</param>
        /// <param name="shift">Shift determines the range in which we are searching</param>
        /// <returns></returns>
        private bool WasRoutePlanned(int routeId, int shift)
        {
            DateOnly end = start.AddDays(-shift);

            bool exists = previousPlans.Any(
                    plan => plan.Date >= end &&
                    plan.Date < start &&
                    plan.RouteIds.Any(route => route == routeId)
            );
            return exists;
        }

        /// <summary>
        /// From the 
        /// </summary>
        /// <param name="assignment"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public List<PlanDto> ConvertAssignmentToPlan(Dictionary<int, int?> assignment, List<Variable> variables)
        {
            List<PlanDto> plans = new List<PlanDto>();
            Dictionary<int, Variable> variableDictionary = variables.ToDictionary(variable => variable.VariableId, variable => variable);
            Dictionary<int, RangerDto> rangerDictionary = rangers.ToDictionary(ranger => ranger.Id, ranger => ranger);
            foreach (var assigned in assignment)
            {
                if (assigned.Value != null)
                {
                    Variable var = variableDictionary[assigned.Key];
                    var ranger = rangerDictionary[(int)assigned.Value];
                    plans.Add(new PlanDto(start.AddDays(var.DaysFromStart), ranger, [var.RouteId], []));
                }
            }
            return plans;
        }
    }
}
