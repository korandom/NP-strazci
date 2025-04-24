using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{
    /// <summary>
    /// API controller for managing vehicles - updating, creating, deleting and getting all vehicles in a certain district.
    /// </summary>
    /// <param name="unitOfWork">Injecting Unit of Work for accessing repositories</param>
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        /// <summary>
        /// Get all vehicles, that belong to a district with ID DistrictId.
        /// </summary>
        /// <param name="DistrictId">Id of the district</param>
        /// <returns>Status code 200 Ok and a IEnumerable<Vehicle> of all vehicles, or 404 Not Found, if getting vehicles was not successful</returns>
        [Authorize]
        [HttpGet("in-district/{DistrictId}")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetVehiclesInDistrict(int DistrictId)
        {
            var vehicles = await _unitOfWork.VehicleRepository.Get(vehicle => vehicle.DistrictId == DistrictId);
            if (vehicles == null)
            {
                return NotFound("Failed to fetch vehicles.");
            }
            var vehiclesDtos = vehicles.Select(vehicle => vehicle.ToDto()).ToList();
            return Ok(vehiclesDtos);
        }

        /// <summary>
        /// Create new Vehicle. 
        /// </summary>
        /// <param name="vehicleDto">New Vehicle</param>
        /// <returns>Status 200 Ok and new VehicleDto, or 400 BadRequest if district of the new vehicle does not exist.</returns>
        [Authorize(Roles = "Admin,HeadOfDistrict")]
        [HttpPost("create")]
        public async Task<ActionResult<VehicleDto>> Create(VehicleDto vehicleDto)
        {
            var district = await _unitOfWork.DistrictRepository.GetById(vehicleDto.DistrictId);
            if (district == null)
            {
                return BadRequest("District id not found.");
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
        
        /// <summary>
        /// Delete a vehicle by its Id. Irreversible.
        /// </summary>
        /// <param name="vehicleId">Id of the vehicle being deleted.</param>
        /// <returns>Status 200 Ok, or 400 BadRequest if Vehicle could not be found by the Id.</returns>
        [Authorize(Roles = "Admin,HeadOfDistrict")]
        [HttpDelete("delete/{vehicleId}")]
        public async Task<ActionResult> Delete(int vehicleId)
        {
            var vehicle = await _unitOfWork.VehicleRepository.GetById(vehicleId);
            if (vehicle == null)
            {
                return BadRequest("Vehicle id not found.");
            }

            _unitOfWork.VehicleRepository.Delete(vehicle);
            await _unitOfWork.SaveAsync();
            return Ok("Succesfully deleted vehicle.");
        }

        /// <summary>
        /// Update an existing vehicle.
        /// </summary>
        /// <param name="vehicleDto">Updated vehicle.</param>
        /// <returns>Status code 200 Ok with updated vehicle, or 404 NotFound, if the vehicle could not be found by the id.</returns>
        [Authorize(Roles = "Admin,HeadOfDistrict")]
        [HttpPut("update")]
        public async Task<ActionResult<VehicleDto>> Update(VehicleDto vehicleDto)
        {
            var vehicle = await _unitOfWork.VehicleRepository.GetById(vehicleDto.Id);
            if (vehicle == null)
            {
                return NotFound("Vehicle not found.");
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
