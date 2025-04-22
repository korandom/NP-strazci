using App.Server.Controllers;
using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using App.Server.Repositories;
using App.Server.Services.Authentication;
using App.Server.Services.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Controllers.AttendenceControllerTests
{
    public class UpdateTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Attendence>> _mockAttendenceRepository;
        private readonly Mock<IGenericRepository<Ranger>> _mockRangerRepository;
        private readonly AttendenceController _attendenceController;

        public UpdateTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAttendenceRepository = new Mock<IGenericRepository<Attendence>>();
            _mockRangerRepository = new Mock<IGenericRepository<Ranger>>();

            _mockUnitOfWork.Setup(u => u.AttendenceRepository).Returns(_mockAttendenceRepository.Object);
            _mockUnitOfWork.Setup(u => u.RangerRepository).Returns(_mockRangerRepository.Object);

            _attendenceController = new AttendenceController(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task UpdateExistingAttendence()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            var rangerId = 1;

            var attendence = new Attendence(date, new Ranger { Id = rangerId });
            attendence.Working = true;
            _mockAttendenceRepository.Setup(repo => repo.GetById(date, rangerId)).ReturnsAsync(attendence);

            var dto = new AttendenceDto
            {
                Date = date,
                Ranger = new RangerDto { Id = rangerId },
                Working = false,
                ReasonOfAbsence = ReasonOfAbsence.D,
                From = null
            };

            // act
            var result = await _attendenceController.Update(dto);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<AttendenceDto>(okResult.Value);
            _mockAttendenceRepository.Verify(r => r.Update(It.IsAny<Attendence>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateNewAttendence()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            var rangerId = 2;

            _mockAttendenceRepository.Setup(r => r.GetById(date, rangerId)).ReturnsAsync((Attendence?)null);
            _mockRangerRepository.Setup(r => r.GetById(rangerId)).ReturnsAsync(new Ranger { Id = rangerId });

            var dto = new AttendenceDto
            {
                Date = date,
                Ranger = new RangerDto { Id = rangerId },
                Working = false,
                ReasonOfAbsence = ReasonOfAbsence.D,
                From = null
            };

            // act
            var result = await _attendenceController.Update(dto);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<AttendenceDto>(okResult.Value);
            _mockAttendenceRepository.Verify(r => r.Add(It.IsAny<Attendence>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Exactly(2)); // once when creating, then updating
        }

        [Fact]
        public async Task RangerDoesntExist()
        {
            // arrange
            var date = new DateOnly(2025, 1, 1);
            var rangerId = 3;

            _mockAttendenceRepository.Setup(r => r.GetById(date, rangerId)).ReturnsAsync((Attendence?)null);
            _mockRangerRepository.Setup(r => r.GetById(rangerId)).ReturnsAsync((Ranger?)null);

            var dto = new AttendenceDto
            {
                Date = date,
                Ranger = new RangerDto { Id = rangerId },
                Working = false,
                ReasonOfAbsence = ReasonOfAbsence.N,
                From = null
            };

            // act
            var result = await _attendenceController.Update(dto);

            // assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Ranger with id 3 not found.", badRequestResult.Value);
        }
    }
}
