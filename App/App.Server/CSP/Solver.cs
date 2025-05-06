
using App.Server.CSP.Constraints;


namespace App.Server.CSP
{
    using TDomain = int?;
    using TVariableId = int;

    /// <summary>
    /// Solver is a class for finding assignment of rangers from provided domains to variables.
    /// Using a backtracking algorithm to satisfy constraints.
    /// </summary>
    public class Solver
    {
        private readonly List<Variable> _variables;
        private readonly Dictionary<int, HashSet<int>> _domainsForDays;
        private readonly Dictionary<int, List<int>> _domainsForRoutes;
        private readonly Dictionary<TVariableId, List<Constraint<TVariableId, TDomain>>> _constraints;

        // for tracking min once variables
        private readonly List<int> plannedMinOnceVariables = [];


        /// <summary>
        /// Constructor of Solver.
        /// </summary>
        /// <param name="variables">List of variables.</param>
        /// <param name="domainsForDays">Dictionary, where keys are days and values are lists of available rangers.</param>
        /// <param name="domainsForRoutes">Dictionary, where keys are routes and values are sorted lists of rangers by best suited.</param>
        /// <param name="constraints">List of constraints for all variables.</param>
        public Solver(List<Variable> variables, Dictionary<int, List<int>> domainsForDays, Dictionary<int, List<int>> domainsForRoutes, Dictionary<TVariableId, List<Constraint<TVariableId, TDomain>>> constraints)
        {
            _variables = variables;
            _domainsForDays = domainsForDays.ToDictionary(day => day.Key, day => day.Value.ToHashSet());
            _domainsForRoutes = domainsForRoutes;
            _constraints = constraints;
        }

        /// <summary>
        /// Attempts to find a valid assignment of values from domains (rangers) to variables.
        /// </summary>
        /// <returns>
        /// A dictionary mapping variable IDs to ranger IDs (or null if unassigned),
        /// or null if no solution could be found.
        /// </returns>
        public Dictionary<TVariableId, TDomain>? Solve()
        {
            return SolveBacktrack([]);
        }

        /// <summary>
        /// Returns the set of valid ranger IDs for a given variable,
        /// respecting both day and route domains. Adds null for non-daily routes.
        /// </summary>
        private List<TDomain> GetValidDomain(Variable variable)
        {
            List<TDomain> domain = _domainsForRoutes[variable.RouteId].Where(d => _domainsForDays[variable.DaysFromStart].Contains(d)).Select(x => (int?)x).ToList();
            if (variable.RouteType != RouteType.Daily)
            {
                domain.Add(null);
            }
            return domain;
        }

        /// <summary>
        /// Checks if all constraints for a variable varId are satisfied for the current assignment.
        /// </summary>
        private bool CheckConstraints(int varId, Dictionary<TVariableId, TDomain> setVariables)
        {
            if (_constraints.TryGetValue(varId, out var varConstraints))
            {
                return varConstraints.All(c => c.IsSatisfied(setVariables));
            }
            return true;
        }

        /// <summary>
        /// Updates the domain for a day by removing an assigned ranger to prevent duplicate assignment.
        /// </summary>
        private void UpdateDayDomains(Variable variable, TDomain domain)
        {
            if (domain == null) return;

            // the ranger value should not be assigned again in the same day
            _domainsForDays[variable.DaysFromStart].Remove((int)domain);
        }

        /// <summary>
        /// Re-adds a ranger to the day domain when backtracking.
        /// </summary>
        private void BacktrackDayDomains(Variable variable, TDomain domain)
        {
            if (domain == null) return;

            _domainsForDays[variable.DaysFromStart].Add((int)domain);
        }

        /// <summary>
        /// Updates the route domain and tracks planning of MinOnce routes.
        /// Moves the newly assigned ranger to the back of the sorted list.
        /// </summary>
        /// <returns>The index where the ranger was originally located for backtracking.</returns>
        private int UpdateRouteDomains(Variable variable, TDomain domain)
        {
            if (domain == null) return -1;
            int value = (int)domain;
            int index = _domainsForRoutes[variable.RouteId].IndexOf(value);
            _domainsForRoutes[variable.RouteId].RemoveAt(index);
            _domainsForRoutes[variable.RouteId].Add(value);
            if (variable.RouteType == RouteType.MinOnce)
            {
                plannedMinOnceVariables.Add(variable.RouteId);
            }
            return index;
        }

