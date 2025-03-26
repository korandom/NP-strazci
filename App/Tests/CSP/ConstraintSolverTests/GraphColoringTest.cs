

using App.Server.CSP;
using App.Server.CSP.Constraints;

using static Tests.CSP.ConstraintSolverTests.GraphColoringTest;

namespace Tests.CSP.ConstraintSolverTests
{
    public class GraphColoringTest
    {
        public class DifferentColorConstraint(int[] variables) : Constraint<int, string>(variables)
        {
            public override bool IsSatisfied(Dictionary<int, string> assigned)
            {
                var var1 = Variables[0];
                var var2 = Variables[1];
                if (assigned.TryGetValue(var1, out var value1) && assigned.TryGetValue(var2, out var value2))
                {
                    return value1 != value2;
                }
                return true;
            }
        }


        //   1-----2
        //   |
        //   |
        //   3

        [Fact]
        public void SimpleExample()
        {
            int[] nodes = [1, 2, 3];
            List<string> colors = ["Blue", "Red"];

            Dictionary<int, List<string>> domains = new Dictionary<int, List<string>>();
            foreach (int i in nodes)
            {
                domains[i] = new List<string>(colors);
            }

            List<Constraint<int, string>> constraintsList = new List<Constraint<int, string>>()
            {
                new DifferentColorConstraint([1,2]),
                new DifferentColorConstraint([3,1])
            };

            Dictionary<int, List<Constraint<int, string>>> constraints = Enumerable.Range(1, 3).ToDictionary(n => n, n => new List<Constraint<int, string>>());

            foreach (var constraint in constraintsList)
            {
                foreach (var node in constraint.Variables)
                {
                    constraints[node].Add(constraint);
                }
            }


            ConstraintSolver<int, string> solver = new ConstraintSolver<int, string>(nodes, domains, constraints);

            var assigned = solver.Solve();


            Assert.NotNull(assigned);
            Assert.NotEqual(assigned[1], assigned[2]);
            Assert.NotEqual(assigned[1], assigned[3]);
        }


        //   1-----2\
        //   |     | \
        //   |     |  \
        //   3-----4---5
        [Fact]
        public void UnusedUnprioritizedColor()
        {
            int[] nodes = [1, 2, 3, 4, 5];
            List<string> colors = ["Blue", "Red", "Green", "Yellow"];

            Dictionary<int, List<string>> domains = new Dictionary<int, List<string>>();
            foreach (int i in nodes)
            {
                domains[i] = new List<string>(colors);
            }

            List<Constraint<int, string>> constraintsList = new List<Constraint<int, string>>()
            {
                new DifferentColorConstraint([1,2]),
                new DifferentColorConstraint([3,1]),
                new DifferentColorConstraint([2,5]),
                new DifferentColorConstraint([2,4]),
                new DifferentColorConstraint([4,5]),
                new DifferentColorConstraint([3,4])
            };

            Dictionary<int, List<Constraint<int, string>>> constraints = Enumerable.Range(1, 5).ToDictionary(n => n, n => new List<Constraint<int, string>>());

            foreach (var constraint in constraintsList)
            {
                foreach (var node in constraint.Variables)
                {
                    constraints[node].Add(constraint);
                }
            }


            ConstraintSolver<int, string> solver = new ConstraintSolver<int, string>(nodes, domains, constraints.ToDictionary());

            var assigned = solver.Solve();


            Assert.NotNull(assigned);
            Assert.NotEqual(assigned[1], assigned[2]);
            Assert.NotEqual(assigned[1], assigned[3]);
            Assert.All(assigned.Values, value => Assert.NotEqual("Yellow", value));
        }
    }
}
