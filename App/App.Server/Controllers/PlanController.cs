using App.Server.CSP;
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
    /// API controller for managing plans and locks on plans, adding routes, vehicles to plans, getting plans etc.
    /// </summary>
    /// <param name="unitOfWork">Injected Unit of Work, for accessing repositories.</param>
    /// <param name="authenticationService">Injected authentication service</param>
    /// <param name="authorizationService">Injected authorization service</param>
    [ApiController]
    [Route("api/[controller]")]
    public class PlanController(IUnitOfWork unitOfWork, IAppAuthenticationService authenticationService, IAppAuthorizationService authorizationService, IRoutePlanGenerator routePlanGenerator) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAppAuthenticationService _authenticationService = authenticationService;
        private readonly IAppAuthorizationService _authorizationService = authorizationService;
        private readonly IRoutePlanGenerator _routePlanGenerator = routePlanGenerator;

        /// <summary>
        /// Creates a new plan, is a private function.
        /// Plan is created when a route/vehicle is added to a so far 'empty' plan.
        /// </summary>
        /// <param name="date">Date of plan</param>
        /// <param name="rangerId">Id of a ranger, owner of the plan</param>
        /// <returns>Created plan</returns>
        /// <exception cref="InvalidOperationException">Ranger with rangerId does not exist.</exception>
        private async Task<Plan> Create(DateOnly date, int rangerId)
        {
            Ranger? ranger = await _unitOfWork.RangerRepository.GetById(rangerId)
                ?? throw new InvalidOperationException("Ranger id not found.");

            var plan = new Plan(date, ranger);

            _unitOfWork.PlanRepository.Add(plan);
            await _unitOfWork.SaveAsync();
            return plan;
        }

        /// <summary>
        /// Adds route with routeId to a plan with date and rangerId. 
        /// If such plan doesnt exist, creates a new plan.
        /// Requires user authorization - user must be either owner of the plan, or head of district.
        /// </summary>
        /// <param name="date">Date of plan</param>
        /// <param name="rangerId">Id of plan owner ranger</param>
        /// <param name="routeId">Id of added route</param>
        /// <returns>Status Code 200 Ok, if succesful, else 400 Bad Request</returns>
        [Authorize(Roles = "Ranger,HeadOfDistrict")]
        [HttpPut("add-route/{date}/{rangerId}")]
        public async Task<IActionResult> AddRoute(DateOnly date, int rangerId, int routeId)
        {
            var user = await _authenticationService.GetUserAsync(User);
            // if user is not authorized
            if (user == null
            || (!_authorizationService.IsUserOwner(user, rangerId) && !await _authorizationService.IsInRoleAsync(user, "HeadOfDistrict"))
            )
            {
                return Unauthorized("User is not authorized to change this plan.");
            }

            var plan = await _unitOfWork.PlanRepository.GetById(date, rangerId);

            // if plan not yet created, try to create
            if (plan == null)
            {
                try
                {
                    plan = await Create(date, rangerId);
                }
                catch (InvalidOperationException e)
                {
                    return BadRequest(e.Message);
                }
            }

            var route = await _unitOfWork.RouteRepository.GetById(routeId);
            if (route == null)
                return BadRequest("Route id not found.");

            // add route to plan 
            plan.Routes.Add(route);
            route.Plans.Add(plan);

            _unitOfWork.PlanRepository.Update(plan);

            await _unitOfWork.SaveAsync();

            return Ok(plan.ToDto());
        }

        /// <summary>
        /// Removes route with routeId from a plan with date and rangerId.
        /// Requires Authorization - user must be either owner of the plan or a head of district.
        /// If after removing the route the plan is empty, it is deleted from the database.
        /// </summary>
        /// <param name="date">Date of plan</param>
        /// <param name="rangerId">Id of plan owner ranger</param>
        /// <param name="routeId">Id of route being removed</param>
        /// <returns>Status Code 200 Ok, if succesful, else 400 Bad Request</returns>
        [Authorize(Roles = "Ranger,HeadOfDistrict")]
        [HttpPut("remove-route/{date}/{rangerId}")]
        public async Task<IActionResult> RemoveRoute(DateOnly date, int rangerId, int routeId)
        {
            var user = await _authenticationService.GetUserAsync(User);

            // if user is not authorized
            if (user == null
            || (!_authorizationService.IsUserOwner(user, rangerId) && !await _authorizationService.IsInRoleAsync(user, "HeadOfDistrict"))
            )
            {
                return Unauthorized("User is not authorized to change this plan.");
            }

            var plan = await _unitOfWork.PlanRepository.GetById(date, rangerId);

            if (plan == null)
                return BadRequest("Plan not found.");

            var route = await _unitOfWork.RouteRepository.GetById(routeId);
            if (route == null)
                return BadRequest("Route id not found.");

            // remove route from plan
            plan.Routes.Remove(route);
            route.Plans.Remove(plan);

            _unitOfWork.PlanRepository.Update(plan);

            // Check if the plan has no routes and no vehicles
            if (plan.Routes.Count == 0 && plan.Vehicles.Count == 0)
            {
                _unitOfWork.PlanRepository.Delete(plan);
            }

            await _unitOfWork.SaveAsync();

            return Ok(plan.ToDto());
        }

        /// <summary>
        /// Adds vehicle with vehicleId to a plan with date and rangerId. 
        /// If such plan doesnt exist, creates a new plan.
        /// Requires user authorization - user must be either owner of the plan, or head of district.
        /// </summary>
        /// <param name="date">Date of plan</param>
        /// <param name="rangerId">Id of plan owner ranger</param>
        /// <param name="vehicleId">Id of added vehicle</param>
        /// <returns>Status Code 200 Ok, if succesful, else 400 Bad Request</returns>
        [Authorize(Roles = "HeadOfDistrict")]
        [HttpPut("add-vehicle/{date}/{rangerId}")]
        public async Task<IActionResult> AddVehicle(DateOnly date, int rangerId, int vehicleId)
        {
            var plan = await _unitOfWork.PlanRepository.GetById(date, rangerId);

            // if plan not yet created, try to create
            if (plan == null)
            {
                try
                {
                    plan = await Create(date, rangerId);
                }
                catch (InvalidOperationException e)
                {
                    return BadRequest(e.Message);
                }
            }

            var vehicle = await _unitOfWork.VehicleRepository.GetById(vehicleId);
            if (vehicle == null)
                return BadRequest("Vehicle id not found.");

            // add vehicle to plan
            plan.Vehicles.Add(vehicle);
            vehicle.Plans.Add(plan);

            _unitOfWork.PlanRepository.Update(plan);

            await _unitOfWork.SaveAsync();

            return Ok(plan.ToDto());
        }

        /// <summary>
        /// Removes vehicle with vehicleId from a plan with date and rangerId.
        /// Requires Authorization - user must be either owner of the plan or a head of district.
        /// If after removing the vehicle the plan is empty, it is deleted from the database.
        /// </summary>
        /// <param name="date">Date of plan</param>
        /// <param name="rangerId">Id of plan owner ranger</param>
        /// <param name="vehicleId">Id of vehicle being removed</param>
        /// <returns>Status Code 200 Ok, if succesful, else 400 Bad Request</returns>
        [Authorize(Roles = "HeadOfDistrict")]
        [HttpPut("remove-vehicle/{date}/{rangerId}")]
        public async Task<IActionResult> RemoveVehicle(DateOnly date, int rangerId, int vehicleId)
        {
            var plan = await _unitOfWork.PlanRepository.GetById(date, rangerId);

            if (plan == null)
                return NotFound("Plan not found.");

            var vehicle = await _unitOfWork.VehicleRepository.GetById(vehicleId);
            if (vehicle == null)
                return BadRequest("Vehicle id not found.");

            // remove vehicle from plan
            plan.Vehicles.Remove(vehicle);
            vehicle.Plans.Remove(plan);

            _unitOfWork.PlanRepository.Update(plan);

            // Check if the plan has no routes and no vehicles
            if (plan.Routes.Count == 0 && plan.Vehicles.Count == 0)
            {
                _unitOfWork.PlanRepository.Delete(plan);
            }

            await _unitOfWork.SaveAsync();

            return Ok(plan.ToDto());
        }

        /// <summary>
        /// Function that updates or creates multiple plans at once.
        /// Useful for automatic generation of plans, to lessen the load of requests.
        /// </summary>
        /// <param name="plans">Updated plans</param>
        /// <returns>Status Code 200 Ok, if succesful, else 400 Bad Request if unknown ranger id, else 500 </returns>
        [Authorize(Roles = "HeadOfDistrict")]
        [HttpPost("updateAll")]
        public async Task<IActionResult> UpdatePlans(IEnumerable<PlanDto> planDtos)
        {
            try
            {
                foreach (var planDto in planDtos)
                {
                    await UpdatePlanInternal(planDto);
                }

                await _unitOfWork.SaveAsync();
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // creating of new plans not succesfull - bad ranger data
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the plans.");
            }
        }
        /// <summary>
        /// Creates a new plan or updates it if it already exists.
        /// </summary>
        /// <param name="planDto">Plan to be updated</param>
        /// <returns>Status Code 200 Ok, if succesful, else 400 Bad Request if unknown ranger id, else 500</returns>
        [Authorize(Roles = "HeadOfDistrict,Ranger")]
        [HttpPost("update")]
        public async Task<IActionResult> UpdatePlan(PlanDto planDto)
        {
            try
            {
                await UpdatePlanInternal(planDto);
                await _unitOfWork.SaveAsync();
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the plan.");
            }
        }


        /// <summary>
        /// Generates a route plan for a week, considering preexisting plans, ranger attendence, route priorities and route distribution between rangers in a given district.
        /// 
        /// </summary>
        /// <param name="districtId">Id of the district</param>
        /// <param name="startDate">Start of the week, for which generating is done</param>
        /// <returns> A GenerateResultDto. </returns>
        [Authorize(Roles = "HeadOfDistrict,Admin")]
        [HttpGet("generate/{districtId}/{startDate}")]
        public async Task<ActionResult<GenerateResultDto>> GenerateRoutePlan(int districtId, DateOnly startDate)
        {
            int planningPeriod = 7;
            int numberOfDaysPreprocess = 7 * 3;
            var preexistingPlans = await _unitOfWork.PlanRepository.Get(
                plan => plan.Ranger.DistrictId == districtId &&
                plan.Date >= startDate &&
                plan.Date < startDate.AddDays(planningPeriod), "Routes,Vehicles,Ranger");
            if (preexistingPlans == null)
            {
                return NotFound("Getting preexisting plans was not successful.");
            }
            List<PlanDto> preexistingPlansDto = preexistingPlans.Select(plan => plan.ToDto()).ToList();

            var previousPlans = await _unitOfWork.PlanRepository.Get(
                plan => plan.Ranger.DistrictId == districtId &&
                plan.Date < startDate &&
                plan.Date >= startDate.AddDays(-numberOfDaysPreprocess), "Routes,Vehicles,Ranger");
            if (previousPlans == null)
            {
                return NotFound("Getting previous plans was not successful.");
            }
            List<PlanDto> previousPlansDto = previousPlans.Select(plan => plan.ToDto()).ToList();

            var attendence = await _unitOfWork.AttendenceRepository.Get
                (
                    att => att.Ranger.DistrictId == districtId &&
                    att.Date >= startDate &&
                    att.Date < startDate.AddDays(planningPeriod), "Ranger"
                );
            if (attendence == null)
            {
                return NotFound("Getting attendence for planning was not successful");
            }
            List<AttendenceDto> attendenceDtos = attendence.Select(a => a.ToDto()).ToList();

            var routes = await _unitOfWork.RouteRepository.Get(route => route.DistrictId == districtId);
            if (routes == null)
            {
                return NotFound("Getting routes of district was not successfull");
            }
            List<RouteDto> routeDto = routes.Select(r => r.ToDto()).ToList();

            var rangers = await _unitOfWork.RangerRepository.Get(ranger => ranger.DistrictId == districtId);
            if (rangers == null)
            {
                return NotFound("Getting rangers of district was not successfull");
            }
            List<RangerDto> rangersDto = rangers.Select(r => r.ToDto()).ToList();

            var result = _routePlanGenerator.Generate(previousPlansDto, preexistingPlansDto, attendenceDtos, routeDto, rangersDto, startDate);

            return Ok(result);
        }

        private async Task UpdatePlanInternal(PlanDto planDto)
        {
            var plan = await _unitOfWork.PlanRepository.GetById(planDto.Date, planDto.Ranger.Id);

            // if plan not yet created, try to create
            if (plan == null)
            {
                plan = await Create(planDto.Date, planDto.Ranger.Id);
            }

            var routes = await _unitOfWork.RouteRepository.Get(r => planDto.RouteIds.Contains(r.Id));
            plan.Routes = new List<Models.AppData.Route>(routes);

            var vehicles = await _unitOfWork.VehicleRepository.Get(v => planDto.VehicleIds.Contains(v.Id));
            plan.Vehicles = new List<Vehicle>(vehicles);
            _unitOfWork.PlanRepository.Update(plan);
        }


        /// <summary>
        /// Get rangerSchedules - plans and attendences merged -  by district and date range.
        /// </summary>
        /// <param name="districtId"> Id of the district.</param>
        /// <param name="startDate">Start date of the range.</param>
        /// <param name="endDate">End date of the range</param>
        /// <returns>
        /// A list of RangerScheduleDto representing the found attendences and plans in the given range and district.
        /// Status Code 200 Ok, if succesful, else NotFound.     
        /// </returns>
        [Authorize(Roles = "Ranger,HeadOfDistrict,Admin")]
        [HttpGet("by-dates/{districtId}/{startDate}/{endDate}")]
        public async Task<ActionResult<IEnumerable<RangerScheduleDto>>> GetRangerSchedulesInRange(int districtId, DateOnly startDate, DateOnly endDate)
        {
            var plans = await _unitOfWork.PlanRepository.Get(plan => plan.Ranger.DistrictId == districtId && plan.Date >= startDate && plan.Date <= endDate, "Routes,Vehicles,Ranger");

            var attendances = await _unitOfWork.AttendenceRepository.Get(attend => attend.Ranger.DistrictId == districtId && attend.Date >= startDate && attend.Date <= endDate, "Ranger");

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
