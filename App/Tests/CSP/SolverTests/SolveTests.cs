using App.Server.CSP;
using App.Server.CSP.Constraints;

namespace Tests.CSP.SolverTests
{
    public class SolveTests
    {
        [Fact]
        public void Solve()
        {
            // arrange
            var variables = new List<Variable>
            {
                new() { RouteId = 1, DaysFromStart = 1, RouteType = RouteType.Once, VariableId = 1},
                new() { RouteId = 1, DaysFromStart = 2, RouteType = RouteType.Once, VariableId = 2},
                new() { RouteId = 2, DaysFromStart = 1, RouteType = RouteType.Once, VariableId = 3}
            };

            var domainsForDays = new Dictionary<int, List<int>>
            {
                { 1, new List<int> { 1, 2 } },
                { 2, new List<int> { 2, 3 } }
            };

            var domainsForRoutes = new Dictionary<int, List<int>>
            {
                { 1, new List<int> { 1, 2 } },
                { 2, new List<int> { 1, 2 } }
            };

            var constraints = new Dictionary<int, List<Constraint<int, int?>>>
            {
                { 1, new List<Constraint<int, int?>> { new SomeAssignedConstraint([1,2]) } },
                { 2, new List<Constraint<int, int?>> { new SomeAssignedConstraint([1,2]) } },
                { 3, new List<Constraint<int, int?>> { new SomeAssignedConstraint([3]) } },
            };

            var solver = new Solver(variables, domainsForDays, domainsForRoutes, constraints);

            // act
            var result = solver.Solve();

            // assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(1));
            Assert.True(result.ContainsKey(2));
            Assert.True(result.ContainsKey(3));

            Assert.True(result[1] != null || result[2] != null); // some assigned
        }

        [Fact]
        public void NoSolution()
        {
            // arrange
            var variables = new List<Variable>
            {
                new() { RouteId = 1, DaysFromStart = 1, RouteType = RouteType.Daily, VariableId = 1},
                new() { RouteId = 2, DaysFromStart = 1, RouteType = RouteType.Daily, VariableId = 2}

            };

            var domainsForDays = new Dictionary<int, List<int>>
            {
                { 1, new List<int> { 1 } }
            };

            var domainsForRoutes = new Dictionary<int, List<int>>
            {
                { 1, new List<int> { 1 } },
                { 2, new List<int> { 1 } }
            };

            var constraints = new Dictionary<int, List<Constraint<int, int?>>>();

            var solver = new Solver(variables, domainsForDays, domainsForRoutes, constraints);

            // act
            var result = solver.Solve();

            // assert
            Assert.Null(result); // No valid assignment should exist
        }
    }
}
