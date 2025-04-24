using App.Server.Controllers;
using App.Server.CSP;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using App.Server.Services.Authentication;
using App.Server.Services.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers.RangerControllerTests
{
    public class CreateTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAppAuthenticationService> _mockAuthService;
        private readonly Mock<IGenericRepository<Ranger>> _mockRangerRepo;
        private readonly Mock<IGenericRepository<District>> _mockDistrictRepo;

        private readonly RangerController _controller;

        public CreateTest()
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
        public async Task DistrictDoesNotExist()
        {
            // arrange
            var districtId = 1;
            var rangerDto = new RangerDto { Id = 0, DistrictId = 1, Email = "abc@gmail.com", FirstName = "a", LastName = "a" };
            _mockDistrictRepo.Setup(r => r.GetById(districtId)).ReturnsAsync((District?)null);

            // act
            var result = await _controller.Create(rangerDto);

            // assert
            var badReq = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("District id not found.", badReq.Value);
        }

        [Fact] 
        public async Task CreateSucess()
        {
            // arrange
            var districtId = 1;
            var district = new District { Id = districtId, Name = "District" };
            var rangerDto = new RangerDto { Id = 0, DistrictId = 1, Email = "abc@gmail.com", FirstName = "a", LastName = "a" };
            _mockDistrictRepo.Setup(r => r.GetById(districtId)).ReturnsAsync(district);

            // act
            var result = await _controller.Create(rangerDto);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<RangerDto>(okResult.Value);
            _mockRangerRepo.Verify(r => r.Add(It.IsAny<Ranger>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
