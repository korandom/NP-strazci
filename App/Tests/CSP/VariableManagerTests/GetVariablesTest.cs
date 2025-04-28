using App.Server.CSP;
using App.Server.DTOs;
using Moq;

namespace Tests.CSP.VariableManagerTests
{
    public class GetVariablesTest
    {
        private readonly Mock<IRouteTypeDeterminer> _mockDeterminer;
        public GetVariablesTest()
        {
            _mockDeterminer = new Mock<IRouteTypeDeterminer>();
            _mockDeterminer.Setup(d => d.DetermineRouteType(It.IsAny<RouteDto>()))
              .Returns(RouteType.Once);
            _mockDeterminer.Setup(d => d.IsPreplanned(It.IsAny<int>(), It.IsAny<int>())).Returns(false);
        }
        [Fact]
        public void NoRoutes()
        {
            //arrange
            VariableManager variableManager = new(_mockDeterminer.Object, [], 2);

            //act
            List<Variable> variables = variableManager.GetVariables();

            //assert
            Assert.NotNull(variables);
            Assert.Empty(variables);
        }


        [Theory]
        [InlineData(1, 7)]
        [InlineData(12, 7)]
        [InlineData(15, 12)]

        public void CorrectVariableCount(int routeCount, int dayCount)
        {
            //arrange
            List<RouteDto> routes = [];
            for (int i = 0; i < routeCount; i++)
            {
                routes.Add(new RouteDto(i, "a", 0, null, 1));
            }
            VariableManager variableManager = new(_mockDeterminer.Object, routes, dayCount);

            //act
            List<Variable> variables = variableManager.GetVariables();

            //assert
            Assert.NotNull(variables);
            Assert.Equal(routeCount * dayCount, variables.Count);
        }

        [Fact]
        public void FilterPreplannedVariables()
        {
            //arrange
            List<RouteDto> routes = [new RouteDto(1, "a", 0, null, 1)];
            _mockDeterminer.Setup(d => d.IsPreplanned(1, 1)).Returns(true);
            VariableManager variableManager = new(_mockDeterminer.Object, routes, 5);

            //act
            List<Variable> variables = variableManager.GetVariables();

            //assert
            Assert.NotNull(variables);
            Assert.Equal(4, variables.Count);
        }

        [Fact]
        public void FilterNoneRoutes()
        {
            //arrange
            List<RouteDto> routes = [new RouteDto(1, "a", 0, null, 1)];
            _mockDeterminer.Setup(d => d.DetermineRouteType(It.IsAny<RouteDto>()))
              .Returns(RouteType.None);
            VariableManager variableManager = new(_mockDeterminer.Object, routes, 5);

            //act
            List<Variable> variables = variableManager.GetVariables();

            //assert
            Assert.NotNull(variables);
            Assert.Empty(variables);
        }
    }
}