        /// <summary>
        /// Restores route domain to its previous placement in sorted list and unmarks MinOnce planning if necessary.
        /// </summary>
        private void BacktrackRouteDomains(Variable variable, TDomain domain, int index)
        {
            if (domain == null || index == -1) return;
            int value = (int)domain;

            _domainsForRoutes[variable.RouteId].Remove(value);
            _domainsForRoutes[variable.RouteId].Insert(index, value);
            if (variable.RouteType == RouteType.MinOnce)
            {
                plannedMinOnceVariables.Remove(variable.RouteId);
            }
        }

        /// <summary>
        /// Chooses the next variable from not assigned variables to be assigned next based on priority and suitability.
        /// First are chosen variables with daily types, then once and min once and rest next.
        /// Daily are sorted by date, rest categories by suitability.
        /// </summary>
        private Variable ChooseVariable(Dictionary<TVariableId, TDomain> setVariables)
        {
            List<Variable> filtered = _variables.Where(var => !setVariables.ContainsKey(var.VariableId)).ToList();

            // first daily variables
            var dailyVariable = filtered.Find(var => var.RouteType == RouteType.Daily);
            if (dailyVariable != null)
            {
                return dailyVariable;
            }

            // then variables that must be assigned to fullfill priority -- once and min once
            var priorityVariables = filtered.Where(var => var.RouteType == RouteType.Once || (var.RouteType == RouteType.MinOnce && !plannedMinOnceVariables.Contains(var.RouteId))).ToList();
            if (priorityVariables != null && priorityVariables.Count > 0)
            {
                return priorityVariables.OrderBy(var => GetVariableSuitabilityFactor(_domainsForDays[var.DaysFromStart], _domainsForRoutes[var.RouteId])).First();
            }

            // choose best suitable route from the not priority routes
            return filtered.OrderBy(var => GetVariableSuitabilityFactor(_domainsForDays[var.DaysFromStart], _domainsForRoutes[var.RouteId])).ThenByDescending(var => var.RouteType).First();
        }

        /// <summary>
        /// Heuristic to rank how suitable a variable's domain is based on overlap.
        /// The smaller the number, the better.
        /// Variables with no domain overlap go first => fail first.
        /// </summary>
        private static int GetVariableSuitabilityFactor(HashSet<int> workingRangers, List<int> bestRangers)
        {
            int count = 0;
            int bestRangersCount = bestRangers.Count;
            for (int i = 0; i < bestRangersCount; i++)
            {
                if (workingRangers.Contains(bestRangers[i]))
                {
                    count += i;
                }
            }
            return count;
        }

        /// <summary>
        /// Identifies other variables that conflict with this one (e.g., Once routes can't repeat).
        /// </summary>
        /// <param name="variable">Newly assigned variable</param>
        /// <param name="domain">Domain value being assigned</param>
        /// <returns></returns>
        private List<Variable> GetConflictingVariables(Variable variable, int? domain)
        {
            List<Variable> conflictingVars = [];
            if (domain != null && (variable.RouteType == RouteType.Once || variable.RouteType == RouteType.MaxOnce))
            {
                conflictingVars = _variables.Where(var => variable.RouteId == var.RouteId && var.VariableId != variable.VariableId).ToList();
            }
            return conflictingVars;
        }

        /// <summary>
        /// Core backtracking algorithm for solving the CSP.
        /// </summary>
        /// <param name="setVariables">Current state of assigned variables.</param>
        /// <returns>Complete assignment dictionary or null if no valid solution exists.</returns>
        private Dictionary<TVariableId, TDomain>? SolveBacktrack(Dictionary<TVariableId, TDomain> setVariables)
        {
            // check for complete assignment
            if (setVariables.Count == _variables.Count)
            {
                return setVariables;
            }

            var variable = ChooseVariable(setVariables);
            int varId = variable.VariableId;

            foreach (var domain in GetValidDomain(variable))
            {
                setVariables[varId] = domain;
                if (CheckConstraints(varId, setVariables))
                {
                    // update domains
                    UpdateDayDomains(variable, domain);
                    int index = UpdateRouteDomains(variable, domain);


                    // update conflicting variables
                    List<Variable> conflictingVariables = GetConflictingVariables(variable, domain);
                    foreach (var var in conflictingVariables)
                    {
                        setVariables[var.VariableId] = null;
                    }


                    // move forward
                    var result = SolveBacktrack(setVariables);
                    if (result != null)
                    {
                        return result;
                    }

                    // backtrack domains
                    BacktrackDayDomains(variable, domain);
                    BacktrackRouteDomains(variable, domain, index);

                    // backtrack conflicting variables
                    foreach (var var in conflictingVariables)
                    {
                        setVariables.Remove(var.VariableId);
                    }
                }
                // unassign
                setVariables.Remove(varId);
            }
            return null;
        }
    }
}
