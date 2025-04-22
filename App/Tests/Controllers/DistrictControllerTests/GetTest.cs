using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers.DistrictControllerTests
{
    public class GetTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<District>> _mockDistrictRepository;
        private readonly DistrictController _districtController;

        public GetTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockDistrictRepository = new Mock<IGenericRepository<District>>();
            _mockUnitOfWork.Setup(u => u.DistrictRepository).Returns(_mockDistrictRepository.Object);

            _districtController = new DistrictController(_mockUnitOfWork.Object);
        }


        [Fact]
        public async Task DistrictExists()
        {
            // arrange
            var districtId = 1;
            var district = new District { Id = districtId, Name = "District" };
            _mockDistrictRepository.Setup(r => r.GetById(districtId)).ReturnsAsync(district);

            // act
            var result = await _districtController.GetDistrict(districtId);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<DistrictDto>(okResult.Value);
            Assert.Equal("District", dto.Name);
            Assert.Equal(districtId, dto.Id);
        }

        [Fact]
        public async Task DistrictDoesNotExist()
        {
            // arrange
            var districtId = 1;
            _mockDistrictRepository.Setup(r => r.GetById(districtId)).ReturnsAsync((District?)null);

            // act
            var result = await _districtController.GetDistrict(districtId);

            // assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("District not found.", notFound.Value);
        }
    }
}