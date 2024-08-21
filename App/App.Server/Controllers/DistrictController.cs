using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DistrictController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

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
    }
}
