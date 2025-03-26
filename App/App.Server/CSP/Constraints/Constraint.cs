using System.Collections.Immutable;

namespace App.Server.CSP.Constraints
{
    public abstract class Constraint<TVariable, TDomain>(TVariable[] variables)
        where TVariable : notnull
    {
        public TVariable[] Variables { get; } = variables;

        public abstract bool IsSatisfied(Dictionary<TVariable, TDomain> assigned);
    }
}
