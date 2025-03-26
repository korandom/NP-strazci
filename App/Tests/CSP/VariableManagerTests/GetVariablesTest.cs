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
        }
        [Fact]
        public void NoRoutes()
        {
            //assign
            VariableManager variableManager = new(_mockDeterminer.Object, new List<RouteDto>(), 2);

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
            //assign
            List<RouteDto> routes = new List<RouteDto>();
            for (int i = 0; i < routeCount; i++)
            {
                routes.Add(new RouteDto() { Id = i, Priority = 0, DistrictId = 1 });
            }
            VariableManager variableManager = new(_mockDeterminer.Object, routes, dayCount);

            //act
            List<Variable> variables = variableManager.GetVariables();

            //assert
            Assert.NotNull(variables);
            Assert.Equal(routeCount * dayCount, variables.Count);
        }
    }
}
