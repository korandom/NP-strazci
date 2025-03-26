
using App.Server.CSP;
using Xunit.Abstractions;

namespace Tests.CSP.VariableManagerTests
{
    public class GetOrderedVariableIdsTest
    {
        [Fact]
        public void EmptyVariables()
        {
            //assign
            List<Variable> variables = [];

            //act
            int[] varIDs = App.Server.CSP.VariableManager.GetOrderedVariableIds(variables);

            //assert
            Assert.NotNull(varIDs);
            Assert.Empty(varIDs);
        }

        [Fact]
        public void CorrectOrderByDays()
        {
            //assign
            List<Variable> variables = new List<Variable>()
            {
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
                    RouteType = RouteType.Daily,
                    DaysFromStart= 2
                },
                new Variable
                {
                    VariableId= 3,
                    RouteId = 2,
                    RouteType = RouteType.Daily,
                    DaysFromStart= 1
                }
            };

            //act
            int[] varIDs = App.Server.CSP.VariableManager.GetOrderedVariableIds(variables);

            //assert
            int[] expected = [1,3,2];
            Assert.NotNull(varIDs);
            Assert.Equal(3, varIDs.Length);
            Assert.Equal(expected, varIDs);
        }

        [Fact]
        public void CorrectOrderByRouteType()
        {
            //assign
            List<Variable> variables = new List<Variable>()
            {
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
                    RouteType = RouteType.Meh,
                    DaysFromStart= 0
                },
                new Variable
                {
                    VariableId= 3,
                    RouteId = 2,
                    RouteType = RouteType.Once,
                    DaysFromStart= 0
                }
            };

            //act
            int[] varIDs = App.Server.CSP.VariableManager.GetOrderedVariableIds(variables);

            //assert
            int[] expected = [1, 3, 2];
            Assert.NotNull(varIDs);
            Assert.Equal(3, varIDs.Length);
            Assert.Equal(expected, varIDs);
        }

        [Fact]
        public void CorrectOrderByBoth()
        {
            //assign
            List<Variable> variables = new List<Variable>()
            {
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
                    RouteType = RouteType.Meh,
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
            };

            //act
            int[] varIDs = App.Server.CSP.VariableManager.GetOrderedVariableIds(variables);

            //assert
            int[] expected = [1, 5, 2, 4, 3];
            Assert.NotNull(varIDs);
            Assert.Equal(5, varIDs.Length);
            Assert.Equal(expected, varIDs);
        }

        // Variables with the same routeType and Day - should not always be the same
        [Fact]
        public void RandomSubOrder()
        {
            //assign
            List<Variable> variables = new List<Variable>()
            {
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
                    RouteType = RouteType.Daily,
                    DaysFromStart= 0
                },
                new Variable
                {
                    VariableId= 3,
                    RouteId = 3,
                    RouteType = RouteType.Daily,
                    DaysFromStart= 0
                },
                new Variable
                {
                    VariableId = 4,
                    RouteId = 4,
                    RouteType = RouteType.Daily,
                    DaysFromStart= 0
                },
                new Variable
                {
                    VariableId= 5,
                    RouteId = 5,
                    RouteType = RouteType.Daily,
                    DaysFromStart= 0
                },
                new Variable
                {
                    VariableId = 6,
                    RouteId = 1,
                    RouteType = RouteType.Daily,
                    DaysFromStart= 1
                },
                new Variable
                {
                    VariableId = 7,
                    RouteId = 2,
                    RouteType = RouteType.Daily,
                    DaysFromStart= 1
                },
                new Variable
                {
                    VariableId = 8,
                    RouteId = 3,
                    RouteType = RouteType.Daily,
                    DaysFromStart= 1
                },
                new Variable
                {
                    VariableId = 9,
                    RouteId = 4,
                    RouteType = RouteType.Daily,
                    DaysFromStart= 1
                },
                new Variable
                {
                    VariableId = 10,
                    RouteId = 5,
                    RouteType = RouteType.Daily,
                    DaysFromStart= 1
                },
            };

            //act
            int[] varIDs1 = App.Server.CSP.VariableManager.GetOrderedVariableIds(variables);
            int[] varIDs2 = App.Server.CSP.VariableManager.GetOrderedVariableIds(variables);

            // just some logging for clarity
            for (int i = 0; i < 10; i++)
            {
                int[] varID = App.Server.CSP.VariableManager.GetOrderedVariableIds(variables);
                _output.WriteLine(string.Join(',', varID));
            }

            //assert
            Assert.NotEqual(varIDs1, varIDs2);
        }

        private readonly ITestOutputHelper _output;

        public GetOrderedVariableIdsTest(ITestOutputHelper output)
        {
            _output = output;
        }
    }
}
