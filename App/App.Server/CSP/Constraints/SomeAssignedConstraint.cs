

namespace App.Server.CSP.Constraints
{
    using RangerID = int?;
    using VariableID = int;

    public class SomeAssignedConstraint(VariableID[] variables) : Constraint<VariableID, RangerID>(variables)
    {
        public override bool IsSatisfied(Dictionary<VariableID, RangerID> assigned)
        {
            foreach (var variable in Variables)
            {
                if (!assigned.TryGetValue(variable, out var value) || value != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
