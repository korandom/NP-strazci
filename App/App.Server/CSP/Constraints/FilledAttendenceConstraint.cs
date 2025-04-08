namespace App.Server.CSP.Constraints
{
    using RangerID = int?;
    using VariableID = int;

    public class FilledAttendenceConstraint(VariableID[] variables, int minimalPlanned) : Constraint<VariableID, RangerID>(variables)
    {
        private readonly int _minimalPlanned = minimalPlanned;

        public override bool IsSatisfied(Dictionary<VariableID, RangerID> assigned)
        {
            // counting variables with either assigned rangers, or not assigned at all (not "assigned null")
            int notNullCounter = 0;

            foreach (var variable in Variables)
            {
                if (!assigned.TryGetValue(variable, out var value) || value != null)
                {
                    notNullCounter++;
                }
            }
            // returns true if there is enough rangers working / future possibility of enough working to fill attendence
            return notNullCounter >= _minimalPlanned;
        }
    }
}
