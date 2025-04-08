using App.Server.DTOs;

namespace App.Server.CSP
{
    /// <summary>
    /// Type of Route based on how critical it is, to plan it in this time range.
    /// (how many times it should be planned)
    /// </summary>
    public enum RouteType
    {
        Daily,
        MinOnce,
        Once,
        MaxOnce,
        None
    }

    /// <summary>
    ///  Variable is a class used in solver, it represents a single variable - combination of route and day.
    ///  It is then assigned a value (ranger or null) in solver.
    /// </summary>
    public class Variable
    {
        public int RouteId { get; set; }
        public int DaysFromStart { get; set; }
        public RouteType RouteType { get; set; }

        public int VariableId { get; set; } // used in solver
    }
    /// <summary>
    /// VariableManager is a class that creates variables from provided data.
    /// </summary>
    public class VariableManager
    {
        private readonly IRouteTypeDeterminer _determiner;
        private readonly List<RouteDto> _routes;
        private readonly int _dayCount;
        public VariableManager(IRouteTypeDeterminer determiner, List<RouteDto> routes, int dayCount)
        {
            _determiner = determiner;
            _routes = routes;
            _dayCount = dayCount;
        }
        /// <summary>
        /// Creates variables for all possible combinations of routes and days in the planning range.
        /// Filters out variables that shouldnt be planned at all (None RouteType).
        /// Filters out variables for pairs of route-day that are already planned.
        /// VariableIds are created with a counter from 0-number of variables.
        /// </summary>
        /// <returns>A list of created variables.</returns>
        public List<Variable> GetVariables()
        {
            List<Variable> variables = [];
            int counter = 0;
            foreach (var route in _routes)
            {
                RouteType type = _determiner.DetermineRouteType(route);
                // filter out routes that shouldnt be planned
                if (type == RouteType.None) continue;
                for (int i = 0; i < _dayCount; i++)
                {
                    // filter out pairs that are already planned to avoid duplicating
                    if (_determiner.IsPreplanned(route.Id, i)) continue;
                    variables.Add(new Variable { RouteId = route.Id, DaysFromStart = i, RouteType = type, VariableId = counter++ });
                }
            }
            return variables;
        }

        /// <summary>
        /// Orders provided variables first by RouteType and then by DayFromStart.
        /// </summary>
        /// <param name="variables">List of variables.</param>
        /// <returns>An ordered list of variables.</returns>
        public static List<Variable> GetOrderedVariables(List<Variable> variables)
        {
            return variables.OrderBy(var => var.RouteType).ThenBy(var => var.DaysFromStart).ToList();
        }
    }
}
