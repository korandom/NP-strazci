using App.Server.DTOs;
using App.Server.Models;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using App.Server.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RangerController(IUnitOfWork unitOfWork, IAppAuthenticationService authenticationService) : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAppAuthenticationService _authenticationService = authenticationService;

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<RangerDto>> GetCurrentRanger()
        {
            var user = await _authenticationService.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("No user is signed in.");
            }
            if (user.RangerId == null)
            {
                return NotFound("No ranger found connected to currently signed in user.");
            }
            var ranger = await _unitOfWork.RangerRepository.GetById(user.RangerId);
            if (ranger == null)
            {
                return NotFound("Invalid RangerId for user.");
            }
            return Ok(ranger.ToDto());
        }

        [Authorize]
        [HttpGet("in-district/{DistrictId}")]
        public async Task<ActionResult<IEnumerable<RangerDto>>> GetRangersInDistrict(int DistrictId)
        {
            var rangers = await _unitOfWork.RangerRepository.Get(ranger => ranger.DistrictId == DistrictId);
            if (rangers == null )
            {
                return NotFound("Failed to fetch rangers.");
            }
            var rangerDtos = rangers.Select(ranger => ranger.ToDto()).ToList();
            return Ok(rangerDtos);
        }

        [Authorize(Roles = "Admin,HeadOfDistrict")]
        [HttpPost("create")]
        public async Task<ActionResult<RangerDto>> Create(RangerDto rangerDto)
        {
            var district = await _unitOfWork.DistrictRepository.GetById(rangerDto.DistrictId);
            if (district == null)
            {
                return BadRequest("District id not found");
            }
            Ranger ranger = new()
            {
                FirstName = rangerDto.FirstName,
                LastName = rangerDto.LastName,
                Email = rangerDto.Email,
                DistrictId = rangerDto.DistrictId,
                District = district
            };

            _unitOfWork.RangerRepository.Add(ranger);
            await _unitOfWork.SaveAsync();
            return Ok(ranger.ToDto());
        }

        [Authorize(Roles = "Admin,HeadOfDistrict")]
        [HttpDelete("delete/{RangerId}")]
        public async  Task<ActionResult> Delete(int RangerId)
        {
            var ranger = await _unitOfWork.RangerRepository.GetById(RangerId);
            if (ranger == null)
            {
                return BadRequest("Ranger id not found");
            }

            _unitOfWork.RangerRepository.Delete(ranger);
            await _unitOfWork.SaveAsync();
            return Ok("Succesfully deleted ranger");
        }

        [Authorize(Roles = "Admin,HeadOfDistrict")]
        [HttpPut("update")]
        public async Task<ActionResult<RangerDto>> Update(RangerDto rangerDto)
        {
            var ranger = await _unitOfWork.RangerRepository.GetById(rangerDto.Id);
            if (ranger == null)
            {
                return NotFound("Ranger not found");
            }

            ranger.FirstName = rangerDto.FirstName;
            ranger.LastName = rangerDto.LastName;
            ranger.Email = rangerDto.Email;
            ranger.DistrictId = rangerDto.DistrictId;

            _unitOfWork.RangerRepository.Update(ranger);
            await _unitOfWork.SaveAsync();

            return Ok(ranger.ToDto());  
        }
    }
}
