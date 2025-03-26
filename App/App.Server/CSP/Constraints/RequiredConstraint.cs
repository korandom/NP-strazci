
namespace App.Server.CSP.Constraints
{
    using VariableID = int;
    using RangerID = int?;

    public class RequiredConstraint(VariableID variable) : Constraint<VariableID, RangerID>([ variable ])
    {
        public override bool IsSatisfied(Dictionary<VariableID, RangerID> assigned)
        {
            if (assigned.TryGetValue(Variables[0], out var value))
            {
                return value != null;
            }
            return true;
        }
    }
}
