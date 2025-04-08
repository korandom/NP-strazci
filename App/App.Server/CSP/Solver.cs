
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
        private readonly List<int> plannedMinOnceVariables = [];


        public Solver(List<Variable> variables, Dictionary<int, List<int>> domainsForDays, Dictionary<int, List<int>> domainsForRoutes, Dictionary<TVariableId, List<Constraint<TVariableId, TDomain>>> constraints)
        {
            _variables = variables;
            _domainsForDays = domainsForDays.ToDictionary(day => day.Key, day => day.Value.ToHashSet());
            _domainsForRoutes = domainsForRoutes;
            _constraints = constraints;
        }

        public Dictionary<TVariableId, TDomain>? Solve()
        {
            return SolveBacktrack([]);
        }

        private List<TDomain> GetValidDomain(Variable variable)
        {
            List<TDomain> domain = _domainsForRoutes[variable.RouteId].Where(d => _domainsForDays[variable.DaysFromStart].Contains(d)).Select(x => (int?)x).ToList();
            if (variable.RouteType != RouteType.Daily)
            {
                domain.Add(null);
            }
            return domain;
        }

        private bool CheckConstraints(int varId, Dictionary<TVariableId, TDomain> setVariables)
        {
            if (_constraints.TryGetValue(varId, out var varConstraints))
            {
                return varConstraints.All(c => c.IsSatisfied(setVariables));
            }
            return true;
        }
        private void UpdateDayDomains(Variable variable, TDomain domain)
        {
            if (domain == null) return;

            // the ranger value should not be assigned again in the same day
            _domainsForDays[variable.DaysFromStart].Remove((int)domain);
        }

        private void BacktrackDayDomains(Variable variable, TDomain domain)
        {
            if (domain == null) return;

            _domainsForDays[variable.DaysFromStart].Add((int)domain);
        }

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
        private List<Variable> GetConflictingVariables(Variable variable, int? domain)
        {
            List<Variable> conflictingVars = [];
            if (domain != null && (variable.RouteType == RouteType.Once || variable.RouteType == RouteType.MaxOnce))
            {
                conflictingVars = _variables.Where(var => variable.RouteId == var.RouteId && var.VariableId != variable.VariableId).ToList();
            }
            return conflictingVars;
        }
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
