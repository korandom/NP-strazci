

using App.Server.CSP.Constraints;
namespace Tests.CSP.ConstraintsTests
{
    public class SomeAssignedConstrantTest
    {
        // Variable one is assigned, two and three are set to null 
        [Theory]
        [InlineData(new int[] { 1, 2, 3 }, true)]
        [InlineData(new int[] { 1, 4 }, true)]
        [InlineData(new int[] { 2, 3 }, false)]
        [InlineData(new int[] { 4, 5, 6 }, true)]
        public void SimpleScenarios(int[] variablesArray, bool expected)
        {
            //assign
            int[] variables = [.. variablesArray];
            Dictionary<int, int?> assigned = new Dictionary<int, int?>
            {
                { 1, 1 },
                { 2, null },
                { 3, null },

            };
            SomeAssignedConstraint constraint = new SomeAssignedConstraint(variables);

            //act
            bool result = constraint.IsSatisfied(assigned);

            //assert
            Assert.Equal(expected, result);
        }
    }
}
