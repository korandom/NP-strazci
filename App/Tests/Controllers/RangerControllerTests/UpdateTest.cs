using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using App.Server.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers.RangerControllerTests
{
    public class UpdateTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAppAuthenticationService> _mockAuthService;
        private readonly Mock<IGenericRepository<Ranger>> _mockRangerRepo;

        private readonly RangerController _controller;

        public UpdateTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthService = new Mock<IAppAuthenticationService>();
            _mockRangerRepo = new Mock<IGenericRepository<Ranger>>();
            _mockUnitOfWork.Setup(u => u.RangerRepository).Returns(_mockRangerRepo.Object);

            _controller = new RangerController(
                _mockUnitOfWork.Object,
                _mockAuthService.Object
            );
        }

        [Fact]
        public async Task RangerDoesNotExist()
        {
            // arrange
            var rangerId = 1;
            var rangerDto = new RangerDto { Id = rangerId, DistrictId = 1, Email = "abc@gmail.com", FirstName = "a", LastName = "a" };
            _mockRangerRepo.Setup(r => r.GetById(rangerId)).ReturnsAsync((Ranger?)null);

            // act
            var result = await _controller.Update(rangerDto);

            // assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Ranger not found.", notFound.Value);
        }

        [Fact]
        public async Task UpdateSucess()
        {
            // arrange
            var rangerId = 1;
            var ranger = new Ranger { Id = rangerId, DistrictId = 1, Email = "abc@gmail.com", FirstName = "a", LastName = "a" };
            var rangerDto = new RangerDto { Id = rangerId, DistrictId = 1, Email = "abc@gmail.com", FirstName = "b", LastName = "b" };
            _mockRangerRepo.Setup(r => r.GetById(rangerId)).ReturnsAsync(ranger);


            // act
            var result = await _controller.Update(rangerDto);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<RangerDto>(okResult.Value);
            _mockRangerRepo.Verify(r => r.Update(It.IsAny<Ranger>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}