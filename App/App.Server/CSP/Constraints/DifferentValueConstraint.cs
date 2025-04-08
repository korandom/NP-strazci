namespace App.Server.CSP.Constraints
{
    using RangerID = int?;
    using VariableID = int;

    public class DifferentValueConstraint(VariableID[] variables) : Constraint<VariableID, RangerID>(variables)
    {
        public override bool IsSatisfied(Dictionary<VariableID, RangerID> assigned)
        {
            var tracked = new HashSet<RangerID>();

            foreach (var variable in Variables)
            {
                if (assigned.TryGetValue(variable, out var value))
                {
                    if (value != null && !tracked.Add(value))
                    {
                        return false;
                    }
                }
            }

            // if no duplicates found, the constraint is not broken (even if none are assigned yet)
            return true;
        }
    }
}
