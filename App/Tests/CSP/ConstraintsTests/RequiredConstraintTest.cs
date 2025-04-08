using App.Server.CSP.Constraints;


namespace Tests.CSP.ConstraintsTests
{
    public class RequiredConstraintTest
    {
        // Varible one is assigned a non null value, two is assigned null
        [Theory]
        [InlineData(1, true)]
        [InlineData(2, false)]
        // since 3 is not assigned yet, still could become assigned a value later
        [InlineData(3, true)]

        public void SimpleScenarios(int variable, bool expected)
        {

            Dictionary<int, int?> assigned = new()
            {
                { 1, 1 },
                { 2, null }

            };
            RequiredConstraint constraint = new(variable);

            //act
            bool result = constraint.IsSatisfied(assigned);

            //assert
            Assert.Equal(expected, result);
        }
    }
}
