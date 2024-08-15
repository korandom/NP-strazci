using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        [Authorize]
        [HttpGet("in-district/{DistrictId}")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetVehiclesInDistrict(int DistrictId)
        {
            var vehicles = await _unitOfWork.VehicleRepository.Get(vehicle => vehicle.DistrictId == DistrictId);
            if (vehicles == null || !vehicles.Any())
            {
                return NotFound("No vehicles found in district");
            }
            var vehiclesDtos = vehicles.Select(vehicle => vehicle.ToDto()).ToList();
            return Ok(vehiclesDtos);
        }

        [Authorize(Roles = "Admin,HeadOfDistrict")]
        [HttpPost("create")]
        public async Task<ActionResult<VehicleDto>> Create(VehicleDto vehicleDto)
        {
            var district = await _unitOfWork.DistrictRepository.GetById(vehicleDto.DistrictId);
            if (district== null)
            {
                return BadRequest("District id not found");
            }
            Vehicle vehicle = new()
            {
                Id = vehicleDto.Id,
                Name = vehicleDto.Name,
                Type = vehicleDto.Type,
                DistrictId = vehicleDto.DistrictId,
            };

            _unitOfWork.VehicleRepository.Add(vehicle);
            await _unitOfWork.SaveAsync();
            return Ok(vehicle.ToDto());
        }

        [Authorize(Roles = "Admin,HeadOfDistrict")]
        [HttpDelete("delete")]
        public async Task<ActionResult> Delete(int vehicleId)
        {
            var vehicle = await _unitOfWork.VehicleRepository.GetById(vehicleId);
            if (vehicle == null)
            {
                return BadRequest("Vehicle id not found");
            }

            _unitOfWork.VehicleRepository.Delete(vehicle);
            await _unitOfWork.SaveAsync();
            return Ok("Succesfully deleted vehicle");
        }

        [Authorize(Roles = "Admin,HeadOfDistrict")]
        [HttpPut("update")]
        public async Task<ActionResult<VehicleDto>> Update(VehicleDto vehicleDto)
        {
            var vehicle = await _unitOfWork.VehicleRepository.GetById(vehicleDto.Id);
            if (vehicle == null)
            {
                return NotFound("Ranger not found");
            }

            vehicle.Name = vehicleDto.Name;
            vehicle.Type = vehicleDto.Type;
            vehicle.DistrictId = vehicleDto.DistrictId;

            _unitOfWork.VehicleRepository.Update(vehicle);
            await _unitOfWork.SaveAsync();

            return Ok(vehicle.ToDto());
        }
    }
}
