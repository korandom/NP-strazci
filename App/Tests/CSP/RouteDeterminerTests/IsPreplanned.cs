using App.Server.CSP;
using App.Server.DTOs;

namespace Tests.CSP.RouteDeterminerTests
{
    public class IsPreplanned
    {
        [Fact]
        public void NoPlansGiven()
        {
            //arrange
            var routeDeterminer = new RouteDeterminer(new DateOnly(2025, 1, 1), []);

            //act
            var isPreplanned = routeDeterminer.IsPreplanned(1, 0);

            //assert
            Assert.False(isPreplanned);
        }

        // One plan exists on the correct day, testing correct detection
        [Theory]
        [InlineData(1, new int[] { 1 }, true)]
        [InlineData(1, new int[] { 1, 2 }, true)]
        [InlineData(1, new int[] { 2 }, false)]
        [InlineData(1, new int[] { }, false)]

        public void OnePlanExists(int routeId, int[] plannedRoutes, bool result)
        {
            //arrange
            var date = new DateOnly(2025, 1, 1);
            var ranger = new RangerDto
            {
                Id = 1,
                FirstName = "Jan",
                LastName = "Novak",
                DistrictId = 1,
                Email = "abc@gmail"
            };

            List<PlanDto> plans = [
                new PlanDto(date, ranger, plannedRoutes, [])
            ];
            var routeDeterminer = new RouteDeterminer(date, plans);

            //act
            var isPreplanned = routeDeterminer.IsPreplanned(routeId, 0);

            //assert
            Assert.Equal(result, isPreplanned);
        }


        // One plan exists on the correct day, testing correct detection
        [Theory]
        [InlineData(1, 1, true)]
        // multiple plans with planned route 2, but none on day 0
        [InlineData(2, 0, false)]
        // none route 3 planned
        [InlineData(3, 2, false)]
        // no routes planned on day 5
        [InlineData(2, 5, false)]

        public void MorePlansExists(int routeId, int dayNumber, bool result)
        {
            //arrange
            var date = new DateOnly(2025, 1, 1);
            var ranger = new RangerDto
            {
                Id = 1,
                FirstName = "Jan",
                LastName = "Novak",
                DistrictId = 1,
                Email = "abc@gmail"
            };

            List<PlanDto> plans =
            [
                new PlanDto
                (
                    date.AddDays(0),
                    ranger,
                    [1],
                    [2]
                ),
                new PlanDto
                (
                    date.AddDays(1),
                    ranger,
                    [1,2],
                    [2]
                ),

                new PlanDto
                (
                    date.AddDays(2),
                    ranger,
                    [2],
                    []
                ),
                new PlanDto
                (
                    date.AddDays(3),
                    ranger,
                    [2],
                    []
                ),
                new PlanDto
                (
                    date.AddDays(4),
                    ranger,
                    [2],
                    []
                )
            ];
            var routeDeterminer = new RouteDeterminer(date, plans);

            //act
            var isPreplanned = routeDeterminer.IsPreplanned(routeId, dayNumber);

            //assert
            Assert.Equal(result, isPreplanned);
        }

    }
}
