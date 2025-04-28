
using App.Server.Controllers;
using App.Server.CSP;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using App.Server.Services.Authentication;
using App.Server.Services.Authorization;


using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;

namespace Tests.Controllers.PlanControllerTests
{
    public class GenerateRoutePlanTest
    {

        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Plan>> _mockPlanRepo;
        private readonly Mock<IGenericRepository<Attendence>> _mockAttendenceRepo;
        private readonly Mock<IGenericRepository<Route>> _mockRouteRepo;
        private readonly Mock<IGenericRepository<Ranger>> _mockRangerRepo;
        private readonly Mock<IRoutePlanGenerator> _mockPlanRouteGenerator;
        private readonly PlanController _controller;

        public GenerateRoutePlanTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPlanRepo = new Mock<IGenericRepository<Plan>>();
            _mockAttendenceRepo = new Mock<IGenericRepository<Attendence>>();
            _mockRouteRepo = new Mock<IGenericRepository<Route>>();
            _mockRangerRepo = new Mock<IGenericRepository<Ranger>>();
            _mockPlanRouteGenerator = new Mock<IRoutePlanGenerator>();
            var authMock = new Mock<IAppAuthenticationService>();
            var authorMock = new Mock<IAppAuthorizationService>();

            _mockUnitOfWork.Setup(u => u.PlanRepository).Returns(_mockPlanRepo.Object);
            _mockUnitOfWork.Setup(u => u.AttendenceRepository).Returns(_mockAttendenceRepo.Object);
            _mockUnitOfWork.Setup(u => u.RouteRepository).Returns(_mockRouteRepo.Object);
            _mockUnitOfWork.Setup(u => u.RangerRepository).Returns(_mockRangerRepo.Object);

            _controller = new PlanController(_mockUnitOfWork.Object, authMock.Object, authorMock.Object, _mockPlanRouteGenerator.Object);
        }

        [Fact]
        public async Task GenerateRoutePlanSuccess()
        {
            // arrange
            var startDate = new DateOnly(2025, 4, 28);
            var districtId = 1;

            var ranger = new Ranger { Id = 1, DistrictId = districtId, Email = "abc@gmail.com", FirstName = "a", LastName = "b" };
            var plan = new Plan(startDate, ranger);
            var route = new Route { Id = 1, DistrictId = districtId };
            var attendence = new Attendence { Ranger = ranger, Date = startDate };

            _mockPlanRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Plan, bool>>>(), It.IsAny<string>())).ReturnsAsync(new List<Plan> { plan });

            _mockAttendenceRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Attendence, bool>>>(), It.IsAny<string>())).ReturnsAsync([attendence]);

            _mockRouteRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Route, bool>>>(), "")).ReturnsAsync([route]);

            _mockRangerRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Ranger, bool>>>(), "")).ReturnsAsync([ranger]);

            _mockPlanRouteGenerator.Setup(g => g.Generate(
                It.IsAny<List<PlanDto>>(),          // previousPlans
                It.IsAny<List<PlanDto>>(),          // preexistingPlans
                It.IsAny<List<AttendenceDto>>(),    // attendences
                It.IsAny<List<RouteDto>>(),         // routes
                It.IsAny<List<RangerDto>>(),        // rangers
                It.IsAny<DateOnly>()))              // startDate
            .Returns(new GenerateResultDto
            {
                Plans = new List<PlanDto>
                {
                   new PlanDto(new DateOnly(2025,1,1), new RangerDto{Id= 1}, [3], [])
                },
                Success = true
            });

            // act
            var result = await _controller.GenerateRoutePlan(districtId, startDate);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<GenerateResultDto>(okResult.Value);
        }

        [Theory]
        [InlineData("preexisting")]
        [InlineData("previous")]
        [InlineData("attendence")]
        [InlineData("routes")]
        [InlineData("rangers")]
        public async Task UnableToRetrieveSomething(string failPoint)
        {
            // arrange
            var startDate = new DateOnly(2025, 4, 28);
            var districtId = 1;

            SetupMocksToFailOn(failPoint);

            // act
            var result = await _controller.GenerateRoutePlan(districtId, startDate);

            // assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("not successful", notFound.Value!.ToString()!.ToLower());

        }

        private void SetupMocksToFailOn(string failPoint)
        {
            _mockPlanRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Plan, bool>>>(), It.IsAny<string>()))
                         .ReturnsAsync(failPoint is "preexisting" or "previous" ? null! : new List<Plan> { new Plan(new DateOnly(2025, 4, 28), new Ranger()) });

            _mockAttendenceRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Attendence, bool>>>(), It.IsAny<string>()))
                               .ReturnsAsync(failPoint == "attendence" ? null! : new List<Attendence> { new Attendence { Date = new DateOnly(2025, 4, 28), Ranger = new Ranger() } });

            _mockRouteRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Route, bool>>>(), ""))
                          .ReturnsAsync(failPoint == "routes" ? null! : new List<Route> { new Route { Id = 1 } });

            _mockRangerRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Ranger, bool>>>(), ""))
                           .ReturnsAsync(failPoint == "rangers" ? null! : new List<Ranger> { new Ranger { Id = 1 } });
        }
    }
}
