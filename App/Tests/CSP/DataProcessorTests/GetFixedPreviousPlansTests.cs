using App.Server.CSP;
using App.Server.DTOs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.CSP.DataProcessorTests
{
    public class GetFixedPreviousPlansTests
    {
        [Fact]
        public void NoDataTest()
        {
            //act
            var fixedPlans = DataProcessor.GetFixedPreviousPlans([]);

            //assert
            Assert.NotNull(fixedPlans);
            Assert.Empty(fixedPlans);
        }

        [Fact]
        public void NoFixed()
        {
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

            List<PlanDto> plans =
            [
                new PlanDto
                (
                    start.AddDays(-2),
                    rangers[0],
                    [1],
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
            var fixedPlans = DataProcessor.GetFixedPreviousPlans(plans);

            //assert
            Assert.NotNull(fixedPlans);
            Assert.Empty(fixedPlans);
        }

        [Fact]
        public void MoreThanOneRouteInPlan()
        {
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
                    [1,2,3],
                    []
                )
            ];
            //act
            var fixedPlans = DataProcessor.GetFixedPreviousPlans(plans);

            //assert
            Assert.NotNull(fixedPlans);
            Assert.Equal(2, fixedPlans.Count);
        }

        [Fact]
        public void RouteTwiceInDayTest()
        {
            // arrange
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
                },
                new RangerDto
                {
                    Id = 2,
                    FirstName = "Jan",
                    LastName = "Babel",
                    DistrictId = 1,
                    Email = "abdc@gmail"
                }
            ];

            List<PlanDto> plans =
            [
                new PlanDto
                (
                    start.AddDays(-2),
                    rangers[0],
                    [1, 2],
                    []
                ),

                new PlanDto
                (
                    start.AddDays(-2),
                    rangers[1],
                    [1],
                    []
                )
            ];
            //act
            var fixedPlans = DataProcessor.GetFixedPreviousPlans(plans);

            //assert
            Assert.NotNull(fixedPlans);
            Assert.Equal(2, fixedPlans.Count);
        }
    }
}
