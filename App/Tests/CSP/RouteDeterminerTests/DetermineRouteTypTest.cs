using App.Server.CSP;
using App.Server.DTOs;

namespace Tests.CSP.RouteDeterminerTests
{
    public class DetermineRouteTypTest
    {
        public static IEnumerable<object[]> GetRouteTypesForNoPlans()
        {
            yield return new object[] { 0, RouteType.Once};
            yield return new object[] { 1, RouteType.Once};
            yield return new object[] { 2, RouteType.MinOnce};
            yield return new object[] { 3, RouteType.Daily};

        }

        [Theory]
        [MemberData(nameof(GetRouteTypesForNoPlans))]
        public void NoPlansGiven(int priority, RouteType result)
        {
            //arrange
            var route = new RouteDto(1, "route", priority, null, 1);
            var routeDeterminer = new RouteDeterminer(new DateOnly(2025, 1, 1), []);

            //act
            var routeType = routeDeterminer.DetermineRouteType(route);

            //assert
            Assert.Equal(result, routeType);
        }

        public static IEnumerable<object[]> GetRouteInfo()
        {
            // monthly priority
            yield return new object[] { 0, new int[] { 5 }, RouteType.None };
            yield return new object[] { 0, new int[] { 0 }, RouteType.None };
            yield return new object[] { 0, new int[] { -7 }, RouteType.None };
            yield return new object[] { 0, new int[] { -8 }, RouteType.MaxOnce };
            yield return new object[] { 0, new int[] { -21 }, RouteType.MaxOnce };
            yield return new object[] { 0, new int[] { -22 }, RouteType.Once };
            yield return new object[] { 0, new int[] { -25 }, RouteType.Once };
            yield return new object[] { 0, new int[] { -25, 0 }, RouteType.None };
            // forthnight
            yield return new object[] { 1, new int[] { 0 }, RouteType.None };
            yield return new object[] { 1, new int[] { -7 }, RouteType.MaxOnce };
            yield return new object[] { 1, new int[] { -8 }, RouteType.Once };
            yield return new object[] { 1, new int[] { -20 }, RouteType.Once };
            yield return new object[] { 1, new int[] { -25, 0 }, RouteType.None };
            // weekly
            yield return new object[] { 2, new int[] { 0 }, RouteType.MaxOnce };
            yield return new object[] { 2, new int[] { -7 }, RouteType.MinOnce };
            yield return new object[] { 2, new int[] { -8 }, RouteType.MinOnce };
            yield return new object[] { 2, new int[] { -25, 0 }, RouteType.MaxOnce };
            //daily
            yield return new object[] { 3, new int[] { 0 }, RouteType.Daily };
            yield return new object[] { 3, new int[] { -6 }, RouteType.Daily };
            yield return new object[] { 3, new int[] { -7 }, RouteType.Daily};
            yield return new object[] { 3, new int[] { -20 }, RouteType.Daily };
            yield return new object[] { 3, new int[] { -25, 0 }, RouteType.Daily };
        }

        [Theory]
        [MemberData(nameof(GetRouteInfo))]
        public void RouteIsPlanned(int priority, int[] daysPlanned, RouteType result)
        {
            //arrange
            var start = new DateOnly(2025, 1, 1);
            var ranger = new RangerDto
            {
                Id = 1,
                FirstName = "Jan",
                LastName = "Novak",
                DistrictId = 1,
                Email = "abc@gmail"
            };
            var route = new RouteDto(1, "route", priority, null, 1);

            List<PlanDto> plans = [ new PlanDto(start, ranger, [2], []) ];
            foreach( var days in daysPlanned)
            {
                plans.Add(new PlanDto(start.AddDays(days), ranger, [1], []));
            }

            var routeDeterminer = new RouteDeterminer(start, plans);

            //act
            var routeType = routeDeterminer.DetermineRouteType(route);

            //assert
            Assert.Equal(result, routeType);
        }
    }
}
