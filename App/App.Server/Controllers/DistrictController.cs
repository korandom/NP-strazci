using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{
    /// <summary>
    /// API controller for managing district data.
    /// </summary>
    /// <param name="unitOfWork">Injected Unit of Work, for accessing district repository.</param>
    [ApiController]
    [Route("api/[controller]")]
    public class DistrictController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;


        /// <summary>
        /// Gets all districts available. 
        /// Requires authorized user.
        /// </summary>
        /// <returns>
        /// A list of DistrictDtos, representing each district.
        /// With status code 200 Ok, if action sucessful, else 404 NotFound.
        /// </returns>
        [Authorize]
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<DistrictDto>>> GetAllDistricts()
        {
            var districts = await _unitOfWork.DistrictRepository.Get();
            if (districts == null || !districts.Any())
            {
                return NotFound("No districts found.");
            }
            var districtDtos = districts.Select(district => district.ToDto()).ToList();
            return Ok(districtDtos);
        }

        /// <summary>
        /// Gets a district by Id.
        /// Requires authorized user.
        /// </summary>
        /// <param name="districtId">Id of the district</param>
        /// <returns>
        /// A DistrictDto, representing the found district with Id = districtId.
        /// With status code 200 Ok, if action sucessful, else 404 NotFound.
        /// </returns>
        [Authorize]
        [HttpGet("{districtId}")]
        public async Task<ActionResult<DistrictDto>> GetDistrict(int districtId)
        {
            var district = await _unitOfWork.DistrictRepository.GetById(districtId);
            if (district == null)
            {
                return NotFound("District not found.");
            }
            return Ok(district.ToDto());
        }
    }
}
