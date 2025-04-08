
using App.Server.CSP;
using App.Server.DTOs;

namespace Tests.CSP.DataProcessorTests
{
    public class GetBestRangersForRoutesTest
    {
        readonly List<RouteDto> routes;
        public GetBestRangersForRoutesTest()
        {
            routes =
            [
                new() {
                    Id= 1,
                    Name = "route1",
                    DistrictId= 1,
                    Priority = 1,
                    ControlPlace= null
                },
                new() {
                    Id= 2,
                    Name = "route2",
                    DistrictId= 1,
                    Priority = 2,
                    ControlPlace= null
                }
            ];
        }

        [Fact]
        public void NoDataTest()
        {
            //act
            var bestRangers = DataProcessor.GetBestRangersForRoutes([], routes, []);

            //assert
            Assert.NotNull(bestRangers);
            Assert.Contains(1, bestRangers);
            Assert.Contains(2, bestRangers);
            Assert.Empty(bestRangers[1]);
            Assert.Empty(bestRangers[2]);

        }

        [Fact]
        public void SimpleExistenceTest()
        {

            //assign
            var distributions = new Dictionary<int, Dictionary<int, int>>
            {
                [1] = new Dictionary<int, int>
                {
                    { 1, 1000 }, {2, 0}
                }
            };

            //act
            var bestRangers = DataProcessor.GetBestRangersForRoutes(distributions, routes, []);

            //assert
            Assert.NotNull(bestRangers);
            Assert.Contains(1, bestRangers);
            Assert.Contains(2, bestRangers);
            Assert.Single(bestRangers[1]);
            Assert.Single(bestRangers[2]);
            Assert.Equal(1, bestRangers[1][0]);
        }

        [Fact]
        public void CorrectOrderOfRangersTest()
        {
            //assign
            var distributions = new Dictionary<int, Dictionary<int, int>>
            {
                [1] = new Dictionary<int, int>
                {
                    { 1, 1000 }, {2, 0}
                },
                [2] = new Dictionary<int, int>
                {
                    { 1, 0 }, {2, 1000}
                },
                [3] = new Dictionary<int, int>
                {
                    { 1, 500 }, {2, 500}
                }
            };
            //act
            var bestRangers = DataProcessor.GetBestRangersForRoutes(distributions, routes, []);

            //assert
            Assert.NotNull(bestRangers);
            Assert.Contains(1, bestRangers);
            Assert.Contains(2, bestRangers);

            // route 1 
            Assert.Equal(2, bestRangers[1][0]);
            Assert.Equal(3, bestRangers[1][1]);
            Assert.Equal(1, bestRangers[1][2]);


            //route 2
            Assert.Equal(1, bestRangers[2][0]);
            Assert.Equal(3, bestRangers[2][1]);
            Assert.Equal(2, bestRangers[2][2]);
        }
    }
}