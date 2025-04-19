using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using App.Server.Services.Authentication;
using App.Server.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace App.Server.Controllers
{

    /// <summary>
    /// API controller that updates attendence.
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
    }
}
