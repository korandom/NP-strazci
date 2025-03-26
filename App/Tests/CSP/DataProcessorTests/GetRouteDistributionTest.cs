using App.Server.CSP;
using App.Server.DTOs;

namespace Tests.CSP.DataProcessor
{
    public class GetRouteDistributions
    {
        [Fact]
        public void NoDataTest()
        {
            //assign
            var dataProcessor = new App.Server.CSP.DataProcessor(new List<PlanDto>(), new List<RangerDto>(), new List<RouteDto>(), new DateOnly(2025, 1, 1));
            
            //act
            var distributions = dataProcessor.GetRouteDistributions();

            //assert
            Assert.NotNull(distributions);
            Assert.Empty(distributions);
        }

        [Fact]
        public void SimpleExistenceTest()
        {
            //assign
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
            
            var dataProcessor = new App.Server.CSP.DataProcessor(new List<PlanDto>(), rangers, routes, start);
            
            //act
            var distributions = dataProcessor.GetRouteDistributions();

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
            //assign
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
            // route1 1 times, route2 4 times
            List<PlanDto> previousPlans = new List<PlanDto>
            {
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
            };
            var dataProcessor = new App.Server.CSP.DataProcessor(previousPlans, rangers, routes, new DateOnly(2025, 1, 1));

            //act
            var distributions = dataProcessor.GetRouteDistributions();

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