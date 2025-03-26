
using App.Server.DTOs;

namespace Tests.CSP.DataProcessor
{
    public class GetBestRangersForRoutesTest
    {
        App.Server.CSP.DataProcessor dataProcessor;
        public GetBestRangersForRoutesTest()
        {
            DateOnly start = new DateOnly(2025, 1, 1);
            List<RangerDto> rangers = new List<RangerDto>
            {
                new RangerDto
                {
                    Id = 1,
                    FirstName = "Jan",
                    LastName = "Novak",
                    DistrictId = 1,
                    Email = "abc@gmail"
                },
                new RangerDto
                {
                    Id = 2,
                    FirstName = "Pavel",
                    LastName = "Novak",
                    DistrictId = 1,
                    Email = "abc@gmail"
                },
                new RangerDto
                {
                    Id = 3,
                    FirstName = "Jakub",
                    LastName = "Novak",
                    DistrictId = 1,
                    Email = "abc@gmail"
                }
            };
            List<RouteDto> routes = new List<RouteDto>
            {
                new RouteDto
                {
                    Id= 1,
                    Name = "route1",
                    DistrictId= 1,
                    Priority = 1,
                    ControlPlace= null
                },
                new RouteDto
                {
                    Id= 2,
                    Name = "route2",
                    DistrictId= 1,
                    Priority = 2,
                    ControlPlace= null
                }
            };

            dataProcessor = new App.Server.CSP.DataProcessor(new List<PlanDto>(), rangers, routes, start);

        }
        [Fact]
        public void NoDataTest()
        {
            //assign
            var dataProcessorNoData = new App.Server.CSP.DataProcessor(new List<PlanDto>(), new List<RangerDto>(), new List<RouteDto>(), new DateOnly(2025, 1, 1));

            //act
            var bestRangers = dataProcessor.GetBestRangersForRoutes(new Dictionary<int, Dictionary<int, int>>());

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
            var bestRangers = dataProcessor.GetBestRangersForRoutes(distributions);

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
            var bestRangers = dataProcessor.GetBestRangersForRoutes(distributions);

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