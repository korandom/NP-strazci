using App.Server.CSP.Constraints;
using App.Server.DTOs;
using System.Data;


namespace App.Server.CSP
{
    using Rangers = List<int>;

    /// <summary>
    /// Generates new route plan according to previous data, new preselected routes, route fairness distribution, attendence and route priorities.
    /// </summary>
    public class RoutePlanGenerator
    {
        private static readonly int dayCount = 7;

        /// <summary>
        /// Generates a new route plan for a week or gives a reason why no route plan was planned by returning a generated result.
        /// </summary>
        /// <returns>
        /// A Successful result with generated route plan upon finding solution.
        /// A Semi succesfull result with generated route plan and a warning, if preselected routes had to be rewritten.
        /// A Failed result if finding solution was not sucessful.
        /// </returns>
        public GenerateResultDto Generate(List<PlanDto> previousPlans, List<PlanDto> preexistingPlans, List<AttendenceDto> attendences, List<RouteDto> routes, List<RangerDto> rangers, DateOnly start)
        {
            List<PlanDto> fixedPlans = DataProcessor.GetFixedPreviousPlans(preexistingPlans);

            // try to generate a result that accomodates all preexistingPlans
            List<AttendenceDto> filteredAttendence = attendences.Where(att => !preexistingPlans.Any(plan => plan.Date == att.Date && plan.Ranger.Id == att.Ranger.Id && plan.RouteIds.Length > 0)).ToList();
            var result = TrySolving([.. previousPlans, .. preexistingPlans], preexistingPlans, filteredAttendence, routes, rangers, start);

            if (result.Success)
            {
                result.Plans = result.Plans.Concat(preexistingPlans);
                return result;
            }

            List<AttendenceDto> lessRestraintAttendence = attendences.Where(att => !fixedPlans.Any(plan => plan.Date == att.Date && plan.Ranger.Id == att.Ranger.Id && plan.RouteIds.Length > 0)).ToList();
            var resultWithoutPreferences = TrySolving([.. previousPlans, .. fixedPlans], fixedPlans, lessRestraintAttendence, routes, rangers, start);
            if (resultWithoutPreferences.Success)
            {
                resultWithoutPreferences.Plans = resultWithoutPreferences.Plans.Concat(fixedPlans);
                resultWithoutPreferences.Message = "Pro naplnění priority tras došlo k přeplánování manuálně naplánovaných tras.";
            }
            return resultWithoutPreferences;
        }

        private GenerateResultDto TrySolving(List<PlanDto> allPlans, List<PlanDto> preplanned, List<AttendenceDto> attendences, List<RouteDto> routes, List<RangerDto> rangers, DateOnly start)
        {

            RouteDeterminer determiner = new(start, allPlans);
            VariableManager variableManager = new(determiner, routes, dayCount);

            // process data 
            var routeDistributions = DataProcessor.GetRouteDistributions(allPlans, rangers, routes);
            var bestRangersForRoutes = DataProcessor.GetBestRangersForRoutes(routeDistributions, routes, preplanned);
            var workingRangers = DataProcessor.GetWorkingRangers(attendences, dayCount, start);

            // set up variables and check 
            var variables = variableManager.GetVariables();

            // check if possible to solve 
            if (!CheckEnoughAvailableRangers(variables, workingRangers))
            {
                return new GenerateResultDto
                {
                    Success = false,
                    Message = "Nedostatečná docházka pro naplnění důležitosti tras."
                };

            }

            List<Variable> orderedVariables = VariableManager.GetOrderedVariables(variables);

            // set up constraints
            var constraints = GetConstraints(orderedVariables);
            Solver solver = new(orderedVariables, workingRangers, bestRangersForRoutes, constraints);

            var assignedVariables = solver.Solve();

            if (assignedVariables == null)
            {
                return new GenerateResultDto
                {
                    Success = false,
                    Message = "Generování nebylo úspěšné."
                };
            }

            else
            {
                return new GenerateResultDto
                {
                    Success = true,
                    Plans = DataProcessor.ConvertAssignmentToPlan(assignedVariables, variables, rangers, start)
                };
            }
        }
        /// <summary>
        /// Checks if there is enough working rangers to cover the route priorities for this time period expressed in variables.
        /// </summary>
        /// <param name="variables"></param>
        /// <param name="workingRangers">Lists of working rangers per day</param>
        /// <returns></returns>
        private static bool CheckEnoughAvailableRangers(List<Variable> variables, Dictionary<int, Rangers> workingRangers)
        {
            var dailyVariables = variables.Where(variable => variable.RouteType == RouteType.Daily).GroupBy(variable => variable.DaysFromStart).ToDictionary(group => group.Key, group => group.Count());
            int onceVariableCount = variables.Where(variable => variable.RouteType == RouteType.Once || variable.RouteType == RouteType.MinOnce).GroupBy(variable => variable.RouteId).Count();
            int rangerDaysCount = workingRangers.SelectMany(workingRangers => workingRangers.Value).Count();

            // each day has at least enough working rangers to cover daily routes
            bool satisfiesMinDayCapacity = workingRangers.All(rangers => rangers.Value.Count >= dailyVariables[rangers.Key]);

            // enough working rangers in whole to cover daily and at least once routes
            int minimalCoverage = dailyVariables.Sum(daily => daily.Value) + onceVariableCount;
            bool satisfiesMinCumulative = rangerDaysCount - minimalCoverage >= 0;

            return satisfiesMinDayCapacity && satisfiesMinCumulative;
        }


        /// <summary>
        /// Add all constraints derived from the variables.
        /// </summary>

        /// <param name="variables">Given Variables</param>
        private Dictionary<int, List<Constraint<int, int?>>> GetConstraints(List<Variable> variables)
        {
            Dictionary<int, List<Constraint<int, int?>>> constraints = [];
            var onceRouteGrouping = variables.Where(variable => variable.RouteType == RouteType.Once)
                                             .GroupBy(variable => variable.RouteId);
            foreach (var onceRouteGroup in onceRouteGrouping)
            {
                int[] varIds = onceRouteGroup.Select(group => group.VariableId).ToArray();
                AddConstraint(new SomeAssignedConstraint(varIds), constraints);
            }
            return constraints;
        }

        private void AddConstraint(Constraint<int, int?> constraint, Dictionary<int, List<Constraint<int, int?>>> constraints)
        {
            foreach (var variable in constraint.Variables)
            {
                if (constraints.TryGetValue(variable, out var varConstraints))
                {
                    varConstraints.Add(constraint);
                }
                else
                {
                    constraints.Add(variable, [constraint]);
                }
            }
        }
    }
}
