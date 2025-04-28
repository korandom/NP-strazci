using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using App.Server.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers.RangerControllerTests
{
    public class DeleteTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAppAuthenticationService> _mockAuthService;
        private readonly Mock<IGenericRepository<Ranger>> _mockRangerRepo;

        private readonly RangerController _controller;

        public DeleteTest()
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
            var result = await _controller.Delete(rangerId);

            // assert
            var badReq = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Ranger id not found.", badReq.Value);
        }

        [Fact]
        public async Task DeleteSucess()
        {
            // arrange
            var rangerId = 1;
            var ranger = new Ranger { Id = rangerId, DistrictId = 1, Email = "abc@gmail.com", FirstName = "a", LastName = "a" };
            _mockRangerRepo.Setup(r => r.GetById(rangerId)).ReturnsAsync(ranger);


            // act
            var result = await _controller.Delete(rangerId);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockRangerRepo.Verify(r => r.Delete(It.IsAny<Ranger>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
