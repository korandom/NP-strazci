
using App.Server.CSP;
using App.Server.DTOs;
using System.Collections.Generic;

namespace Tests.CSP.DataProcessorTests
{
    public class GetBestRangersForRoutesTest
    {
        readonly List<RouteDto> routes;
        public GetBestRangersForRoutesTest()
        {
            routes =
            [
                new RouteDto(1, "route1", 1, null, 1),
                new RouteDto(2, "route2", 2, null, 1)
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