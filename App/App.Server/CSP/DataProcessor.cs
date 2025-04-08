using App.Server.DTOs;

namespace App.Server.CSP
{

    using Rangers = List<int>;
    using RouteDistribution = Dictionary<int, int>;

    /// <summary>
    /// DataProcessor is a class for extracting information - like route distribution, from plans to use for generating new route plan.
    /// It takes into account rangers and routes, that should be included into the processing.
    /// </summary>
    public static class DataProcessor
    {

        /// <summary>
        /// Calculates distribution for all rangers. viz <see cref="CalculateRouteDistribution"/>
        /// </summary>
        /// <returns>A Dictionary<int, RouteDistribution> where key is rangerId. </returns>
        public static Dictionary<int, RouteDistribution> GetRouteDistributions(List<PlanDto> plans, List<RangerDto> rangers, List<RouteDto> routes)
        {
            var rangersPlansDic = plans
                .GroupBy(plan => plan.Ranger.Id)
                .ToDictionary(group => group.Key, group => group.ToList());

            var distributionDic = new Dictionary<int, RouteDistribution>();

            foreach (var ranger in rangers)
            {
                distributionDic[ranger.Id] = CalculateRouteDistribution(rangersPlansDic.TryGetValue(ranger.Id, out var rangerPlans) ? plans : [], routes);
            }

            return distributionDic;
        }

        /// <summary>
        ///  Calculates from the given plans a percentage distribution of routes.
        ///  If plans of one ranger are given, the result can be compared to results of other rangers
        ///  to assess data not by absolute numbers ("ranger went to this route 5 times this month") 
        ///  but by relative numbers ("ranger went to this route 80% of the time")
        ///  This is helpful, because some rangers can go to work much less than others and absolute numbers wouldnt be telling.
        ///  
        ///  The distribution is represented as an int between 0 and 1000 which is coresponding to percentage * 10.  
        /// </summary>
        /// <param name="plans">Given Plans</param>
        /// <returns>
        /// A Dictionary<int,int>, where the key is a routeId and the value is distribution of that route in the given plans.
        /// </returns>
        private static RouteDistribution CalculateRouteDistribution(List<PlanDto> plans, List<RouteDto> routes)
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
        /// If some are preplanned, they are moved to the back.
        /// </summary>
        /// <param name="routeDistributions">Route distribution of all rangers.</param>
        /// <returns>A Dictionary with key being RouteId and value are Rangers - a list of rangerIDs, ordered by best to worst for route.</returns>
        public static Dictionary<int, Rangers> GetBestRangersForRoutes(Dictionary<int, RouteDistribution> routeDistributions, List<RouteDto> routes, List<PlanDto> preexistingPlans)
        {
            var bestRangers = routes.ToDictionary(route => route.Id, route => DetermineBestRangers(route.Id, routeDistributions));
            foreach (var plan in preexistingPlans)
            {
                foreach (var routeId in plan.RouteIds)
                {
                    bestRangers[routeId].Remove(plan.Ranger.Id);
                    bestRangers[routeId].Add(plan.Ranger.Id);
                }
            }
            return bestRangers;
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
        /// Get preexisting plans that cant be reassigned.
        /// </summary>
        /// <param name="preexistingPlans"></param>
        /// <returns></returns>
        public static List<PlanDto> GetFixedPreviousPlans(List<PlanDto> preexistingPlans)
        {
            return preexistingPlans.Where(plan =>
                plan.RouteIds.Length >= 2 ||
                preexistingPlans.Any(second =>
                    plan.Date == second.Date &&
                    plan.Ranger.Id != second.Ranger.Id &&
                    plan.RouteIds.Intersect(second.RouteIds).Any()))
            .ToList();
        }



        /// <summary>
        /// Converts assignment of values to variables to plans.
        /// </summary>
        /// <param name="assignment">Generated assignment.</param>
        /// <param name="variables">Variables.</param>
        /// <returns></returns>
        public static List<PlanDto> ConvertAssignmentToPlan(Dictionary<int, int?> assignment, List<Variable> variables, List<RangerDto> rangers, DateOnly start)
        {
            List<PlanDto> plans = [];
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
