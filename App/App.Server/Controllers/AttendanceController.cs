using App.Server.DTOs;
using Microsoft.AspNetCore.Mvc;
using App.Server.Repositories.Interfaces;
using App.Server.Models.AppData;
using Microsoft.AspNetCore.Authorization;
using App.Server.Services.Authentication;
using App.Server.Services.Authorization;


namespace App.Server.Controllers
{

    /// <summary>
    /// API controller that updates attendence and manages locks.
    /// </summary>
    /// <param name="unitOfWork">Injected Unit of Work, for accessing repositories.</param>
    /// <param name="authenticationService">Injected authentication service</param>
    /// <param name="authorizationService">Injected authorization service</param>
    [ApiController]
    [Route("api/[controller]")]
    public class AttendenceController(IUnitOfWork unitOfWork, IAppAuthenticationService authenticationService, IAppAuthorizationService authorizationService) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAppAuthenticationService _authenticationService = authenticationService;
        private readonly IAppAuthorizationService _authorizationService = authorizationService;


        /// <summary>
        /// Creates a new attendence, is a private function.
        /// An Attendence instance is created, when some changes are made, otherwise it is 'empty' and default values are used.
        /// </summary>
        /// <param name="date">Date of attendence</param>
        /// <param name="rangerId">Id of a ranger, whose attendence is taken</param>
        /// <returns>Created attendence item</returns>
        /// <exception cref="InvalidOperationException">Ranger with rangerId does not exist.</exception>
        private async Task<Attendence> Create(DateOnly date, int rangerId)
        {
            Ranger? ranger = await _unitOfWork.RangerRepository.GetById(rangerId)
                ?? throw new InvalidOperationException($"Ranger with id {rangerId} not found.");

            var attend = new Attendence(date, ranger);

            _unitOfWork.AttendenceRepository.Add(attend);
            await _unitOfWork.SaveAsync();
            return attend;
        }

        /// <summary>
        /// Updates Attendenc.
        /// If attendence for that date and ranger is not yet marked, tries to create a new one.
        /// </summary>
        /// <param name="attendenceDto">Attendce to be updated.</param>
        /// <returns> The updated AttendenceDto with status 200 if succesfull, else 400 Bad Request (the ranger doesnt exist) </returns>
        [Authorize(Roles = "Ranger,HeadOfDistrict")]
        [HttpPut("update")]
        public async Task<ActionResult<AttendenceDto>> Update(AttendenceDto attendenceDto)
        {
            var attend = await _unitOfWork.AttendenceRepository.GetById(attendenceDto.Date, attendenceDto.Ranger.Id);
            if (attend == null)
            {
                try
                {
                    attend = await Create(attendenceDto.Date, attendenceDto.Ranger.Id);
                }
                catch (InvalidOperationException e)
                {
                    return BadRequest(e.Message);
                }
            }

            attend.Working = attendenceDto.Working;
            attend.ReasonOfAbsenceEnum = attendenceDto.ReasonOfAbsence;
            attend.From = attendenceDto.From;
            _unitOfWork.AttendenceRepository.Update(attend);
            await _unitOfWork.SaveAsync();

            return Ok(attend.ToDto());
        }

        /// <summary>
        /// Get attendence by district and date range.
        /// </summary>
        /// <param name="districtId"> Id of the district.</param>
        /// <param name="startDate">Start date of the range.</param>
        /// <param name="endDate">End date of the range</param>
        /// <returns>
        /// A list of AttendenceDto representing the found attendence in the given range and district.
        /// Status Code 200 Ok, if succesful, else NotFound.     
        /// </returns>
        [Authorize(Roles = "Ranger,HeadOfDistrict,Admin")]
        [HttpGet("by-dates/{districtId}/{startDate}/{endDate}")]
        public async Task<ActionResult<IEnumerable<AttendenceDto>>> GetAttendenceByDateRange(int districtId, DateOnly startDate, DateOnly endDate)
        {
            var attendences = await _unitOfWork.AttendenceRepository.Get(attend => attend.Ranger.DistrictId == districtId && attend.Date >= startDate && attend.Date <= endDate, null, "Ranger");
            if(attendences == null)
            {
                return NotFound("No attendences found in range");
            }
            var attendencesDtos = attendences.Select(attend => attend.ToDto()).ToList();
            return Ok(attendencesDtos);
        }
    }
}
