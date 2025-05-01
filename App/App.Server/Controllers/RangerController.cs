using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using App.Server.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{
    /// <summary>
    /// API controller for managing rangers - creating new, deleting, updating, get all rangers in district, or get ranger who is the currently signed in user.
    /// </summary>
    /// <param name="unitOfWork">Injected Unit Of Work, for accesing repositories</param>
    /// <param name="authenticationService">Injected authentication service</param>
    [ApiController]
    [Route("api/[controller]")]
    public class RangerController(IUnitOfWork unitOfWork, IAppAuthenticationService authenticationService) : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAppAuthenticationService _authenticationService = authenticationService;

        /// <summary>
        /// Get the ranger, that is the current user - assigned.
        /// </summary>
        /// <returns>Status Code 200 and the ranger if success,
        /// else 404 not found (no user signed in, or signed in user has no ranger assigned or assigned ranger does not exist)</returns>
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

        /// <summary>
        /// Get all rangers in certain district.
        /// </summary>
        /// <param name="districtId">Id of said district</param>
        /// <returns>Status code 200 and IEnumerable of RangerDtos in the district, or 404 Not Found when failed to get rangers.</returns>
        [Authorize]
        [HttpGet("in-district/{districtId}")]
        public async Task<ActionResult<IEnumerable<RangerDto>>> GetRangersInDistrict(int districtId)
        {
            var district = await _unitOfWork.DistrictRepository.GetById(districtId);
            if (district == null)
            {
                return NotFound("District not found.");
            }
            var rangers = await _unitOfWork.RangerRepository.Get(ranger => ranger.DistrictId == districtId);
            if (rangers == null)
            {
                return NotFound("Failed to fetch rangers.");
            }
            var rangerDtos = rangers.Select(ranger => ranger.ToDto()).ToList();
            return Ok(rangerDtos);
        }

        /// <summary>
        /// Create new Ranger.
        /// </summary>
        /// <param name="rangerDto">New Ranger.</param>
        /// <returns>Status 200 Ok when success, else if ranger belongs to non-existent district 400 BadRequest.</returns>
        [Authorize(Roles = "Admin,HeadOfDistrict")]
        [HttpPost("create")]
        public async Task<ActionResult<RangerDto>> Create(RangerDto rangerDto)
        {
            var district = await _unitOfWork.DistrictRepository.GetById(rangerDto.DistrictId);
            if (district == null)
            {
                return BadRequest("District id not found.");
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

        /// <summary>
        ///  Attempts to delete a ranger with said id.
        /// </summary>
        /// <param name="rangerId"> Id of ranger being deleted/</param>
        /// <returns>Status code 200 ok, if succesfully deleted, or 400 BadRequest if the rangerId is not found.</returns>
        [Authorize(Roles = "Admin,HeadOfDistrict")]
        [HttpDelete("delete/{rangerId}")]
        public async Task<ActionResult> Delete(int rangerId)
        {
            var ranger = await _unitOfWork.RangerRepository.GetById(rangerId);
            if (ranger == null)
            {
                return BadRequest("Ranger id not found.");
            }

            _unitOfWork.RangerRepository.Delete(ranger);
            await _unitOfWork.SaveAsync();
            return Ok("Succesfully deleted ranger");
        }

        /// <summary>
        /// Update data of Ranger.
        /// </summary>
        /// <param name="rangerDto">Updated Ranger</param>
        /// <returns>Status code 200 ok, if successfully updated, else if ranger was not found in repository by the given id, status code 400 Bad Request. </returns>
        [Authorize(Roles = "Admin,HeadOfDistrict")]
        [HttpPut("update")]
        public async Task<ActionResult<RangerDto>> Update(RangerDto rangerDto)
        {
            var ranger = await _unitOfWork.RangerRepository.GetById(rangerDto.Id);
            if (ranger == null)
            {
                return NotFound("Ranger not found.");
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
