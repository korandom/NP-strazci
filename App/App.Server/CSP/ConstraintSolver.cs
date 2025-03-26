using System.Collections.Immutable;
using App.Server.CSP.Constraints;

namespace App.Server.CSP
{
    public class ConstraintSolver<TVariable, TDomain>
        where TVariable : notnull
    {
        private TVariable[] _variables;
        private Dictionary<TVariable, List<TDomain>> _domains;
        private Dictionary<TVariable, List<Constraint<TVariable, TDomain>>> _constraints;

        public ConstraintSolver(TVariable[] variables, Dictionary<TVariable, List<TDomain>> domains, Dictionary<TVariable, List<Constraint<TVariable, TDomain>>> constraints)
        {
            _variables = variables;
            _domains = domains;
            _constraints = constraints;
        }

        public Dictionary<TVariable, TDomain>? Solve(Dictionary<TVariable, TDomain>? setVariables = null)
        {
            if (setVariables != null)
            {
                return BackTrack(setVariables);
            }
            else
            {
                return BackTrack(new Dictionary<TVariable, TDomain>());
            }
        }

        private Dictionary<TVariable, TDomain>? BackTrack(Dictionary<TVariable, TDomain> setVariables)
        {
            if (setVariables.Count == _variables.Length)
            {
                return setVariables;
            }
            var variable = _variables.First(variable => !setVariables.ContainsKey(variable));

            foreach (var domain in _domains[variable])
            {
                setVariables[variable]= domain;
                if (_constraints[variable].All(c => c.IsSatisfied(setVariables)))
                {
                    var result = BackTrack(setVariables);
                    if (result != null)
                    {
                        return result;
                    }
                }
                setVariables.Remove(variable);
            }
            return null;
        }
    }
}
