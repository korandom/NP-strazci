using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Models.Identity;
using App.Server.Repositories.Interfaces;
using App.Server.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Tests.Controllers.RangerControllerTests
{
    public class GetCurrentRangerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAppAuthenticationService> _mockAuthService;
        private readonly Mock<IGenericRepository<Ranger>> _mockRangerRepo;
        private readonly RangerController _controller;

        public GetCurrentRangerTest()
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
        public async Task UserIsNotAuthenticated()
        {
            // arrange
            _mockAuthService.Setup(s => s.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((ApplicationUser?)null);

            // act
            var result = await _controller.GetCurrentRanger();

            //assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No user is signed in.", notFound.Value);

        }

        [Fact]
        public async Task NoRangerAssigned()
        {
            // arrange
            var user = new ApplicationUser { Id = "1", Email = "abcR@gmail.com" };
            _mockAuthService.Setup(s => s.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            // act
            var result = await _controller.GetCurrentRanger();

            //assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No ranger found connected to currently signed in user.", notFound.Value);
        }

        [Fact]
        public async Task RangerDoesNotExist()
        {
            var rangerId = 1;
            var user = new ApplicationUser { Id = "1", Email = "abcR@gmail.com", RangerId = rangerId };
            _mockAuthService.Setup(s => s.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockRangerRepo.Setup(r => r.GetById(rangerId)).ReturnsAsync((Ranger?)null);

            // act
            var result = await _controller.GetCurrentRanger();

            //assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Invalid RangerId for user.", notFound.Value);
        }


        [Fact]
        public async Task GetRangerSuccess()
        {
            var rangerId = 1;
            var ranger = new Ranger { Id = rangerId };
            var user = new ApplicationUser { Id = "1", Email = "abcR@gmail.com", RangerId = rangerId };
            _mockAuthService.Setup(s => s.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockRangerRepo.Setup(r => r.GetById(rangerId)).ReturnsAsync(ranger);

            // act
            var result = await _controller.GetCurrentRanger();

            //assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<RangerDto>(ok.Value);
        }

    }
}
