
using App.Server.CSP;

namespace Tests.CSP.VariableManagerTests
{

    public class GetOrderedVariableTest
    {
        [Fact]
        public void EmptyVariables()
        {
            //assign
            List<Variable> variables = [];

            //act
            var ordered = VariableManager.GetOrderedVariables(variables);

            //assert
            Assert.NotNull(ordered);
            Assert.Empty(ordered);
        }

        [Fact]
        public void CorrectOrder()
        {
            //assign
            List<Variable> variables =
            [
                new Variable
                {
                    VariableId = 1,
                    RouteId = 1,
                    RouteType = RouteType.Daily,
                    DaysFromStart= 0
                },
                new Variable
                {
                    VariableId= 2,
                    RouteId = 2,
                    RouteType = RouteType.MaxOnce,
                    DaysFromStart= 0
                },
                new Variable
                {
                    VariableId= 3,
                    RouteId = 2,
                    RouteType = RouteType.Once,
                    DaysFromStart= 1
                },
                new Variable
                {
                    VariableId = 4,
                    RouteId = 1,
                    RouteType = RouteType.Daily,
                    DaysFromStart= 1
                },
                new Variable
                {
                    VariableId= 5,
                    RouteId = 2,
                    RouteType = RouteType.Once,
                    DaysFromStart= 0
                }
            ];

            //act
            var ordered = VariableManager.GetOrderedVariables(variables);

            //assert
            int[] expected = [1, 4, 5, 3, 2];
            Assert.NotNull(ordered);
            Assert.Equal(5, ordered.Count);
            Assert.Equal(expected, ordered.Select(var => var.VariableId));
        }
    }
}
