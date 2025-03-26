using App.Server.CSP.Constraints;


namespace Tests.CSP.ConstraintsTests
{
    public class FilledAttendenceConstraintTest
    {

        // Varible one is assigned a value, two and three are assigned null
        [Theory]
        // since one is assigned, already fullfills attendence of one
        [InlineData(new int[] { 1, 2 },  1, true)]
        [InlineData(new int[] { 1, 3 }, 1, true)]
        // both two and three are assigned null, cant fullfill attendence
        [InlineData(new int[] { 2, 3 }, 1, false)]
        // four is not assigned yet, still could fullfill attendence in the future
        [InlineData(new int[] { 1, 4 }, 2, true)]
        // two is null, even if four would get assigned in the future, can not fullfill attendence of two
        [InlineData(new int[] { 2, 4 }, 2, false)]
        // neither yet assigned
        [InlineData(new int[] { 4 , 5 }, 2, true)]


        public void SimpleScenarios(int[] variablesArray, int minimalAssigned, bool expected)
        {
            //assign
            int[] variables = [.. variablesArray];
            Dictionary<int, int?> assigned = new Dictionary<int, int?>
            {
                { 1, 1 },
                { 2, null },
                { 3, null },
                
            };
            FilledAttendenceConstraint constraint = new FilledAttendenceConstraint(variables, minimalAssigned);

            //act
            bool result = constraint.IsSatisfied(assigned);

            //assert
            Assert.Equal(expected, result);
        }
    }
}
