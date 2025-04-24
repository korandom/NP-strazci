using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using App.Server.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;

namespace Tests.Controllers.RangerControllerTests
{
    public class GetRangersInDistrictTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAppAuthenticationService> _mockAuthService;
        private readonly Mock<IGenericRepository<Ranger>> _mockRangerRepo;
        private readonly Mock<IGenericRepository<District>> _mockDistrictRepo;

        private readonly RangerController _controller;

        public GetRangersInDistrictTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthService = new Mock<IAppAuthenticationService>();
            _mockRangerRepo = new Mock<IGenericRepository<Ranger>>();
            _mockDistrictRepo = new Mock<IGenericRepository<District>>();
            _mockUnitOfWork.Setup(u => u.RangerRepository).Returns(_mockRangerRepo.Object);
            _mockUnitOfWork.Setup(u => u.DistrictRepository).Returns(_mockDistrictRepo.Object);


            _controller = new RangerController(
                _mockUnitOfWork.Object,
                _mockAuthService.Object
            );
        }

        [Fact]
        public async Task FailedToGetRangers()
        {
            // arrange
            var districtId = 1;
            _mockRangerRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Ranger, bool>>>(),"")).ReturnsAsync((IEnumerable<Ranger>?)null!);

            // act
            var result = await _controller.GetRangersInDistrict(districtId);

            // assert
            var badReq = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Failed to fetch rangers.", badReq.Value);
        }

        [Fact]
        public async Task GetRangersSuccess()
        {
            // arrange
            var districtId = 1;
            var ranger = new Ranger { Id = 0, DistrictId = 1, Email = "abc@gmail.com", FirstName = "a", LastName = "a" };
            _mockRangerRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Ranger, bool>>>(), "")).ReturnsAsync([ranger]);

            // act
            var result = await _controller.GetRangersInDistrict(districtId);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRangers = okResult.Value as List<RangerDto>;
            Assert.NotNull(returnedRangers);
            Assert.Single(returnedRangers);
        }
    }
}
