using App.Server.CSP;
using App.Server.DTOs;

namespace Tests.CSP.DataProcessorTests
{
    public class GetRouteDistributions
    {
        [Fact]
        public void NoDataTest()
        {
            //act
            var distributions = DataProcessor.GetRouteDistributions([], [], []);

            //assert
            Assert.NotNull(distributions);
            Assert.Empty(distributions);
        }

        [Fact]
        public void SimpleExistenceTest()
        {
            //arrange
            List<RangerDto> rangers =
            [
                new RangerDto
                {
                    Id = 1,
                    FirstName = "Jan",
                    LastName = "Novak",
                    DistrictId = 1,
                    Email = "abc@gmail"
                }
            ];
            List<RouteDto> routes =
            [
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
            ];


            //act
            var distributions = DataProcessor.GetRouteDistributions([], rangers, routes);

            //assert
            Assert.NotNull(distributions);
            Assert.Contains(1, distributions);
            Assert.Single(distributions);
            Assert.Equal(2, distributions[1].Count);



            // since no plans were given, distribution for both routes should be 0
            Assert.Equal(0, distributions[1][1]);
            Assert.Equal(0, distributions[1][2]);
        }

        [Fact]
        public void CorrectDistributionTest()
        {
            //arrange
            DateOnly start = new(2025, 1, 1);
            List<RangerDto> rangers =
            [
                new RangerDto
                {
                    Id = 1,
                    FirstName = "Jan",
                    LastName = "Novak",
                    DistrictId = 1,
                    Email = "abc@gmail"
                }
            ];
            List<RouteDto> routes =
            [
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
            ];
            // route1 1 times, route2 4 times
            List<PlanDto> plans =
            [
                new PlanDto
                (
                    start.AddDays(-2),
                    rangers[0],
                    [1,2],
                    []
                ),

                new PlanDto
                (
                    start.AddDays(-5),
                    rangers[0],
                    [2],
                    []
                ),
                new PlanDto
                (
                    start.AddDays(-7),
                    rangers[0],
                    [2],
                    []
                ),
                new PlanDto
                (
                    start.AddDays(-3),
                    rangers[0],
                    [2],
                    []
                )
            ];

            //act
            var distributions = DataProcessor.GetRouteDistributions(plans, rangers, routes);

            //assert
            Assert.NotNull(distributions);
            Assert.Contains(1, distributions);
            Assert.Single(distributions);
            Assert.Equal(2, distributions[1].Count);



            // since no plans were given, distribution for both routes should be 0
            Assert.Equal(200, distributions[1][1]);
            Assert.Equal(800, distributions[1][2]);
        }
    }
}