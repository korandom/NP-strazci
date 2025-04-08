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
    /// API controller that updates attendence and manages locks.
    /// </summary>
    /// <param name="unitOfWork">Injected Unit of Work, for accessing repositories.</param>
    /// <param name="authenticationService">Injected authentication service</param>
    /// <param name="authorizationService">Injected authorization service</param>
    [ApiController]
    [Route("api/[controller]")]
    public class RangerScheduleController(IUnitOfWork unitOfWork, IAppAuthenticationService authenticationService, IAppAuthorizationService authorizationService) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAppAuthenticationService _authenticationService = authenticationService;
        private readonly IAppAuthorizationService _authorizationService = authorizationService;

        [Authorize(Roles = "Ranger,HeadOfDistrict,Admin")]
        [HttpGet("by-dates/{districtId}/{startDate}/{endDate}")]
        public async Task<ActionResult<IEnumerable<RangerScheduleDto>>> GetRangerSchedulesInRange(int districtId, DateOnly startDate, DateOnly endDate)
        {
            var plans = await _unitOfWork.PlanRepository.Get(plan => plan.Ranger.DistrictId == districtId && plan.Date >= startDate && plan.Date <= endDate, null, "Routes,Vehicles,Ranger");

            var attendances = await _unitOfWork.AttendenceRepository.Get(attend => attend.Ranger.DistrictId == districtId && attend.Date >= startDate && attend.Date <= endDate, null, "Ranger");

            var mergeDict = new Dictionary<string, RangerScheduleDto>();

            // add plans to dictionary
            foreach (var plan in plans)
            {
                string key = $"{plan.Date}-{plan.RangerId}";

                mergeDict[key] = new RangerScheduleDto
                {
                    Ranger = plan.Ranger.ToDto(),
                    Date = plan.Date,
                    VehicleIds = plan.Vehicles.Select(v => v.Id).ToArray(),
                    RouteIds = plan.Routes.Select(r => r.Id).ToArray(),
                };
            }

            // merge attendance into dictionary
            foreach (var attend in attendances)
            {
                string key = $"{attend.Date}-{attend.RangerId}";

                if (!mergeDict.ContainsKey(key))
                {
                    mergeDict[key] = new RangerScheduleDto
                    {
                        Ranger = attend.Ranger.ToDto(),
                        Date = attend.Date,
                        Working = attend.Working,
                        From = attend.From,
                        ReasonOfAbsence = attend.ReasonOfAbsenceEnum
                    };
                }
                else
                {
                    mergeDict[key].Working = attend.Working;
                    mergeDict[key].From = attend.From;
                    mergeDict[key].ReasonOfAbsence = attend.ReasonOfAbsenceEnum;
                }
            }

            var schedules = mergeDict.Values.ToList();

            return Ok(schedules);
        }

    }
}
