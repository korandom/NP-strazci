using App.Server.CSP.Constraints;
using System.Collections.Immutable;

namespace App.Server.CSP
{
    public class ConstraintSolverBuilder<TVariable, TDomain>
        where TVariable : notnull
    {
        private TVariable[] _variables;
        private Dictionary<TVariable, List<TDomain>> _domains;
        private Dictionary<TVariable, List<Constraint<TVariable, TDomain>>> _constraints;

        public ConstraintSolverBuilder(TVariable[] variables, Dictionary<TVariable, List<TDomain>> domains)
        {
            _variables = variables;
            _domains = domains;
            _constraints = new Dictionary<TVariable, List<Constraint<TVariable, TDomain>>>();
        }


        public void AddConstraint(Constraint<TVariable, TDomain> constraint)
        {
            foreach (var variable in constraint.Variables)
            {
                if (_constraints.TryGetValue(variable, out var constraints))
                {
                    constraints.Add(constraint);
                }
                else
                {
                    _constraints.Add(variable, new List<Constraint<TVariable, TDomain>>() { constraint });
                }
            }
        }

        public ConstraintSolver<TVariable, TDomain> Build()
        {
            return new ConstraintSolver<TVariable, TDomain>(
                _variables,
                _domains,
                _constraints
            );
        }
    }
}
