using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;


namespace Tests.Controllers.DistrictControllerTests
{
    public class GetAllTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<District>> _mockDistrictRepository;
        private readonly DistrictController _districtController;

        public GetAllTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockDistrictRepository = new Mock<IGenericRepository<District>>();
            _mockUnitOfWork.Setup(u => u.DistrictRepository).Returns(_mockDistrictRepository.Object);

            _districtController = new DistrictController(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task ReturnsExistingDistricts()
        {
            // arrange
            var districts = new List<District>
            {
                new() { Id = 1, Name = "District 1" },
                new() { Id = 2, Name = "District 2" }
            };

            _mockDistrictRepository.Setup(r => r.Get(null, "")).ReturnsAsync(districts);

            // act
            var result = await _districtController.GetAllDistricts();

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<DistrictDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
        }

        [Fact]
        public async Task GetAllDistricts_ReturnsNotFound_WhenNoDistrictsExist()
        {
            // arrange
            _mockDistrictRepository.Setup(r => r.Get(null, "")).ReturnsAsync([]);

            // act
            var result = await _districtController.GetAllDistricts();

            // assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No districts found.", notFound.Value);
        }
    }
}
