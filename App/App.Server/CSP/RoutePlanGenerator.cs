using App.Server.CSP.Constraints;
using App.Server.DTOs;
using App.Server.Models.AppData;

namespace App.Server.CSP
{
    using Rangers = List<int>;

    /// <summary>
    /// Generates new route plan according to previous data, new preselected routes, route fairness distribution, attendence and route priorities.
    /// </summary>
    public class RoutePlanGenerator
    {
        private readonly DataProcessor _dataProcessor;
        private readonly VariableManager _variableManager;
        private readonly List<PlanDto> _preexistingPlans;
        private readonly Dictionary<int, Rangers> _workingRangers;
        static readonly int dayCount = 7; // logic is heavily based on planning for 7 days, changing this would require a lot of refactoring


        public RoutePlanGenerator(List<PlanDto> previousPlans, List<PlanDto> preexistingPlans, List<AttendenceDto> attendences, List<RouteDto> routes, List<RangerDto> rangers, DateOnly start)
        {
            this._dataProcessor = new DataProcessor(previousPlans, rangers, routes, start);
            this._variableManager = new VariableManager(_dataProcessor, routes, dayCount);
            this._preexistingPlans = preexistingPlans;
            this._workingRangers = DataProcessor.GetWorkingRangers(attendences, dayCount, start);
        }

        /// <summary>
        /// Generates a new route plan for a week.
        /// </summary>
        /// <returns>
        /// A Successful result with generated route plan upon finding solution.
        /// A Semi succesfull result with generated route plan and a warning, if preselected routes had to be rewritten.
        /// A Failed result if finding solution was not sucessful.
        /// </returns>
        public GenerateResultDto Generate()
        {
            // process data 
            var routeDistributions = _dataProcessor.GetRouteDistributions();
            var bestRangersForRoutes = _dataProcessor.GetBestRangersForRoutes(routeDistributions);

            // set up variables and domains and check if possible at all to solve
            var variables = _variableManager.GetVariables();
            var variableIDs = VariableManager.GetOrderedVariableIds(variables);

            if (!CheckEnoughAvailableRangers(variables, _workingRangers))
            {
                return new GenerateResultDto() { 
                    Success=false, 
                    Message="Nedostatečná docházka pro naplnění důležitosti tras."
                }; 
            }

            var domains = VariableManager.CreateDomains(variables, _workingRangers, bestRangersForRoutes);

            // set up constraints
            var builder = new ConstraintSolverBuilder<int,int?>(variableIDs, domains);
            AddConstraints(builder, variables);

            //solve
            var solver = builder.Build();

            //TODO set up preexisting plans 
            var assignedVariables = solver.Solve();
            if (assignedVariables == null)
            {
                return new GenerateResultDto()
                {
                    Success = false,
                    Message = "Generování nebylo úspěšné."
                };
            }
            else
            {
                return new GenerateResultDto()
                {
                    Success = true,
                    Plans = _dataProcessor.ConvertAssignmentToPlan(assignedVariables, variables)
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
            int dailyVariableCount = variables.Where(variable => variable.RouteType == RouteType.Daily).Count();
            int onceVariableCount = variables.Where(variable => variable.RouteType == RouteType.Once).Count();
            int rangerDaysCount = workingRangers.SelectMany(workingRangers => workingRangers.Value).Count();

            // each day has at least enough working rangers to cover daily routes
            bool satisfiesMinDayCapacity = workingRangers.All(rangers => rangers.Value.Count() >= dailyVariableCount / dayCount);

            // enough working rangers in whole to cover daily and at least once routes
            int minimalCoverage = dailyVariableCount + onceVariableCount / dayCount;
            bool satisfiesMinCumulative = rangerDaysCount - minimalCoverage >= 0;

            return satisfiesMinDayCapacity && satisfiesMinCumulative;
        }


        /// <summary>
        /// Add all constraints derived from the variables to the solver builder.
        /// </summary>
        /// <param name="builder">Given Builder</param>
        /// <param name="variables">Given Variables</param>
        private void AddConstraints(ConstraintSolverBuilder<int, int?> builder, List<Variable> variables)
        {
            var dateGrouping = variables.GroupBy(variable => variable.DaysFromStart);
            foreach (var dateGroup in dateGrouping)
            {
                int[] varIds = dateGroup.Select(group => group.VariableId).ToArray();
                int rangerCount = _workingRangers[dateGroup.Key].Count;
                builder.AddConstraint(new FilledAttendenceConstraint(varIds, rangerCount));
                builder.AddConstraint(new DifferentValueConstraint(varIds));
            }

            var onceRouteGrouping = variables.Where(variable => variable.RouteType == RouteType.Once)
                                             .GroupBy(variable => variable.RouteId);
            foreach (var onceRouteGroup in onceRouteGrouping)
            {
                int[] varIds = onceRouteGroup.Select(group => group.VariableId).ToArray();
                builder.AddConstraint(new SomeAssignedConstraint(varIds));
            }
        }
    }
}
