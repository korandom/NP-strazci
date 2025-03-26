

using App.Server.DTOs;
using System.Collections.Immutable;

namespace App.Server.CSP
{
    /// <summary>
    /// Type of Route based on how critical it is, to plan it in this time range.
    /// </summary>
    public enum RouteType
    {
        Daily,
        Once,
        Meh
    }
    
    /// <summary>
    ///  
    /// </summary>
    public class Variable
    {
        public int RouteId { get; set; }
        public int DaysFromStart { get; set; }
        public RouteType RouteType { get; set; }

        public int VariableId { get; set; } // used in solver
    }

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
        public  List<Variable> GetVariables()
        {
            List<Variable> variables = new List<Variable>();
            int counter = 0;
            foreach (var route in _routes)
            {
                RouteType type = _determiner.DetermineRouteType(route);
                for (int i = 0; i < _dayCount; i++)
                {
                    variables.Add(new Variable { RouteId = route.Id, DaysFromStart = i, RouteType = type, VariableId = counter++ });
                }
            }
            return variables;
        }

        public static int[] GetOrderedVariableIds(List<Variable> variables)
        {
            Random random = new Random();
            return variables.OrderBy(variable=> variable.DaysFromStart).ThenBy(variable=> variable.RouteType).ThenBy(variable => random.Next()).Select(variable=> variable.VariableId).ToArray();
        }

        public static Dictionary<int, List<int?>> CreateDomains(List<Variable> variables, Dictionary<int, List<int>> workingRangersForDays, Dictionary<int, List<int>> bestRangersForRoutes)
        {
            Dictionary<int, List<int?>> domains = new Dictionary<int, List<int?>>();
            foreach (var variable in variables)
            {
                List<int?> possibleRangers = new List<int?>();
                foreach (var ranger in bestRangersForRoutes[variable.RouteId])
                {
                    if (workingRangersForDays[variable.DaysFromStart].Contains(ranger))
                    {
                        possibleRangers.Add(ranger);
                    }
                }

                // option to not plan any ranger for it
                if(variable.RouteType != RouteType.Daily)
                {
                    possibleRangers.Add(null);
                }

                domains.Add(variable.VariableId, possibleRangers);
            }
            return domains;
        }
    }
}
