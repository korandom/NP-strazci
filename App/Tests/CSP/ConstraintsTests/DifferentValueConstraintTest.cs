using App.Server.CSP.Constraints;


namespace Tests.CSP.ConstraintsTests
{
    public class DifferentValueConstraintTest
    {
        // always returns true, wrong use
        [Fact]
        public void OnlyOneVariable()
        {
            //assign
            int[] variables = [1];
            Dictionary<int, int?> assigned =  new Dictionary<int, int?>
            {
                { 1, 1 },
                { 2, 1 },
                { 3, 1 }
            };
            DifferentValueConstraint constraint = new DifferentValueConstraint(variables);

            //act
            bool result = constraint.IsSatisfied(assigned);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void TwoVariablesBothNull()
        {
            //assign
            int[] variables = [1,2];
            Dictionary<int, int?> assigned = new Dictionary<int, int?>
            {
                { 1, null },
                { 2, null },
                { 3, null }
            };
            DifferentValueConstraint constraint = new DifferentValueConstraint(variables);

            //act
            bool result = constraint.IsSatisfied(assigned);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void TwoVariablesDifferent()
        {
            //assign
            int[] variables = [1, 2];
            Dictionary<int, int?> assigned = new Dictionary<int, int?>
            {
                { 1, 1 },
                { 2, 2 },
                { 3, null }
            };
            DifferentValueConstraint constraint = new DifferentValueConstraint(variables);

            //act
            bool result = constraint.IsSatisfied(assigned);

            //assert
            Assert.True(result);
        }


        [Fact]
        public void TwoVariablesSame()
        {
            //assign
            int[] variables = [1, 2];
            Dictionary<int, int?> assigned = new Dictionary<int, int?>
            {
                { 1, 1 },
                { 2, 1 },
                { 3, 2 }
            };
            DifferentValueConstraint constraint = new DifferentValueConstraint(variables);

            //act
            bool result = constraint.IsSatisfied(assigned);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void MultipleVariablesDifferent()
        {
            //assign
            int[] variables = [1, 2, 3, 4];
            Dictionary<int, int?> assigned = new Dictionary<int, int?>
            {
                { 1, 1 },
                { 2, null },
                { 3, 2 },
                { 4, 3 },
                { 5, 1 }
            };
            DifferentValueConstraint constraint = new DifferentValueConstraint(variables);

            //act
            bool result = constraint.IsSatisfied(assigned);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void MultipleVariablesNoneAssigned()
        {
            //assign
            int[] variables = [1, 2, 3, 4];
            Dictionary<int, int?> assigned = new Dictionary<int, int?>
            {
                { 5, 1 },
                { 6, null },
                { 7, 2 },
                { 8, 1 }
            };
            DifferentValueConstraint constraint = new DifferentValueConstraint(variables);

            //act
            bool result = constraint.IsSatisfied(assigned);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void MultipleVariablesSomeSame()
        {
            //assign
            int[] variables = [1, 2, 3, 4];
            Dictionary<int, int?> assigned = new Dictionary<int, int?>
            {
                { 1, 1 },
                { 2, null },
                { 3, 1 },
                { 4, 3 },
                { 5, 3 }
            };
            DifferentValueConstraint constraint = new DifferentValueConstraint(variables);

            //act
            bool result = constraint.IsSatisfied(assigned);

            //assert
            Assert.False(result);
        }
    }
}
