using App.Server.Models;
using App.Server.DTOs;
using Microsoft.AspNetCore.Mvc;
using App.Server.Repositories.Interfaces;
using App.Server.Models.AppData;
using Microsoft.AspNetCore.Authorization;
using App.Server.Services.Authentication;
using App.Server.Services.Authorization;


namespace App.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanController(IUnitOfWork unitOfWork, IAppAuthenticationService authenticationService, IAppAuthorizationService authorizationService) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAppAuthenticationService _authenticationService = authenticationService;
        private readonly IAppAuthorizationService _authorizationService = authorizationService;

        private async Task<Plan> Create(DateOnly date, int rangerId)
        {
            Ranger? ranger = await _unitOfWork.RangerRepository.GetById(rangerId)
                ?? throw new InvalidOperationException("Ranger id not found.");

            var plan = new Plan(date, ranger);

            _unitOfWork.PlanRepository.Add(plan);
            await _unitOfWork.SaveAsync();
            return plan;
        }

        /* NOT NEEDED SO FAR
        [HttpGet("{date}/{rangerId}")]
        public async Task<ActionResult<PlanDto>> GetById(DateOnly date, int rangerId)
        {
            var plan = await _unitOfWork.PlanRepository.GetById(date, rangerId);

            if (plan == null)
            {
                return NotFound();
            }

            return Ok(plan.ToDto());
        }
        */

        [Authorize(Roles = "Ranger,HeadOfDistrict")]
        [HttpPut("add-route/{date}/{rangerId}")]
        public async Task<IActionResult> AddRoute(DateOnly date, int rangerId, int routeId)
        {
            var user = await _authenticationService.GetUserAsync(User);
            // if user is not authorized
            if (user == null 
            || ( !_authorizationService.IsUserOwner(user, rangerId) && !await _authorizationService.IsInRoleAsync(user, "HeadOfDistrict"))
            ){
                return Unauthorized();
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

            return Ok("Succesfully added route to plan.");
        }

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
                return Unauthorized();
            }

            var plan = await _unitOfWork.PlanRepository.GetById(date, rangerId);

            if (plan == null)
                return NotFound("Plan not found.");

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

            return Ok("Successfully removed route from plan.");
        }

        // Add a vehicle to the plan
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

            return Ok("Successfully added vehicle to plan.");
        }

        // Remove a vehicle from the plan
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

            return Ok("Successfully removed vehicle from plan.");
        }

        // TODO locking per days per district?
        // Lock or unlock the plan
        [Authorize(Roles = "HeadOfDistrict")]
        [HttpPut("lock/{date}/{rangerId}")]
        public async Task<IActionResult> LockPlan(DateOnly date, int rangerId, bool is_locked)
        {
            var plan = await _unitOfWork.PlanRepository.GetById(date, rangerId);

            if (plan == null)
                return NotFound("Plan not found.");

            plan.Locked = is_locked;

            _unitOfWork.PlanRepository.Update(plan);
            await _unitOfWork.SaveAsync();

            return Ok("Successfully updated lock of the plan.");
        }

        // TODO finish - by district, 
        // Get plans by date range
        [Authorize(Roles = "Ranger,HeadOfDistrict")]
        [HttpGet("by-dates/{startDate}/{endDate}")]
        public async Task<ActionResult<IEnumerable<PlanDto>>> GetPlansByDateRange(DateOnly startDate, DateOnly endDate)
        {
            var plans = await _unitOfWork.PlanRepository.Get(plan => plan.Date >= startDate && plan.Date <= endDate, null, "Routes,Vehicles,Ranger");
            if(plans == null)
            {
                return NotFound("No plans found in range");
            }
            var planDtos = plans.Select(plan => plan.ToDto()).ToList();
            return Ok(planDtos);
        }
        // TODO finish - by district
        [Authorize(Roles = "Ranger,HeadOfDistrict")]
        [HttpGet("{date}")]
        public async Task<ActionResult<IEnumerable<PlanDto>>> GetPlansByDate(DateOnly date)
        {
            var plans = await _unitOfWork.PlanRepository.Get(plan => plan.Date == date, null, "Routes,Vehicles,Ranger");

            var planDtos = plans.Select(plan => plan.ToDto()).ToList();
            return Ok(planDtos);
        }
    }
}
